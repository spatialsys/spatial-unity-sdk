using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using RSG;
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
        public static string BUILD_DIR = "Exports";
        public static string PACKAGE_EXPORT_PATH = Path.Combine(BUILD_DIR, "spaces.unitypackage");
        public static readonly string[] INVALID_BUNDLE_NAME_CHARS = new string[] { " ", "_", ".", ",", "(", ")", "[", "]", "{", "}", "!", "@", "#", "$", "%", "^", "&", "*", "+", "=", "|", "\\", "/", "?", "<", ">", "`", "~", "'", "\"", ":", ";", "\n", "\t" };
        public static int MAX_SANDBOX_BUNDLE_SIZE = 1000 * 1024 * 1024; // 1 GB; It's higher than the package size limit because we want to allow people to mess around more in the sandbox
        public static int MAX_PACKAGE_SIZE = 500 * 1024 * 1024; // 500 MB

        private static float _lastUploadProgress;
        private static float _uploadStartTime;

        private static MethodInfo _getImportedAssetImportDependenciesAsGUIDs;
        private static MethodInfo _getSourceAssetImportDependenciesAsGUIDs;

        static BuildUtility()
        {
            // Get references to internal AssetDatabase methods
            // string[] AssetDatabase.GetSourceAssetImportDependenciesAsGUIDs(string path)
            // string[] AssetDatabase.GetImportedAssetImportDependenciesAsGUIDs(string path)
            _getImportedAssetImportDependenciesAsGUIDs = typeof(AssetDatabase).GetMethod("GetImportedAssetImportDependenciesAsGUIDs", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            _getSourceAssetImportDependenciesAsGUIDs = typeof(AssetDatabase).GetMethod("GetSourceAssetImportDependenciesAsGUIDs", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        }

        public static IPromise BuildAndUploadForSandbox(BuildTarget target = BuildTarget.WebGL)
        {
            // We must save all scenes, otherwise the bundle build will fail without explanation.
            EditorSceneManager.SaveOpenScenes();

            ProjectConfig.activePackageConfig.savedProjectSettings = SaveProjectSettingsToAsset();

            if (ProjectConfig.activePackageConfig is AvatarAttachmentConfig avatarAttachmentConfig)
                AvatarAttachmentComponentTests.EnforceValidSetup(avatarAttachmentConfig.prefab);

            IPromise<SpatialValidationSummary> validationPromise;
            if (ProjectConfig.activePackageConfig.isSpaceBasedPackage)
            {
                // Make sure the active package is set to the one that contains the active scene
                SceneAsset currentScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(EditorSceneManager.GetActiveScene().path);
                ProjectConfig.SetActivePackageBySourceAsset(currentScene);

                validationPromise = SpatialValidator.RunTestsOnActiveScene(ValidationContext.UploadingToSandbox);
            }
            else
            {
                validationPromise = SpatialValidator.RunTestsOnPackage(ValidationContext.UploadingToSandbox);
            }

            return CheckValidationPromise(validationPromise)
                .Then(() => {
                    ProcessPackageAssets();

                    // Create a new backup to the package config asset in case we modify it, so it's easy to revert any changes made to it.
                    EditorUtility.CreateAssetBackup(ProjectConfig.activePackageConfig);

                    if (ProjectConfig.activePackageConfig is AvatarConfig avatarConfig)
                    {
                        // Create a temporary prefab copy so that when validation and fixes are enforced, it won't make destructive changes to the original prefab. The temporary prefab copy will be uploaded instead.
                        GameObject newPrefab = EditorUtility.CreatePrefabCopyForTemporaryModification(avatarConfig.prefab);

                        avatarConfig.prefab = newPrefab.GetComponent<SpatialAvatar>();
                        UnityEditor.EditorUtility.SetDirty(avatarConfig);

                        AvatarPackageTests.EnforceValidBoneOrientations(avatarConfig.prefab);
                    }

                    // Auto-assign necessary bundle names
                    AssignBundleNamesToPackageAssets();

                    if (!EditorUtility.IsPlatformModuleInstalled(target))
                        return Promise.Rejected(new System.Exception($"Platform module for {target} is not installed, which is required for testing in sandbox"));

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

                    bool openBrowser = target == BuildTarget.WebGL;

                    if (ProjectConfig.activePackageConfig is SpaceTemplateConfig spaceTemplateConfig)
                    {
                        // Get the variant config for the current scene
                        SceneAsset currentSceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(EditorSceneManager.GetActiveScene().path);
                        SpaceTemplateConfig.Variant spaceTemplateVariant = spaceTemplateConfig.variants.FirstOrDefault(v => v.scene == currentSceneAsset);
                        if (spaceTemplateVariant == null)
                            return Promise.Rejected(new System.Exception("The current scene isn't one that is assigned to a variant in the package configuration"));

                        return UploadAssetBundleToSandbox(ProjectConfig.activePackageConfig, spaceTemplateVariant.bundleName, bundleDir, openBrowser);
                    }
                    else
                    {
                        return UploadAssetBundleToSandbox(ProjectConfig.activePackageConfig, ProjectConfig.activePackageConfig.bundleName, bundleDir, openBrowser);
                    }
                })
                .Finally(() => {
                    AssetDatabase.DeleteAsset(EditorUtility.TEMP_DIRECTORY);

                    // Restore config file to previous values, if any has changed.
                    EditorUtility.RestoreAssetFromBackup(ProjectConfig.activePackageConfig);
                });
        }

        private static IPromise UploadAssetBundleToSandbox(PackageConfig packageConfig, string bundleName, string bundleDir, bool openBrowser)
        {
            if (string.IsNullOrEmpty(bundleName))
                return Promise.Rejected(new System.Exception("Unable to retrieve the asset bundle name from the prefab. Make sure an asset is assigned in the package configuration."));

            string bundlePath = Path.Combine(bundleDir, bundleName);
            if (!File.Exists(bundlePath))
                return Promise.Rejected(new System.Exception($"Built asset bundle `{bundlePath}` was not found on disk"));
            long bundleSize = new FileInfo(bundlePath).Length;
            if (bundleSize > MAX_SANDBOX_BUNDLE_SIZE)
                return Promise.Rejected(new System.Exception($"Asset bundle is too large ({bundleSize / 1024 / 1024}MB). The maximum size is {MAX_SANDBOX_BUNDLE_SIZE / 1024 / 1024}MB"));

            _lastUploadProgress = -1f;
            _uploadStartTime = Time.realtimeSinceStartup;
            UpdateSandboxUploadProgressBar(0, 0, 0f);
            return SpatialAPI.UploadSandboxBundle(packageConfig.sku, packageConfig.packageType)
                .Then(resp => {
                    byte[] bundleBytes = File.ReadAllBytes(bundlePath);
                    SpatialAPI.UploadFile(useSpatialHeaders: false, resp.url, bundleBytes, UpdateSandboxUploadProgressBar)
                        .Then(resp => {
                            if (openBrowser)
                            {
                                EditorUtility.OpenSandboxInBrowser();
                            }
                            else
                            {
                                UnityEditor.EditorUtility.DisplayDialog("Upload complete", "Sandbox content uploaded.", "OK");
                            }
                        })
                        .Catch(exc => {
                            (string exceptionMessage, Action callback) = GetExceptionMessageAndCallback(exc);
                            if (UnityEditor.EditorUtility.DisplayDialog("Asset bundle upload error", exceptionMessage, "OK"))
                                callback?.Invoke();
                            Debug.LogError(exc);
                        })
                        .Finally(() => UnityEditor.EditorUtility.ClearProgressBar());
                })
                .Catch(exc => {
                    UnityEditor.EditorUtility.ClearProgressBar();

                    (string exceptionMessage, Action callback) = GetExceptionMessageAndCallback(exc);
                    if (UnityEditor.EditorUtility.DisplayDialog("Upload failed", exceptionMessage, "OK"))
                        callback?.Invoke();
                });
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

        private static void UpdateSandboxUploadProgressBar(long transferred, long total, float progress)
        {
            UpdateUploadProgressBar("Uploading sandbox asset bundle", transferred, total, progress);
        }

        public static IPromise PackageForPublishing()
        {
            // We must save all scenes, otherwise the bundle build will fail without explanation.
            EditorSceneManager.SaveOpenScenes();
            ProjectConfig.activePackageConfig.savedProjectSettings = SaveProjectSettingsToAsset();

            if (ProjectConfig.activePackageConfig is AvatarAttachmentConfig avatarAttachmentConfig)
                AvatarAttachmentComponentTests.EnforceValidSetup(avatarAttachmentConfig.prefab);

            return CheckValidationPromise(SpatialValidator.RunTestsOnPackage(ValidationContext.PublishingPackage))
                .Then(() => {
                    // Always save all assets before publishing so that the uploaded package has the latest changes
                    AssetDatabase.SaveAssets();

                    ProcessPackageAssets();

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
                                UnityEditor.EditorUtility.SetDirty(spaceConfig);
                                AssetDatabase.SaveAssetIfDirty(spaceConfig);
                            });
                    }

                    // Upload package
                    _lastUploadProgress = -1f;
                    UpdatePackageUploadProgressBar(0, 0, 0f);
                    return createWorldPromise;
                }).Then(() => {
                    return SpatialAPI.CreateOrUpdatePackage(ProjectConfig.activePackageConfig.sku, ProjectConfig.activePackageConfig.packageType);
                })
                .Then(resp => {
                    PackageConfig config = ProjectConfig.activePackageConfig;
                    if (config.sku != resp.sku)
                    {
                        config.sku = resp.sku;
                        UnityEditor.EditorUtility.SetDirty(config);
                        AssetDatabase.SaveAssetIfDirty(config);
                    }

                    // Create a new backup to the package config asset in case we modify it, so it's easy to revert any changes made to it.
                    EditorUtility.CreateAssetBackup(ProjectConfig.activePackageConfig);

                    if (ProjectConfig.activePackageConfig is AvatarConfig avatarConfig)
                    {
                        // Create a temporary prefab copy so that when validation and fixes are enforced, it won't make destructive changes to the original prefab. The temporary prefab copy will be uploaded instead.
                        GameObject newPrefab = EditorUtility.CreatePrefabCopyForTemporaryModification(avatarConfig.prefab);

                        avatarConfig.prefab = newPrefab.GetComponent<SpatialAvatar>();
                        UnityEditor.EditorUtility.SetDirty(avatarConfig);

                        AvatarPackageTests.EnforceValidBoneOrientations(avatarConfig.prefab);
                    }

                    // Package it after SKU is assigned or else the config inside the package will not have the SKU
                    // Export all scenes and dependencies as a package
                    if (!Directory.Exists(BUILD_DIR))
                        Directory.CreateDirectory(BUILD_DIR);
                    PackageProject(PACKAGE_EXPORT_PATH);

                    _uploadStartTime = Time.realtimeSinceStartup;
                    byte[] packageBytes = File.ReadAllBytes(PACKAGE_EXPORT_PATH);
                    SpatialAPI.UploadPackage(resp.sku, resp.version, packageBytes, UpdatePackageUploadProgressBar)
                        .Then(resp => {
                            UnityEditor.EditorUtility.DisplayDialog(
                                "Upload complete!",
                                "Your package was successfully uploaded and we've started processing it.\n\n" +
                                "This may take as little as 15 minutes to process, depending on our backlog and your package complexity. If any unexpected errors occur, we aim to address the issue within 24 hours.\n\n" +
                                "You will receive an email notification once the package has been published.",
                                "OK"
                            );
                        })
                        .Catch(exc => {
                            (string exceptionMessage, Action callback) = GetExceptionMessageAndCallback(exc);
                            if (UnityEditor.EditorUtility.DisplayDialog("Package upload network error", exceptionMessage, "OK"))
                                callback?.Invoke();
                        })
                        .Finally(() => UnityEditor.EditorUtility.ClearProgressBar());
                })
                .Catch(exc => {
                    UnityEditor.EditorUtility.ClearProgressBar();
                    (string bodyMessage, Action onCloseDialog) = GetExceptionMessageAndCallback(exc);

                    if (exc is Proyecto26.RequestException ex && SpatialAPI.TryGetSingleError(ex.Response, out SpatialAPI.ErrorResponse.Error error))
                    {
                        switch (error.code)
                        {
                            case "USER_PROFILE_NOT_FOUND":
                                bodyMessage = $"Unable to locate your Spatial account. Follow steps to re-authenticate in the Spatial portal.";
                                onCloseDialog = MenuItems.OpenAccount;
                                break;
                            case "NOT_OWNER_OF_PACKAGE":
                                bodyMessage = "The package you are trying to upload is owned by another user. Only the user that initially published the package can update it.";
                                break;
                            default:
                                bodyMessage = $"Publishing failed with the following error: {ex.Response}";
                                break;
                        }
                    }

                    Debug.LogException(exc);
                    if (UnityEditor.EditorUtility.DisplayDialog("Publishing failed", bodyMessage, "OK"))
                    {
                        onCloseDialog?.Invoke();
                    }
                })
                .Finally(() => {
                    AssetDatabase.DeleteAsset(EditorUtility.TEMP_DIRECTORY);

                    // Restore config file to previous values if necessary.
                    EditorUtility.RestoreAssetFromBackup(ProjectConfig.activePackageConfig);
                });
        }

        public static void PackageProject(string outputPath)
        {
            // TODO: we can also exclude dependencies from packages that are included in the builder project by default
            HashSet<string> dependencies = AssetDatabase.GetDependencies(AssetDatabase.GetAssetPath(ProjectConfig.activePackageConfig)).ToHashSet();

            // GetDependencies doesn't include all "import time" dependencies, so we need to manually add them
            // See: https://forum.unity.com/threads/discrepancy-between-assetdatabase-getdependencies-and-results-of-exportpackage.1295025/
            foreach (string dep in dependencies.ToArray())
            {
                string[] importTimeDependencies = _getImportedAssetImportDependenciesAsGUIDs.Invoke(null, new object[] { dep }) as string[];
                if (importTimeDependencies.Length > 0)
                    dependencies.UnionWith(importTimeDependencies.Select(d => AssetDatabase.GUIDToAssetPath(d)));

                string[] sourceAssetImportTimeDependencies = _getSourceAssetImportDependenciesAsGUIDs.Invoke(null, new object[] { dep }) as string[];
                if (sourceAssetImportTimeDependencies.Length > 0)
                    dependencies.UnionWith(sourceAssetImportTimeDependencies.Select(d => AssetDatabase.GUIDToAssetPath(d)));
            }

            // For `.obj` files in dependencies, we need to also manually include the `.mtl` file as a dependency
            // or else when the package extracted into the builder project, the materials will not be the same
            // as they were in the original project.
            string[] additionalMtlDependencies = dependencies
                .Where(d => d.EndsWith(".obj"))
                .Select(d => d.Replace(".obj", ".mtl"))
                .Where(d => File.Exists(d))
                .ToArray();
            dependencies.UnionWith(additionalMtlDependencies);

            // Remove all unitysdk package references
            dependencies.RemoveWhere(d => d.StartsWith("Packages/io.spatial.unitysdk")); // we already have these

            // Export all referenced assets from active package as a unity package
            // NOTE: Intentionally not including the ProjectConfig since that includes all packages in this project
            AssetDatabase.ExportPackage(dependencies.ToArray(), outputPath);
        }

        public static void PackageActiveScene(string outputPath)
        {
            // Export only the active scene and its dependencies as a package
            AssetDatabase.ExportPackage(
                new string[] { UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path },
                outputPath,
                ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies
            );
        }

        private static void UpdatePackageUploadProgressBar(long transferred, long total, float progress)
        {
            UpdateUploadProgressBar("Uploading package for publishing", transferred, total, progress);
        }

        private static void UpdateUploadProgressBar(string title, long transferred, long total, float progress)
        {
            if (progress <= _lastUploadProgress)
                return;

            _lastUploadProgress = progress;

            long transferredKb = Mathf.FloorToInt(transferred / 1024f);
            long totalKb = Mathf.FloorToInt(total / 1024f);
            int secondsElapsed = Mathf.FloorToInt(Time.realtimeSinceStartup - _uploadStartTime);
            string message = (progress > 0f) ? $"Uploading {EditorUtility.FormatNumber(transferredKb)}kb / {EditorUtility.FormatNumber(totalKb)}kb ({Mathf.FloorToInt(progress * 100f)}%) ({secondsElapsed}sec)" : "Please wait...";
            UnityEditor.EditorUtility.DisplayProgressBar(title, message, progress);
        }

        private static (string, Action) GetExceptionMessageAndCallback(System.Exception exc)
        {
            string additionalMessage = null;
            Action callback = null;

            if (exc is Proyecto26.RequestException requestException)
            {
                switch (requestException.StatusCode)
                {
                    case 401:
                        additionalMessage = $"Invalid or expired SDK token. Follow steps to re-authenticate in the Spatial portal.";
                        callback = MenuItems.OpenAccount;
                        break;
                    case 400:
                    case 500:
                        additionalMessage = "Something went wrong. Contact support@spatial.io for assistance.";
                        break;
                }
            }

            string result = exc.Message;
            if (!string.IsNullOrEmpty(additionalMessage))
                result += "\n\n" + additionalMessage;

            return (result, callback);
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
                    string variantBundleName = GetBundleNameForPackageAsset(config.packageName, variant.name, variant.id);
                    EditorUtility.SetAssetBundleName(variant.scene, variantBundleName);
                }
            }
            else
            {
                // Multiple variants/assets are not supported for package types that aren't SpaceTemplate.
                string assetBundleName = GetBundleNameForPackageAsset(config.packageName);
                UnityEngine.Object firstAsset = config.assets.FirstOrDefault();
                EditorUtility.SetAssetBundleName(firstAsset, assetBundleName);
            }

            UnityEditor.EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Creates a valid asset bundle name for a package
        /// Variant params can be ignored if package does not have any variants
        /// </summary>
        public static string GetBundleNameForPackageAsset(string packageName, string variantName = "default", string variantID = "0")
        {
            // WARNING: Do not edit this without also changing GetVariantIDFromBundleName
            return $"{GetValidBundleNameString(packageName)}_{GetValidBundleNameString(variantName)}_{variantID}";
        }

        public static string GetVariantIDFromBundleName(string bundleName)
        {
            string[] parts = bundleName.Split('_');
            return parts[parts.Length - 1]; // Last part is the variant ID; See GetBundleNameForPackageAsset
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

        public static void ProcessPackageAssets()
        {
            PackageConfig config = ProjectConfig.activePackageConfig;

            foreach (UnityEngine.Object asset in config.assets)
            {
                if (asset is SpatialPackageAsset packageAsset)
                    SpatialPackageProcessor.ProcessPackageAsset(packageAsset);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }

        public static SavedProjectSettings SaveProjectSettingsToAsset()
        {
            SavedProjectSettings projSettings = ScriptableObject.CreateInstance<SavedProjectSettings>();
            projSettings.publishedSDKVersion = PackageManagerUtility.currentVersion;
            projSettings.customCollisionSettings = SpatialSDKPhysicsSettings.SavePhysicsSettings();
            projSettings.customCollision2DSettings = SpatialSDKPhysicsSettings.SavePhysicsSettings(get2D: true);
            projSettings.name = "SavedProjectSettings";

            AssetDatabase.CreateAsset(projSettings, SAVED_PROJECT_SETTINGS_ASSET_PATH);
            AssetDatabase.ImportAsset(SAVED_PROJECT_SETTINGS_ASSET_PATH, ImportAssetOptions.ForceUpdate);

            return projSettings;
        }
    }
}