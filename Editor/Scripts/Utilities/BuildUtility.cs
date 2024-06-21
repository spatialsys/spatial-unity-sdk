using Proyecto26;
using RSG;
using SpatialSys.UnitySDK.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.Build.Pipeline;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Build.Pipeline;

namespace SpatialSys.UnitySDK.Editor
{
    public static class BuildUtility
    {
        private const string SAVED_PROJECT_SETTINGS_ASSET_PATH = "Assets/Spatial/SavedProjectSettings.asset";
        public const string SANDBOX_ADDRESSABLES_BUILD_PATH = "Library/AssetBundles/Sandbox";
        public const string BUILD_DIR = "Exports";
        public static readonly string PACKAGE_EXPORT_PATH = Path.Combine(BUILD_DIR, "spaces.unitypackage");
        public static readonly string[] INVALID_BUNDLE_NAME_CHARS = new string[] { " ", "_", ".", ",", "(", ")", "[", "]", "{", "}", "!", "@", "#", "$", "%", "^", "&", "*", "+", "=", "|", "\\", "/", "?", "<", ">", "`", "~", "'", "\"", ":", ";", "\n", "\t" };
        public static int MAX_SANDBOX_BUNDLE_SIZE = 1000 * 1024 * 1024; // 1 GB; It's higher than the package size limit because we want to allow people to mess around more in the sandbox
        public static int MAX_PACKAGE_SIZE = 500 * 1024 * 1024; // 500 MB

        public static IPromise BuildAndUploadForSandbox(BuildTarget target = BuildTarget.WebGL)
        {
            return UpgradeUtility.PerformUpgradeBeforeBuildIfNecessary()
                // Refetch feature flags, since some validation steps may depend on them.
                .Then(upgradePerformed => SpatialFeatureFlags.Refetch())
                // Must save open scenes for sandbox.
                .Then(() => OnBeforeBuild(saveOpenScenes: true))
                .Then(() => {
                    IPromise<SpatialValidationSummary> validationPromise;
                    if (ProjectConfig.activePackageConfig.isSpaceBasedPackage)
                    {
                        // Make sure the active package is set to the one that contains the active scene
                        SceneAsset currentScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(EditorSceneManager.GetActiveScene().path);
                        ProjectConfig.SetActivePackageBySourceAsset(currentScene);

                        validationPromise = SpatialValidator.RunTestsOnActiveScene(ValidationRunContext.UploadingToSandbox);
                    }
                    else
                    {
                        validationPromise = SpatialValidator.RunTestsOnPackage(ValidationRunContext.UploadingToSandbox);
                    }

                    return CheckValidationPromise(validationPromise);
                })
                .Then(() => {
                    ProcessAndSavePackageAssets();

                    // Create a new backup to the package config asset in case we modify it, so it's easy to revert any changes made to it.
                    EditorUtility.CreateAssetBackup(ProjectConfig.instance);
                    EditorUtility.CreateAssetBackup(ProjectConfig.activePackageConfig);

                    // For avatar-based packages, we need to do some bone orientation standardization
                    ProcessAvatarPackageAssets(ProjectConfig.activePackageConfig);

                    // Switch to target platform
                    if (!EditorUtility.IsPlatformModuleInstalled(target))
                        return Promise.Rejected(new System.Exception($"Platform module for {target} is not installed, which is required for testing in sandbox"));
                    EditorUserBuildSettings.SwitchActiveBuildTarget(BuildPipeline.GetBuildTargetGroup(target), target);

                    // Compile custom C# dlls
                    if (!CompileAssemblyIfExists())
                        return Promise.Rejected(new System.Exception("Failed to compile custom C# scripts"));

                    // Build addressables before the main bundle
                    if (!AddressablesUtility.BuildAddressablesIfNecessary(SANDBOX_ADDRESSABLES_BUILD_PATH, useBrotliCompression: false))
                        return Promise.Rejected(new System.Exception("Failed to build Addressables"));

                    // Auto-assign necessary bundle names
                    AssignBundleNamesToPackageAssets();

                    string bundleDir = Path.Combine(BUILD_DIR, target.ToString());
                    if (Directory.Exists(bundleDir))
                        Directory.Delete(bundleDir, recursive: true);
                    Directory.CreateDirectory(bundleDir);

                    // Compress bundles with LZ4 (ChunkBasedCompression) since web doesn't support LZMA.
                    CompatibilityAssetBundleManifest bundleManifest = CompatibilityBuildPipeline.BuildAssetBundles(
                        bundleDir,
                        BuildAssetBundleOptions.ForceRebuildAssetBundle | BuildAssetBundleOptions.ChunkBasedCompression,
                        target
                    );

                    if (bundleManifest == null)
                        return Promise.Rejected(new System.Exception("Asset bundle build failed"));

                    // We've built the bundles, we don't need to wait for the upload to finish before we restore the backups.
                    // Since we restore the C# assembly, that will trigger a recompile and VS node regeneration, which will hang the upload process.
                    // Delay the recompilation until after the bundle is uploaded.
                    EditorApplication.LockReloadAssemblies();
                    RestoreAssetBackups();

                    Promise uploadPromise = new();
                    EditorCoroutineUtility.StartCoroutineOwnerless(UploadSandboxFilesCoroutine(uploadPromise, ProjectConfig.activePackageConfig, bundleDir));
                    return (IPromise)uploadPromise;
                })
                .Then(() => {
                    bool openBrowser = target == BuildTarget.WebGL;
                    if (openBrowser)
                        EditorUtility.OpenSandboxInBrowser();
                })
                .Catch(exc => {
                    if (exc is RSG.PromiseCancelledException)
                        return;

                    Debug.LogException(exc);
                    (string exceptionMessage, Action onCloseDialog) = GetExceptionMessageAndCallback(exc);
                    if (UnityEditor.EditorUtility.DisplayDialog("Upload to sandbox failed", $"An error occurred while uploading to your sandbox.\n\n{exceptionMessage}", "OK"))
                        onCloseDialog?.Invoke();
                })
                .Finally(() => {
                    RestoreAssetBackups();
                    UnityEditor.EditorUtility.ClearProgressBar();
                    EditorApplication.UnlockReloadAssemblies();
                });
        }

