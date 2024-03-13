using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using SpatialSys.UnitySDK.Editor;

namespace SpatialSys.UnitySDK.Editor
{
    public class TestCompileOnAllPlatforms
    {
        private static List<BuildTarget> targets = new List<BuildTarget>
        {
            BuildTarget.WebGL,
            BuildTarget.iOS,
            BuildTarget.Android,
        };

        [MenuItem("Spatial SDK/Utilities/Compilation Test for Multiple Platforms")]
        public static void CompileOnAllPlatforms()
        {
            BuildTarget originalTarget = EditorUserBuildSettings.activeBuildTarget;
            BuildTargetGroup originalGroup = BuildPipeline.GetBuildTargetGroup(originalTarget);
            Dictionary<BuildTarget, string> compileResults = new Dictionary<BuildTarget, string>();

            // Pre-check installed platforms
            List<string> installedPlatforms = new List<string>();
            List<string> notInstalledPlatforms = new List<string>();
            foreach (BuildTarget target in targets)
            {
                if (EditorUtility.IsPlatformModuleInstalled(target))
                {
                    installedPlatforms.Add(target.ToString());
                }
                else
                {
                    notInstalledPlatforms.Add(target.ToString());
                }
            }

            string message = $"Compiling on platforms: {string.Join(", ", installedPlatforms)}.\n";
            if (notInstalledPlatforms.Count > 0)
            {
                message += $"\nNot installed platforms: {string.Join(", ", notInstalledPlatforms)}\n";
            }

            message += "\nWarning: This might take a few minutes.";

            bool userConsent = UnityEditor.EditorUtility.DisplayDialog(
                "Compilation Test",
                message,
                "Run Test",
                "Cancel"
            );

            if (!userConsent)
            {
                return;
            }

            bool allPlatformsSuccess = true;
            try
            {
                foreach (BuildTarget target in targets)
                {
                    if (!EditorUtility.IsPlatformModuleInstalled(target))
                    {
                        compileResults[target] = "Platform Not Installed";
                        continue;
                    }

                    EditorUserBuildSettings.SwitchActiveBuildTarget(BuildPipeline.GetBuildTargetGroup(target), target);
                    bool compileSuccess = false;

                    if (ProjectConfig.activePackageConfig is SpaceConfig spaceConfig && spaceConfig.csharpAssembly != null)
                    {
                        try
                        {
                            EditorUtility.CreateAssetBackup(spaceConfig.csharpAssembly);
                            CSScriptingEditorUtility.EnforceCustomAssemblyName(spaceConfig.csharpAssembly, null);
                            compileSuccess = CSScriptingEditorUtility.CompileAssembly(spaceConfig.csharpAssembly, null);
                            if (!compileSuccess)
                            {
                                Debug.LogError($"Failed to compile c# assembly for {target}");
                            }
                        }
                        finally
                        {
                            EditorUtility.RestoreAssetFromBackup(spaceConfig.csharpAssembly);
                        }
                    }

                    compileResults[target] = compileSuccess ? "Compile Succeeded" : "Compile Failed";
                    if (!compileSuccess)
                    {
                        allPlatformsSuccess = false;
                    }
                }
            }
            finally
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(originalGroup, originalTarget);
                PrintCompileResults(compileResults, allPlatformsSuccess);
            }
        }

        private static void PrintCompileResults(Dictionary<BuildTarget, string> results, bool success)
        {
            string summaryMessage = "";
            foreach (var result in results)
            {
                summaryMessage += $"{result.Key}: {result.Value}\n";
            }

            if (!success)
            {
                summaryMessage += $"\nSee Editor logs for compilation errors.";
            }

            UnityEditor.EditorUtility.DisplayDialog("Compilation Results", summaryMessage, "OK");
        }

    }
}
