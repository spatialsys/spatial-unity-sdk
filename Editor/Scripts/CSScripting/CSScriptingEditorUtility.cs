using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.Build.Player;

namespace SpatialSys.UnitySDK.Editor
{
    public static class CSScriptingEditorUtility
    {
        public const string OUTPUT_ASSET_PATH = "Assets/Spatial/Generated/" + CSScriptingUtility.CSHARP_ASSEMBLY_NAME + ".dll.txt";

        private static readonly string COMPILE_DESTINATION_DIR = $"Temp/CSScriptingCompiledDlls";

        public static bool CompileAssembly(AssemblyDefinitionAsset assemblyDefinition)
        {
            // TODO: DEV-27572 make sure assembly is named correctly

            BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
            BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
            ScriptCompilationSettings scriptCompilationSettings = new() {
                target = buildTarget,
                group = buildTargetGroup,
                options = ScriptCompilationOptions.None,
            };

            string outputDir = Path.Combine(COMPILE_DESTINATION_DIR, buildTarget.ToString());
            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);

            PlayerBuildInterface.CompilePlayerScripts(scriptCompilationSettings, outputDir);

            // Copy dll to generated folder
            string dllPath = Path.Combine(outputDir, CSScriptingUtility.CSHARP_ASSEMBLY_NAME + ".dll");
            if (File.Exists(dllPath))
            {
                string dllAssetPathOutputDir = Path.GetDirectoryName(OUTPUT_ASSET_PATH);
                if (!Directory.Exists(dllAssetPathOutputDir))
                    Directory.CreateDirectory(dllAssetPathOutputDir);

                File.Copy(dllPath, OUTPUT_ASSET_PATH, true);
                AssetDatabase.ImportAsset(OUTPUT_ASSET_PATH);
                AssetDatabase.Refresh();
                return true;
            }
            else
            {
                Debug.LogError($"Failed to compile {CSScriptingUtility.CSHARP_ASSEMBLY_NAME}.dll");
                return false;
            }
        }
    }
}