        private static IEnumerator UploadSandboxFilesCoroutine(Promise promise, PackageConfig packageConfig, string bundleDir)
        {
            // Get main bundle name
            string mainBundleName = packageConfig.bundleName;
            if (packageConfig is SpaceTemplateConfig spaceTemplateConfig)
            {
                // Get the variant config for the current scene
                SceneAsset currentSceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(EditorSceneManager.GetActiveScene().path);
                SpaceTemplateConfig.Variant spaceTemplateVariant = spaceTemplateConfig.variants.FirstOrDefault(v => v.scene == currentSceneAsset);
                if (spaceTemplateVariant == null)
                {
                    promise.Reject(new Exception("The current scene isn't one that is assigned to a variant in the package configuration"));
                    yield break;
                }

                mainBundleName = spaceTemplateVariant.bundleName;
            }

            if (string.IsNullOrEmpty(mainBundleName))
            {
                promise.Reject(new Exception("Unable to retrieve the asset bundle name from the prefab. Make sure an asset is assigned in the package configuration."));
                yield break;
            }

            // Gather additional bundles (placeholder for future use)
            List<string> additionalBundles = new();
            if (packageConfig is SpaceConfig spaceConfig && spaceConfig.csharpAssembly != null && File.Exists(CSScriptingEditorUtility.OUTPUT_ASSET_PATH))
                additionalBundles.Add(CSScriptingUtility.CSHARP_ASSEMBLY_BUNDLE_ID);

            // Get upload URLS for each bundle
            bool requestFinalized = false;
            SpatialAPI.UploadSandboxBundleResponse sandboxUploadResponse = null;
            SpatialAPI.UploadSandboxBundle(packageConfig, addressablesEnabled: AddressablesUtility.hasBuiltAddressables, additionalBundles.ToArray())
                .Then(resp => sandboxUploadResponse = resp)
                .Catch(ex => promise.Reject(ex))
                .Finally(() => requestFinalized = true);

            // Wait for completion
            while (!requestFinalized)
                yield return null;
            if (sandboxUploadResponse == null)
                yield break;

            FileUploader fileUploader = new();
            fileUploader.progressBarTitleOverride = "Uploading sandbox assets";

            fileUploader.EnqueueWebRequest(Path.Combine(bundleDir, mainBundleName), sandboxUploadResponse.url, SpatialAPI.UploadFile, maxFileSizeBytes: MAX_SANDBOX_BUNDLE_SIZE);
            for (int i = 0; i < additionalBundles.Count; i++)
            {
                string bundleName = GetBundleNameForPackageAsset(packageConfig.packageName, additionalBundles[i]);
                fileUploader.EnqueueWebRequest(Path.Combine(bundleDir, bundleName), sandboxUploadResponse.additionalBundleUrls[i], SpatialAPI.UploadFile, maxFileSizeBytes: MAX_SANDBOX_BUNDLE_SIZE);
            }

            if (AddressablesUtility.hasBuiltAddressables)
            {
                if (!Directory.Exists(SANDBOX_ADDRESSABLES_BUILD_PATH))
                {
                    promise.Reject(new IOException($"Unable to locate addressables build path: {SANDBOX_ADDRESSABLES_BUILD_PATH}"));
                    yield break;
                }

                string catalogJSONPath = Path.Combine(SANDBOX_ADDRESSABLES_BUILD_PATH, $"catalog_{AddressablesUtility.CATALOG_FILE_SUFFIX}.json");
                fileUploader.EnqueueWebRequest(catalogJSONPath, SpatialAPI.GetUploadSandboxAddressableAssetURL("catalog"), SpatialAPI.UploadSandboxAddressableAsset, maxFileSizeBytes: 10 * 1024 * 1024);
                string catalogHashPath = Path.ChangeExtension(catalogJSONPath, ".hash");
                fileUploader.EnqueueWebRequest(catalogHashPath, SpatialAPI.GetUploadSandboxAddressableAssetURL("catalog-hash"), SpatialAPI.UploadSandboxAddressableAsset, maxFileSizeBytes: 32);

                foreach (string bundlePath in Directory.GetFiles(SANDBOX_ADDRESSABLES_BUILD_PATH, "*.bundle", SearchOption.TopDirectoryOnly))
                {
                    string uploadURL = SpatialAPI.GetUploadSandboxAddressableAssetURL(Path.GetFileName(bundlePath));
                    fileUploader.EnqueueWebRequest(bundlePath, uploadURL, SpatialAPI.UploadSandboxAddressableAsset, maxFileSizeBytes: 10 * 1024 * 1024);
                }
            }

            // Start uploading all files sequentially and wait for completion.
            yield return fileUploader;

            if (fileUploader.exception != null)
            {
                promise.Reject(fileUploader.exception);
                yield break;
            }

            promise.Resolve();
        }

