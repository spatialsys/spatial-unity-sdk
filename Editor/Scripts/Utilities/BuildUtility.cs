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
        public static int MAX_SANDBOX_BUNDLE_SIZE = 100 * 1024 * 1024; // 100 MB
        public static int MAX_PACKAGE_SIZE = 500 * 1024 * 1024; // 500 MB

        private static float _lastUploadProgress;

        public static IPromise BuildAndUploadForSandbox()
        {
            // Validate just the active scene
            if (!SpatialValidator.RunTestsOnActiveScene(ValidationContext.Testing))
            {
                SpatialSDKConfigWindow.OpenWindow("issues");
                return Promise.Rejected(new Exception("Package has errors"));
            }

            // Auto-assign necessary bundle names
            AssignBundleNamesToEachVariant();

            // TODO: Ensure WebGL bundle is installed.
            const BuildTarget TARGET = BuildTarget.WebGL;
            string bundleDir = Path.Combine(BUILD_DIR, TARGET.ToString());
            if (Directory.Exists(bundleDir))
                Directory.Delete(bundleDir, recursive: true);
            Directory.CreateDirectory(bundleDir);

            // Compress bundles with LZ4 (ChunkBasedCompression) since web doesn't support LZMA (why isn't this automatic based on build target??)
            CompatibilityAssetBundleManifest bundleManifest = CompatibilityBuildPipeline.BuildAssetBundles(
                bundleDir,
                BuildAssetBundleOptions.ForceRebuildAssetBundle | BuildAssetBundleOptions.ChunkBasedCompression,
                TARGET
            );

            if (bundleManifest == null)
                return Promise.Rejected(new System.Exception("Failed to build asset bundle for sandbox. Check the console for details."));

            // Get the variant config for the current scene
            PackageConfig config = PackageConfig.instance;
            SceneAsset currentScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(EditorSceneManager.GetActiveScene().path);
            PackageConfig.Environment.Variant environmentVariant = config.environment.variants.FirstOrDefault(v => v.scene == currentScene);
            if (environmentVariant == null)
                return Promise.Rejected(new System.Exception("The current scene isn't one that is assigned to a variant in the package configuration"));

            // Check for bundle size
            string bundlePath = Path.Combine(bundleDir, environmentVariant.bundleName);
            if (!File.Exists(bundlePath))
                return Promise.Rejected(new System.Exception("Built asset bundle was not found on disk"));
            long bundleSize = new FileInfo(bundlePath).Length;
            if (bundleSize > MAX_SANDBOX_BUNDLE_SIZE)
                return Promise.Rejected(new System.Exception($"Asset bundle is too large ({bundleSize / 1024 / 1024}MB). The maximum size is {MAX_SANDBOX_BUNDLE_SIZE / 1024 / 1024}MB"));

            // Upload
            _lastUploadProgress = -1f;
            UpdateSandboxUploadProgressBar(0f);
            return SpatialAPI.UploadTestEnvironment()
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
            // Validate package
            if (!SpatialValidator.RunTestsOnProject(ValidationContext.Publishing))
            {
                SpatialSDKConfigWindow.OpenWindow("issues");
                return Promise.Rejected(new Exception("Package has errors"));
            }

            // Export all scenes and dependencies as a package
            if (!Directory.Exists(BUILD_DIR))
                Directory.CreateDirectory(BUILD_DIR);
            PackageProject(PACKAGE_EXPORT_PATH);

            // Upload package
            // TODO: support multiple package types.
            _lastUploadProgress = -1f;
            UpdatePackageUploadProgressBar(0f);
            PackageConfig config = PackageConfig.instance;
            return SpatialAPI.CreateOrUpdatePackage(config.sku, SpatialAPI.PackageType.Environment)
                .Then(resp => {
                    if (config.sku != resp.sku)
                    {
                        config.sku = resp.sku;
                        UnityEditor.EditorUtility.SetDirty(config);
                        AssetDatabase.SaveAssetIfDirty(config);
                    }

                    if (!File.Exists(PACKAGE_EXPORT_PATH))
                        throw new System.Exception("Built package was not found on disk");

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
                        if (ex.StatusCode == 403) // Forbidden
                        {
                            UnityEditor.EditorUtility.DisplayDialog(
                                "Upload failed",
                                "The package you are trying to upload has an SKU that is owned by another user. Only the user that initially published the package can update it.",
                                "OK"
                            );
                            return;
                        }
                    }

                    UnityEditor.EditorUtility.DisplayDialog("Upload failed", GetExceptionMessage(exc), "OK");
                });
        }

        public static void PackageProject(string outputPath)
        {
            // Export all scenes and dependencies as a package
            PackageConfig config = PackageConfig.instance;
            List<string> sourceAssets = new List<string>();
            if (config.packageType == PackageConfig.PackageType.Environment)
                sourceAssets.AddRange(config.environment.variants.Select(v => AssetDatabase.GetAssetPath(v.scene)));
            sourceAssets.Add(AssetDatabase.GetAssetPath(config));
            AssetDatabase.ExportPackage(
                sourceAssets.ToArray(),
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

        public static void AssignBundleNamesToEachVariant()
        {
            PackageConfig config = PackageConfig.instance;

            // Clear all asset bundle assets in the project
            foreach (string name in AssetDatabase.GetAllAssetBundleNames())
                AssetDatabase.RemoveAssetBundleName(name, forceRemove: true);
            AssetDatabase.Refresh();

            // Assign a unique asset bundle name to each scene
            foreach (PackageConfig.Environment.Variant variant in config.environment.variants)
            {
                string scenePath = AssetDatabase.GetAssetPath(variant.scene);
                AssetImporter importer = AssetImporter.GetAtPath(scenePath);
                importer.assetBundleName = variant.bundleName;
                AssetDatabase.SaveAssetIfDirty(importer);
            }
        }
    }
}