using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RSG;
using UnityEditor;
using UnityEditor.Build.Pipeline;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Build.Pipeline;

namespace SpatialSys.UnitySDK.Editor
{
    public class BuildUtility
    {
        public static string BUILD_DIR = "Exports";
        public static string PACKAGE_EXPORT_PATH = Path.Combine(BUILD_DIR, "spaces.unitypackage");
        public static readonly string[] INVALID_BUNDLE_NAME_CHARS = new string[] { " ", "_", ".", ",", "(", ")", "[", "]", "{", "}", "!", "@", "#", "$", "%", "^", "&", "*", "+", "=", "|", "\\", "/", "?", "<", ">", "`", "~", "'", "\"", ":", ";" };
        public static int MAX_SANDBOX_BUNDLE_SIZE = 100 * 1024 * 1024; // 100 MB
        public static int MAX_PACKAGE_SIZE = 500 * 1024 * 1024; // 500 MB

        private static float _lastUploadProgress;

        public static IPromise BuildAndUploadForSandbox()
        {
            // We must save all scenes, otherwise the bundle build will fail without explanation.
            EditorSceneManager.SaveOpenScenes();

            PackageConfig activeConfig = ProjectConfig.activePackage;
            bool validationHasErrors = false;

            if (activeConfig is EnvironmentConfig)
            {
                // Make sure the active package is set to the one that contains the active scene
                SceneAsset currentScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(EditorSceneManager.GetActiveScene().path);
                ProjectConfig.SetActivePackageBySourceAsset(currentScene);

                validationHasErrors = !SpatialValidator.RunTestsOnActiveScene(ValidationContext.Testing);
            }
            else
            {
                validationHasErrors = !SpatialValidator.RunTestsOnPackage(ValidationContext.Testing);
            }

            if (validationHasErrors)
            {
                SpatialSDKConfigWindow.OpenWindow("issues");
                return Promise.Rejected(new Exception("Package has errors"));
            }

            // Auto-assign necessary bundle names
            AssignBundleNamesToPackageAssets();

            // TODO: Ensure WebGL bundle is installed.
            const BuildTarget TARGET = BuildTarget.WebGL;
            string bundleDir = Path.Combine(BUILD_DIR, TARGET.ToString());
            if (Directory.Exists(bundleDir))
                Directory.Delete(bundleDir, recursive: true);
            Directory.CreateDirectory(bundleDir);

            // Compress bundles with LZ4 (ChunkBasedCompression) since web doesn't support LZMA.
            CompatibilityAssetBundleManifest bundleManifest = CompatibilityBuildPipeline.BuildAssetBundles(
                bundleDir,
                BuildAssetBundleOptions.ForceRebuildAssetBundle | BuildAssetBundleOptions.ChunkBasedCompression,
                TARGET
            );

            if (bundleManifest == null)
                return Promise.Rejected(new System.Exception("Something went wrong when trying to build asset bundle for sandbox"));

            if (activeConfig is EnvironmentConfig activeEnvConfig)
            {
                // Get the variant config for the current scene
                SceneAsset currentSceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(EditorSceneManager.GetActiveScene().path);
                EnvironmentConfig.Variant environmentVariant = activeEnvConfig.variants.FirstOrDefault(v => v.scene == currentSceneAsset);
                if (environmentVariant == null)
                    return Promise.Rejected(new System.Exception("The current scene isn't one that is assigned to a variant in the package configuration"));

                return UploadAssetBundleToSandbox(environmentVariant.bundleName, bundleDir, activeConfig.packageType);
            }
            else
            {
                return UploadAssetBundleToSandbox(activeConfig.bundleName, bundleDir, activeConfig.packageType);
            }
        }