        private static IPromise CheckValidationPromise(IPromise<SpatialValidationSummary> validatorPromise)
        {
            return validatorPromise.Then((SpatialValidationSummary validationSummary) => {
                if (validationSummary == null)
                    return Promise.Rejected(new Exception("Package validation failed to run"));

                if (validationSummary.failed || validationSummary.passedWithWarnings)
                {
                    SpatialSDKConfigWindow.OpenIssuesTabWithSummary(validationSummary);
                    if (validationSummary.failed)
                        return Promise.Rejected(new Exception("Package has errors"));
                }
                return Promise.Resolved();
            });
        }

        public static IPromise BuildAndPublishPackage()
        {
            return UpgradeUtility.PerformUpgradeBeforeBuildIfNecessary()
                // Refetch feature flags, since some validation steps may depend on them.
                .Then(upgradePerformed => SpatialFeatureFlags.Refetch())
                // Must save open scenes when publishing space-based packages.
                .Then(() => {
                    OnBeforeBuild(saveOpenScenes: ProjectConfig.activePackageConfig.isSpaceBasedPackage);

                    // Don't enforce name, let the build machine do it
                    if (!CompileAssemblyIfExists(enforceName: false))
                        throw new System.Exception("Failed to compile custom C# scripts");
                })
                .Then(() => CheckValidationPromise(SpatialValidator.RunTestsOnPackage(ValidationRunContext.PublishingPackage)))
                .Then(() => {
                    ProcessAndSavePackageAssets();

                    // Auto-assign necessary bundle names
                    // This gets done on the build machines too, but we also want to do it here just in case there's an issue
                    AssignBundleNamesToPackageAssets();

                    // For "Space" packages, we need to make sure we have a worldID assigned to the project
                    // Worlds are a way to manage an ecosystem of spaces that share currency, rewards, inventory, etc.
                    // Spaces by default need to be assigned to a world
                    IPromise createWorldPromise = Promise.Resolved();
                    if (ProjectConfig.activePackageConfig.packageType == PackageType.Space)
                    {
                        createWorldPromise = WorldUtility.AssignDefaultWorldToProjectIfNecessary()
                            .Then(() => {
                                // Make sure that the space package has a worldID assigned
                                SpaceConfig spaceConfig = ProjectConfig.activePackageConfig as SpaceConfig;
                                spaceConfig.worldID = ProjectConfig.defaultWorldID;
                                EditorUtility.SaveAssetImmediately(spaceConfig);
                            });
                    }

                    return createWorldPromise;
                })
                .Then(() => SpatialAPI.CreateOrUpdatePackage(ProjectConfig.activePackageConfig.sku, ProjectConfig.activePackageConfig.packageType))
                .Then(resp => {
                    PackageConfig config = ProjectConfig.activePackageConfig;
                    if (config.sku != resp.sku)
                    {
                        config.sku = resp.sku;
                        EditorUtility.SaveAssetImmediately(config);
                    }

                    // Create a new backup to the package config asset in case we modify it, so it's easy to revert any changes made to it.
                    EditorUtility.CreateAssetBackup(ProjectConfig.instance);
                    EditorUtility.CreateAssetBackup(ProjectConfig.activePackageConfig);

                    // For avatar-based packages, we need to do some bone orientation standardization
                    ProcessAvatarPackageAssets(ProjectConfig.activePackageConfig);

                    // Before we package up the project, we need to save all assets so that the package has the latest changes
                    AssetDatabase.SaveAssets();

                    // Package it after SKU is assigned or else the config inside the package will not have the SKU
                    // Export all scenes and dependencies as a package
                    if (!Directory.Exists(BUILD_DIR))
                        Directory.CreateDirectory(BUILD_DIR);
                    PackageProject();

                    // We've created the package, we don't need to wait for the upload to finish before we restore the backups
                    RestoreAssetBackups();

                    Promise uploadPromise = new();
                    EditorCoroutineUtility.StartCoroutineOwnerless(UploadPackageCoroutine(uploadPromise, resp.sku, resp.version));
                    return (IPromise)uploadPromise;
                })
                .Then(() => {
                    UnityEditor.EditorUtility.DisplayDialog(
                        "Upload complete!",
                        "Your package was successfully uploaded and we've started processing it.\n\n" +
                        "This may take as little as 15 minutes to process, depending on our backlog and your package complexity. If any unexpected errors occur, we aim to address the issue within 24 hours.\n\n" +
                        "You will receive an email notification once the package has been published.",
                        "OK"
                    );
                })
                .Catch(exc => {
                    if (exc is RSG.PromiseCancelledException)
                        return;

                    Debug.LogException(exc);
                    (string exceptionMessage, Action onCloseDialog) = GetExceptionMessageAndCallback(exc);
                    if (UnityEditor.EditorUtility.DisplayDialog("Publishing package failed", $"An error occurred while publishing your package.\n\n{exceptionMessage}", "OK"))
                    {
                        onCloseDialog?.Invoke();
                    }
                })
                .Finally(() => {
                    UnityEditor.EditorUtility.ClearProgressBar();
                    RestoreAssetBackups();
                });
        }

