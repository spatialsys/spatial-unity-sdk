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
using UnityEngine.SceneManagement;

namespace SpatialSys.UnitySDK.Editor
{
    public class BuildUtility
    {
        public static string BUILD_DIR = "Exports";
        public static string PACKAGE_EXPORT_PATH = Path.Combine(BUILD_DIR, "spaces.unitypackage");

        private static float _lastUploadProgress;

        public static IPromise BuildAndUploadForSandbox()
        {
            // Validate package
            if (!Validation.Validate(out Exception error))
                return Promise.Rejected(error);

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

            _lastUploadProgress = -1f;
            UpdateSandboxUploadProgressBar(0f);

            return SpatialAPI.UploadTestEnvironment()
                .Then(resp => {
                    string bundlePath = Path.Combine(bundleDir, environmentVariant.bundleName);
                    if (!File.Exists(bundlePath))
                        throw new System.Exception("Built asset bundle was not found on disk");

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
            if (!Validation.Validate(out Exception error))
                return Promise.Rejected(new Exception("Package validation failed", error));

            if (!Directory.Exists(BUILD_DIR))
                Directory.CreateDirectory(BUILD_DIR);

            // Export all scenes and dependencies as a package
            PackageConfig config = PackageConfig.instance;
            List<string> sourceAssets = new List<string>();
            if (config.packageType == PackageConfig.PackageType.Environment)
                sourceAssets.AddRange(config.environment.variants.Select(v => AssetDatabase.GetAssetPath(v.scene)));
            sourceAssets.Add(AssetDatabase.GetAssetPath(config));
            AssetDatabase.ExportPackage(
                sourceAssets.ToArray(),
                PACKAGE_EXPORT_PATH,
                ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies
            );

            _lastUploadProgress = -1f;
            UpdatePackageUploadProgressBar(0f);

            // TODO: support multiple package types.
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
                        .Catch(exc => UnityEditor.EditorUtility.DisplayDialog("Package upload network error", GetExceptionMessage(exc), "OK"))
                        .Finally(() => {
                            UnityEditor.EditorUtility.ClearProgressBar();
                            UnityEditor.EditorUtility.DisplayDialog(
                                "Upload complete!",
                                "Your package was successfully uploaded and we've started processing it.\n\n" +
                                "This may take anywhere from 1 to 24 hours. Once the package is processed, you will " +
                                "be able to select it from the 'Create Space' and 'Environment' menus in Spatial.",
                                "OK"
                            );
                        });
                })
                .Catch(exc => {
                    UnityEditor.EditorUtility.ClearProgressBar();
                    UnityEditor.EditorUtility.DisplayDialog("Upload failed", GetExceptionMessage(exc), "OK");
                });
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
    }
}