        private static IPromise UploadAssetBundleToSandbox(string bundleName, string bundleDir, PackageType packageType)
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
            UpdateSandboxUploadProgressBar(0f);
            return SpatialAPI.UploadSandboxBundle((SpatialAPI.PackageType)(int)packageType)
                .Then(resp => {
                    byte[] data = File.ReadAllBytes(bundlePath);
                    SpatialAPI.UploadFile(useSpatialHeaders: false, resp.url, data, UpdateSandboxUploadProgressBar)
                        .Then(resp => EditorUtility.OpenSandboxInBrowser())
                        .Catch(exc => UnityEditor.EditorUtility.DisplayDialog("Asset bundle upload network error", GetExceptionMessage(exc), "OK"))
                        .Finally(() => UnityEditor.EditorUtility.ClearProgressBar());
                })
                .Catch(exc => {
                    UnityEditor.EditorUtility.ClearProgressBar();
                    UnityEditor.EditorUtility.DisplayDialog("Upload failed", GetExceptionMessage(exc), "OK");
                });
        }

        private static void UpdateSandboxUploadProgressBar(float progress)
        {
            UpdateUploadProgressBar("Uploading sandbox asset bundle", progress);
        }

        public static IPromise PackageForPublishing()
        {
            // We must save all scenes, otherwise the bundle build will fail without explanation.
            EditorSceneManager.SaveOpenScenes();

            // Validate package
            if (!SpatialValidator.RunTestsOnPackage(ValidationContext.Publishing))
            {
                SpatialSDKConfigWindow.OpenWindow("issues");
                return Promise.Rejected(new Exception("Package has errors"));
            }

            // Always save all assets before publishing so that the uploaded package has the latest changes
            AssetDatabase.SaveAssets();

            // Auto-assign necessary bundle names
            // This get's done on the build machines too, but we also want to do it here just in case there's an issue
            AssignBundleNamesToPackageAssets();

            // Upload package
            _lastUploadProgress = -1f;
            UpdatePackageUploadProgressBar(0f);
            PackageConfig config = ProjectConfig.activePackage;
            return SpatialAPI.CreateOrUpdatePackage(config.sku, (SpatialAPI.PackageType)(int)config.packageType)
                .Then(resp => {
                    if (config.sku != resp.sku)
                    {
                        config.sku = resp.sku;
                        UnityEditor.EditorUtility.SetDirty(config);
                        AssetDatabase.SaveAssetIfDirty(config);
                    }

                    // Package it after SKU is assigned or else the config inside the package will not have the SKU
                    // Export all scenes and dependencies as a package
                    if (!Directory.Exists(BUILD_DIR))
                        Directory.CreateDirectory(BUILD_DIR);
                    PackageProject(PACKAGE_EXPORT_PATH);

                    byte[] packageData = File.ReadAllBytes(PACKAGE_EXPORT_PATH);
                    SpatialAPI.UploadPackage(resp.sku, resp.version, packageData, UpdatePackageUploadProgressBar)
                        .Then(resp => {
                            UnityEditor.EditorUtility.DisplayDialog(
                                "Upload complete!",
                                "Your package was successfully uploaded and we've started processing it.\n\n" +
                                "This may take as little as 15 minutes to process, depending on our backlog. If any unexpected errors occur, we aim to address the issue within 24 hours.\n\n" +
                                "You will receive an email notification once the package has been published.",
                                "OK"
                            );
                        })
                        .Catch(exc => UnityEditor.EditorUtility.DisplayDialog("Package upload network error", GetExceptionMessage(exc), "OK"))
                        .Finally(() => UnityEditor.EditorUtility.ClearProgressBar());
                })
                .Catch(exc => {
                    UnityEditor.EditorUtility.ClearProgressBar();

                    if (exc is Proyecto26.RequestException ex)
                    {
                        if (SpatialAPI.TryGetSingleError(ex.Response, out SpatialAPI.ErrorResponse.Error error))
                        {
                            switch (error.code)
                            {
                                case "NOT_OWNER_OF_PACKAGE":
                                    UnityEditor.EditorUtility.DisplayDialog(
                                        "Publishing failed",
                                        "The package you are trying to upload has an SKU that is owned by another user. Only the user that initially published the package can update it.",
                                        "OK"
                                    );
                                    return;

                                case "USER_NOT_WHITELISTED":
                                    UnityEditor.EditorUtility.DisplayDialog(
                                        "Apply to be a Spatial Publisher",
                                        "You currently do not have the ability to publish to Spatial. While we are still in beta, we are only allowing a limited number of users to publish packages.\n\nTo become a publisher, fill out the form that will open in your browser after your close this dialog.\n\nMeanwhile, you can still continue to test your environments in the Spatial Sandbox.",
                                        "OK"
                                    );
                                    Application.OpenURL("https://spatialxr.typeform.com/to/e8XPgO5p");
                                    return;

                                default:
                                    UnityEditor.EditorUtility.DisplayDialog(
                                        "Publishing failed",
                                        $"Publishing failed with the following error: {ex.Response}",
                                        "OK"
                                    );
                                    return;
                            }
                        }
                    }

                    UnityEditor.EditorUtility.DisplayDialog("Publishing failed", GetExceptionMessage(exc), "OK");
                });
        }