        private static IEnumerator UploadPackageCoroutine(Promise promise, string sku, int version)
        {
            FileUploader fileUploader = new();
            fileUploader.progressBarTitleOverride = "Uploading package";
            fileUploader.EnqueueWebRequest(PACKAGE_EXPORT_PATH, SpatialAPI.GetUploadPackageURL(sku, version), SpatialAPI.UploadPackage, maxFileSizeBytes: MAX_PACKAGE_SIZE);
            yield return fileUploader;

            if (fileUploader.exception != null)
            {
                promise.Reject(fileUploader.exception);
                yield break;
            }

            promise.Resolve();
        }

        private static void PackageProject()
        {
            // TODO: we can also exclude dependencies from packages that are included in the builder project by default
            HashSet<string> assetPaths = new();
            EditorUtility.UnionWithAssetDependenciesPaths(assetPaths, ProjectConfig.activePackageConfig);

            // For `.obj` files in dependencies, we need to also manually include the `.mtl` file as a dependency
            // or else when the package extracted into the builder project, the materials will not be the same
            // as they were in the original project.
            string[] additionalMtlDependencies = assetPaths
                .Where(d => d.EndsWith(".obj"))
                .Select(d => d.Replace(".obj", ".mtl"))
                .Where(d => File.Exists(d))
                .ToArray();
            assetPaths.UnionWith(additionalMtlDependencies);

            // Include all .cs scripts included in the custom C# assembly
            if (ProjectConfig.activePackageConfig is SpaceConfig spaceConfig && spaceConfig.csharpAssembly != null)
            {
                string assemblyRootDirectory = Path.GetDirectoryName(AssetDatabase.GetAssetPath(spaceConfig.csharpAssembly));
                string[] csharpScriptPaths = Directory.GetFiles(assemblyRootDirectory, "*.cs", SearchOption.AllDirectories);
                assetPaths.UnionWith(csharpScriptPaths.Select(path => path.Replace('\\', '/')));
            }

            // Include global assets that might not be referenced directly anywhere.
            assetPaths.Add(NetworkPrefabTable.ASSET_PATH);
            AddressablesUtility.AddAssetPathsForExportedPackage(assetPaths);

            // Remove all SDK package references since we already have them in the client
            assetPaths.RemoveWhere(d => d.StartsWith(PackageManagerUtility.PACKAGE_DIRECTORY_PATH));

            // Export all referenced assets from active package as a unity package
            // NOTE: Intentionally not including the ProjectConfig since that includes all packages in this project
            AssetDatabase.ExportPackage(assetPaths.ToArray(), PACKAGE_EXPORT_PATH);
        }

