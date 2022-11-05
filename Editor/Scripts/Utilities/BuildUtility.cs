using UnityEngine;
using UnityEngine.Build.Pipeline;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.Build.Pipeline;
using UnityEditor.SceneManagement;

using System.Collections.Generic;
using System.IO;
using RSG;

namespace SpatialSys.UnitySDK.Editor
{
    public class BuildUtility
    {
        public static string BUILD_DIR = "Exports";
        public static string PACKAGE_EXPORT_PATH = Path.Combine(BUILD_DIR, "spaces.unitypackage");

        private static float _lastUploadProgress;

        public static IPromise BuildAndUploadForSandbox()
        {
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

            string[] bundleNames = bundleManifest.GetAllAssetBundles();
            if (bundleNames == null || bundleNames.Length == 0)
                return Promise.Rejected(new System.Exception("There are no asset bundles in this project"));

            var bundleNamesSet = new HashSet<string>(bundleManifest.GetAllAssetBundles());

            // Only upload the bundle for the currently opened scene. If there are multiple open scenes, it will choose the first one encountered.
            string bundleNameToUpload = GetAssetBundleNameForOpenedScene();
            if (string.IsNullOrEmpty(bundleNameToUpload))
                return Promise.Rejected(new System.Exception("None of the open scenes are tagged as an asset bundle"));

            // This shouldn't really happen since we build all bundles, but check anyway.
            if (!bundleNamesSet.Contains(bundleNameToUpload))
                return Promise.Rejected(new System.Exception("Asset bundle for the target scene was not built"));

            _lastUploadProgress = -1f;
            UpdateSandboxUploadProgressBar(0f);

            return SpatialAPI.UploadTestEnvironment()
                .Then(resp => {
                    string bundlePath = Path.Combine(bundleDir, bundleNameToUpload);
                    if (!File.Exists(bundlePath))
                        throw new System.Exception("Built asset bundle was not found on disk");

                    byte[] data = File.ReadAllBytes(bundlePath);
                    SpatialAPI.UploadFile(resp.url, data, UpdateSandboxUploadProgressBar)
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
            if (progress <= _lastUploadProgress)
                return;

            _lastUploadProgress = progress;

            // For some reason, web request progress goes from 0 to 0.5 while uploading, then jumps to 1 at the end. Remap the value to 0 to 1.
            float remappedProgress = Mathf.Clamp01(progress * 2f);
            string message = (remappedProgress > 0f) ? $"Uploading... ({Mathf.FloorToInt(remappedProgress * 100f)}%)" : "Please wait...";
            UnityEditor.EditorUtility.DisplayProgressBar("Uploading sandbox asset bundle", message, remappedProgress);
        }

        public static IPromise PackageForPublishing()
        {
            string[] scenePaths = BuildUtility.GetAllAssetBundleScenePaths();

            // Export all scenes and dependencies as a package
            AssetDatabase.ExportPackage(
                scenePaths,
                PACKAGE_EXPORT_PATH,
                ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies
            );

            // TODO: Upload package to SAPI

            return Promise.Resolved();
        }

        private static string[] GetAllAssetBundleScenePaths()
        {
            List<string> scenePaths = new List<string>();
            string[] assetBundleNames = AssetDatabase.GetAllAssetBundleNames();
            for (int i = 0; i < assetBundleNames.Length; i++)
            {
                string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleNames[i]);
                if (assetPaths.Length > 0 && assetPaths[0].EndsWith(".unity"))
                    scenePaths.Add(assetPaths[0]);
            }

            return scenePaths.ToArray();
        }

        public static string GetAssetBundleNameForOpenedScene()
        {
            for (int i = 0; i < EditorSceneManager.sceneCount; i++)
            {
                Scene scene = EditorSceneManager.GetSceneAt(i);

                if (scene.IsValid() && scene.isLoaded)
                {
                    AssetImporter importer = AssetImporter.GetAtPath(scene.path);
                    if (importer != null && !string.IsNullOrEmpty(importer.assetBundleName))
                        return importer.assetBundleName;
                }
            }

            return null;
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