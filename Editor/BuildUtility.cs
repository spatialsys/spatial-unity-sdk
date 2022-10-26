using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SpatialSys.UnitySDK
{
    public class BuildUtility
    {
        public static string BUILD_DIR = "Exports";
        public static string PACKAGE_EXPORT_PATH = Path.Combine(BUILD_DIR, "spaces.unitypackage");

        public static void BuildForPlaySpace()
        {
            // Build bundle for each target platform
            BuildTarget[] buildTargets = new BuildTarget[] { BuildTarget.WebGL, BuildTarget.StandaloneOSX };
            foreach (BuildTarget target in buildTargets)
            {
                string dir = Path.Combine(BUILD_DIR, target.ToString());
                if (Directory.Exists(dir))
                    Directory.Delete(dir, true);
                Directory.CreateDirectory(dir);

                BuildPipeline.BuildAssetBundles(dir, BuildAssetBundleOptions.None, target);
            }

            // TODO: Upload bundles to SAPI
        }

        public static void PackageForPublishing()
        {
            string[] scenePaths = BuildUtility.GetAllAssetBundleScenePaths();

            // Export all scenes and dependencies as a package
            AssetDatabase.ExportPackage(
                scenePaths,
                PACKAGE_EXPORT_PATH,
                ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies
            );

            // TODO: Upload package to SAPI
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
    }
}