        public static void PackageActiveScene(string outputPath)
        {
            // Export only the active scene and its dependencies as a package
            AssetDatabase.ExportPackage(
                new string[] { EditorSceneManager.GetActiveScene().path },
                outputPath,
                ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies
            );
        }

        private static (string, Action) GetExceptionMessageAndCallback(System.Exception exc)
        {
            string additionalMessage = null;
            Action callback = null;

            if (exc is Proyecto26.RequestException requestException)
            {
                if (SpatialAPI.TryGetSingleError(requestException.Response, out SpatialAPI.ErrorResponse.Error error))
                {
                    switch (error.code)
                    {
                        case "USER_PROFILE_NOT_FOUND":
                            additionalMessage = "Unable to locate your Spatial account. Follow steps to re-authenticate in the Spatial portal.";
                            callback = MenuItems.OpenAccount;
                            break;
                        case "NOT_OWNER_OF_PACKAGE":
                            additionalMessage = "The package you are trying to upload is owned by another user. Only the user that initially published the package can update it.";
                            break;
                    }
                }

                if (string.IsNullOrEmpty(additionalMessage))
                {
                    // Fallback to traditional HTTP error codes.
                    switch (requestException.StatusCode)
                    {
                        case 401:
                            additionalMessage = "Invalid or expired SDK token. Follow steps to re-authenticate in the Spatial portal.";
                            callback = MenuItems.OpenAccount;
                            break;
                        case 400:
                        case 500:
                            additionalMessage = "Something went wrong. Contact support@spatial.io for assistance.";
                            break;
                    }
                }
            }

            string message = exc.Message;
            if (!string.IsNullOrEmpty(additionalMessage))
                message += "\n\n" + additionalMessage;

            return (message, callback);
        }