        public static void PackageProject(string outputPath)
        {
            // TODO: we can also exclude dependencies from packages that are included in the builder project by default
            List<string> dependencies = AssetDatabase.GetDependencies(AssetDatabase.GetAssetPath(ProjectConfig.activePackage))
                .Where(d => !d.StartsWith("Packages/io.spatial.unitysdk")) // we already have these
                .ToList();

            // For `.obj` files in dependencies, we need to also manually include the `.mtl` file as a dependency
            // or else when the package extracted into the builder project, the materials will not be the same
            // as they were in the original project.
            string[] additionalMtlDependencies = dependencies
                .Where(d => d.EndsWith(".obj"))
                .Select(d => d.Replace(".obj", ".mtl"))
                .Where(d => File.Exists(d))
                .ToArray();
            dependencies.AddRange(additionalMtlDependencies);

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

        private static void UpdatePackageUploadProgressBar(float progress)
        {
            UpdateUploadProgressBar("Uploading package for publishing", progress);
        }

        private static void UpdateUploadProgressBar(string title, float progress)
        {
            if (progress <= _lastUploadProgress)
                return;

            _lastUploadProgress = progress;

            // For some reason, web request progress goes from 0 to 0.5 while uploading, then jumps to 1 at the end. Remap the value to 0 to 1.
            float remappedProgress = Mathf.Clamp01(progress * 2f);
            string message = (remappedProgress > 0f) ? $"Uploading... ({Mathf.FloorToInt(remappedProgress * 100f)}%)" : "Please wait...";
            UnityEditor.EditorUtility.DisplayProgressBar(title, message, remappedProgress);
        }

        private static string GetExceptionMessage(System.Exception exc)
        {
            string additionalMessage = null;

            if (exc is Proyecto26.RequestException requestException)
            {
                switch (requestException.StatusCode)
                {
                    case 401:
                        additionalMessage = "Invalid or expired SDK token. Follow steps to re-authenticate by opening \"Spatial SDK/Authentication\" at the top.";
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

            return result;
        }

        public static void AssignBundleNamesToPackageAssets()
        {
            PackageConfig config = ProjectConfig.activePackage;

            // Clear all asset bundle assets in the project
            foreach (string name in AssetDatabase.GetAllAssetBundleNames())
                AssetDatabase.RemoveAssetBundleName(name, forceRemove: true);
            AssetDatabase.Refresh();

            if (config is EnvironmentConfig envConfig)
            {
                // Assign a unique asset bundle name to each scene
                foreach (EnvironmentConfig.Variant variant in envConfig.variants)
                {
                    AssignBundleNameToAsset(variant.scene, config.packageName, variant.name, variant.id);
                }
            }
            else if (config is AvatarConfig avatarConfig)
            {
                AssignBundleNameToAsset(avatarConfig.prefab, config.packageName);
            }
            else if (config is AvatarAnimationConfig avatarAnimConfig)
            {
                AssignBundleNameToAsset(avatarAnimConfig.prefab, config.packageName);
            }
            else if (config is PrefabObjectConfig prefabObjectConfig)
            {
                AssignBundleNameToAsset(prefabObjectConfig.prefab, config.packageName);
            }

            UnityEditor.EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void AssignBundleNameToAsset(UnityEngine.Object asset, string packageName, string variantName = "default", string variantID = "0")
        {
            string assetPath = AssetDatabase.GetAssetPath(asset);
            AssetImporter importer = AssetImporter.GetAtPath(assetPath);
            importer.assetBundleName = GetBundleNameForPackageAsset(packageName);
            AssetDatabase.SaveAssetIfDirty(importer);
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
    }
}