        public static void AssignBundleNamesToPackageAssets()
        {
            PackageConfig config = ProjectConfig.activePackageConfig;

            // Clear all asset bundle assets in the project
            foreach (string name in AssetDatabase.GetAllAssetBundleNames())
                AssetDatabase.RemoveAssetBundleName(name, forceRemove: true);
            AssetDatabase.Refresh();

            if (config is SpaceTemplateConfig spaceTemplateConfig)
            {
                // Assign a unique asset bundle name to each scene
                foreach (SpaceTemplateConfig.Variant variant in spaceTemplateConfig.variants)
                {
                    string variantBundleName = GetBundleNameForPackageAsset(config.packageName, variant.id);
                    EditorUtility.SetAssetBundleName(variant.scene, variantBundleName);
                }
            }
            else
            {
                string assetBundleName = GetBundleNameForPackageAsset(config.packageName);
                UnityEngine.Object firstAsset = config.assets.FirstOrDefault();
                EditorUtility.SetAssetBundleName(firstAsset, assetBundleName);
            }

            if (config is SpaceConfig spaceConfig && spaceConfig.csharpAssembly != null)
            {
                if (!File.Exists(CSScriptingEditorUtility.OUTPUT_ASSET_PATH))
                    throw new Exception($"Unable to find {CSScriptingEditorUtility.OUTPUT_ASSET_PATH}");

                string assetBundleName = GetBundleNameForPackageAsset(config.packageName, CSScriptingUtility.CSHARP_ASSEMBLY_BUNDLE_ID);
                TextAsset compiledDllAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(CSScriptingEditorUtility.OUTPUT_ASSET_PATH);
                EditorUtility.SetAssetBundleName(compiledDllAsset, assetBundleName);
            }

            UnityEditor.EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Creates a valid asset bundle name for a package
        /// BundleID params can be ignored if package does not have any additional bundles
        /// </summary>
        public static string GetBundleNameForPackageAsset(string packageName, string bundleID = "0")
        {
            // WARNING: Do not edit this without also changing GetAssetBundleIDFromFullBundleName
            return $"{GetValidBundleNameString(packageName)}_{bundleID}";
        }

        public static string GetAssetBundleIDFromFullBundleName(string bundleName)
        {
            string[] parts = bundleName.Split('_');
            return parts[parts.Length - 1]; // Last part is the bundle ID; See GetBundleNameForPackageAsset
        }

        /// <summary>
        /// Returns a string that is safe to use as an asset bundle name.
        /// </summary>
        private static string GetValidBundleNameString(string str)
        {
            string safeStr = str.Replace(" ", "-").Replace("_", "-").ToLower();
            foreach (string c in INVALID_BUNDLE_NAME_CHARS)
                safeStr = safeStr.Replace(c, "");
            return safeStr;
        }

        public static void ProcessAndSavePackageAssets()
        {
            foreach (UnityEngine.Object asset in ProjectConfig.activePackageConfig.assets)
            {
                if (asset is SpatialPackageAsset packageAsset)
                    SpatialPackageProcessor.ProcessPackageAsset(packageAsset);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void RestoreAssetBackups()
        {
            // Restore config file to previous values, if any has changed.
            EditorUtility.RestoreAssetFromBackup(ProjectConfig.activePackageConfig);
            EditorUtility.RestoreAssetFromBackup(ProjectConfig.instance);

            if (ProjectConfig.activePackageConfig is SpaceConfig spaceConfig && spaceConfig.csharpAssembly != null)
                EditorUtility.RestoreAssetFromBackup(spaceConfig.csharpAssembly);
        }

        private static bool CompileAssemblyIfExists(bool enforceName = true)
        {
            if (ProjectConfig.activePackageConfig is SpaceConfig spaceConfig && spaceConfig.csharpAssembly != null)
            {
                if (enforceName)
                {
                    // Create backup because we're going to modify the assembly name
                    EditorUtility.CreateAssetBackup(spaceConfig.csharpAssembly);
                    CSScriptingEditorUtility.EnforceCustomAssemblyName(spaceConfig.csharpAssembly, null);
                }

                return CSScriptingEditorUtility.CompileAssembly(spaceConfig.csharpAssembly, null, allowExceptions: true, enforceName);
            }

            return true;
        }

        /// <summary>
        /// Called before validation runs, at the very start of the build process.
        /// </summary>
        private static void OnBeforeBuild(bool saveOpenScenes)
        {
            // We must save all scenes, otherwise the bundle build will fail without explanation.
            // Required for sandbox and publishing space-based packages.
            if (saveOpenScenes)
            {
                EditorSceneManager.SaveOpenScenes();
            }

            // Update project settings and assign to the package config so other places can reference it during build process.
            ProjectConfig.activePackageConfig.savedProjectSettings = SaveProjectSettingsToAsset();
            EditorUtility.SaveAssetImmediately(ProjectConfig.activePackageConfig);

            foreach (UnityEngine.Object asset in ProjectConfig.activePackageConfig.assets)
            {
                // There are settings that should be enforced and automatically corrected without user intervention.
                if (asset is SpatialAvatarAttachment avatarAttachmentPrefab)
                    AvatarAttachmentComponentTests.EnforceValidSetup(avatarAttachmentPrefab, saveImmediately: false);
            }

            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Create a temporary prefab copy so that when validation and fixes are enforced.
        /// it won't make destructive changes to the original prefab. The temporary prefab copy will be uploaded instead.
        /// </summary>
        public static void ProcessAvatarPackageAssets(PackageConfig packageConfig)
        {
            const string PROCESSED_PACKAGE_PREFABS_DIRECTORY = EditorUtility.STORAGE_DIRECTORY + "/TempProcessedPackagePrefabs";

            // For every source asset, we only want to have one processed prefab copy
            // GUID->Path lookup
            const string PROCESSED_PACKAGE_PREFABS_CACHE_INFO_PATH = PROCESSED_PACKAGE_PREFABS_DIRECTORY + "/cache~";
            static Dictionary<string, string> GetCache()
            {
                Dictionary<string, string> prefabCache = new Dictionary<string, string>();
                if (File.Exists(PROCESSED_PACKAGE_PREFABS_CACHE_INFO_PATH))
                    prefabCache = File.ReadAllLines(PROCESSED_PACKAGE_PREFABS_CACHE_INFO_PATH).Select(line => line.Split('|')).ToDictionary(line => line[0], line => line[1]);
                return prefabCache;
            }
            static void SaveCache(Dictionary<string, string> prefabCache)
            {
                Directory.CreateDirectory(PROCESSED_PACKAGE_PREFABS_DIRECTORY);
                File.WriteAllLines(PROCESSED_PACKAGE_PREFABS_CACHE_INFO_PATH, prefabCache.Select(kvp => $"{kvp.Key}|{kvp.Value}"));
            }
            Dictionary<string, string> cache = GetCache();

            static T ProcessPackagePrefab<T>(Dictionary<string, string> cache, UnityEngine.Object prefabAsset) where T : SpatialPackageAsset
            {
                if (prefabAsset == null)
                    return null;

                string sourcePrefabPath = AssetDatabase.GetAssetPath(prefabAsset);
                Hash128 dependencyHash = AssetDatabase.GetAssetDependencyHash(sourcePrefabPath);
                string processedPrefabPath = Path.Combine(PROCESSED_PACKAGE_PREFABS_DIRECTORY, $"{prefabAsset.name}_{dependencyHash}.prefab");

                // Cache hit, no need to re-process
                if (File.Exists(processedPrefabPath))
                    return AssetDatabase.LoadAssetAtPath<T>(processedPrefabPath);

                // Delete the old prefab if it exists
                string sourcePrefabGUID = AssetDatabase.AssetPathToGUID(sourcePrefabPath);
                if (cache.TryGetValue(sourcePrefabGUID, out string oldPrefabPath) && File.Exists(oldPrefabPath))
                    AssetDatabase.DeleteAsset(oldPrefabPath);

                // Cache miss, process and save
                Directory.CreateDirectory(PROCESSED_PACKAGE_PREFABS_DIRECTORY);
                if (!AssetDatabase.CopyAsset(sourcePrefabPath, processedPrefabPath))
                    return null;

                cache[sourcePrefabGUID] = processedPrefabPath;
                T prefabToProcess = AssetDatabase.LoadAssetAtPath<T>(processedPrefabPath);
                ValidationUtility.ProcessPackagePrefab(prefabToProcess);
                return prefabToProcess;
            }

            if (packageConfig is AvatarConfig avatarConfig)
            {
                avatarConfig.prefab = ProcessPackagePrefab<SpatialAvatar>(cache, avatarConfig.prefab);
                UnityEditor.EditorUtility.SetDirty(avatarConfig);
            }
            else if (packageConfig is AvatarAttachmentConfig attachmentConfig)
            {
                attachmentConfig.prefab = ProcessPackagePrefab<SpatialAvatarAttachment>(cache, attachmentConfig.prefab);
                UnityEditor.EditorUtility.SetDirty(attachmentConfig);
            }
            else if (packageConfig is SpaceConfig spaceConfig && spaceConfig.embeddedPackageAssets != null && spaceConfig.embeddedPackageAssets.Length > 0)
            {
                try
                {
                    for (int i = 0; i < spaceConfig.embeddedPackageAssets.Length; i++)
                    {
                        UnityEditor.EditorUtility.DisplayProgressBar("Processing embedded package assets", $"{i} out of {spaceConfig.embeddedPackageAssets.Length} processed", (float)i / spaceConfig.embeddedPackageAssets.Length);

                        EmbeddedPackageAsset em = spaceConfig.embeddedPackageAssets[i];
                        SpatialPackageAsset spatialPackageAsset = em.asset;
                        if (em.asset == null)
                            continue;

                        if (spatialPackageAsset is SpatialAvatarAttachment)
                        {
                            em.asset = ProcessPackagePrefab<SpatialAvatarAttachment>(cache, spatialPackageAsset);
                        }
                        else if (spatialPackageAsset is SpatialAvatar)
                        {
                            em.asset = ProcessPackagePrefab<SpatialAvatar>(cache, spatialPackageAsset);
                        }
                    }
                    UnityEditor.EditorUtility.SetDirty(spaceConfig);
                }
                finally
                {
                    UnityEditor.EditorUtility.ClearProgressBar();
                }
            }

            SaveCache(cache);
        }

        public static SavedProjectSettings SaveProjectSettingsToAsset()
        {
            SavedProjectSettings projSettings = AssetDatabase.LoadAssetAtPath<SavedProjectSettings>(SAVED_PROJECT_SETTINGS_ASSET_PATH);
            bool assetExisted = projSettings != null;

            if (!assetExisted)
            {
                projSettings = ScriptableObject.CreateInstance<SavedProjectSettings>();
                projSettings.name = nameof(SavedProjectSettings);
            }

            projSettings.publishedSDKVersion = PackageManagerUtility.currentVersion;
            projSettings.customCollisionSettings = SpatialSDKPhysicsSettings.SavePhysicsSettings();
            projSettings.customCollision2DSettings = SpatialSDKPhysicsSettings.SavePhysicsSettings(get2D: true);

            if (assetExisted)
            {
                EditorUtility.SaveAssetImmediately(projSettings);
            }
            else
            {
                AssetDatabase.CreateAsset(projSettings, SAVED_PROJECT_SETTINGS_ASSET_PATH);
                // Use the asset reference rather than the runtime ScriptableObject instance that was created above.
                projSettings = AssetDatabase.LoadAssetAtPath<SavedProjectSettings>(SAVED_PROJECT_SETTINGS_ASSET_PATH);
            }

            return projSettings;
        }

        /// <summary>
        /// Returns a string explaining why sandbox and package builds are disabled. Otherwise, returns null.
        /// </summary>
        public static string GetBuildDisabledReason()
        {
            if (ProjectConfig.activePackageConfig == null)
                return "There is no active package selected. You must create or select a package in the configuration window.";
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return "Builds are disabled while in play mode";
            if (!AuthUtility.isAuthenticated)
                return "You must be logged into a Spatial account";
            if (UnityEditor.EditorUtility.scriptCompilationFailed)
                return "You must fix all script compilation errors";
            return null;
        }
    }
}