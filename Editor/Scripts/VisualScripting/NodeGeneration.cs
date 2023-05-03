using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using Unity.VisualScripting;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SpatialSys.UnitySDK.Editor
{
    public static class NodeGeneration
    {
        private const string SETTINGS_ASSET_PATH = "ProjectSettings/VisualScriptingSettings.asset";
        private const string GENERATED_VS_NODES_VERSION_PREFS_KEY = "Spatial_GeneratedVSNodesVersion";

        static NodeGeneration()
        {
            if (PlayerPrefs.GetString(GENERATED_VS_NODES_VERSION_PREFS_KEY) != PackageManagerUtility.currentVersion)
            {
                EditorApplication.update += CheckForNodeRebuild;
            }
        }

        private static void CheckForNodeRebuild()
        {
            //in order to do a proper unit rebuild, we need to wait until we know VS has been initialized.
            //waiting until a VS window is opened is the best way I have found to do this
            if (EditorWindow.HasOpenInstances<GraphWindow>())
            {
                EditorApplication.update -= CheckForNodeRebuild;
                UnityEditor.EditorUtility.DisplayDialog("Spatial Scripting Initialization", "Hold tight while we make sure your visual scripting settings are just right", "OK");

                PlayerPrefs.SetString(GENERATED_VS_NODES_VERSION_PREFS_KEY, PackageManagerUtility.currentVersion);
                PlayerPrefs.Save();

                SetTypesAndAssemblies();
            }
        }

        /// <summary>
        /// We need to set the supported types and assemblies in ProjectSettings/VisualScripting
        /// Ludiq/Unity really does not want us to edit this... so we have to directly edit the json of the file >:(
        /// </summary>
        public static void SetTypesAndAssemblies()
        {
            VSUsageUtility.isVisualScriptingUsed = true;

            if (BoltCore.instance == null || BoltCore.Configuration == null || !File.Exists(SETTINGS_ASSET_PATH))
            {
                UnityEditor.EditorUtility.DisplayDialog("Visual Scripting Settings Not Found", "Visual Scripting is not initialized. Please navigate to the Visual Scripting settings in the Unity project settings to initialize", "OK");
                return;
            }

            // There's an embedded JSON in this asset file. Manually replace the assembly and type arrays.
            string settingsAssetContents = File.ReadAllText(SETTINGS_ASSET_PATH);
            settingsAssetContents = settingsAssetContents.SetJSONArrayValueHelper("assemblyOptions", NodeFilter.assemblyAllowList);

            List<Type> typesToGenerate = new List<Type>(NodeFilter.typeGeneration);
            typesToGenerate.RemoveAll(t => NodeFilter.typeBlockList.Contains(t));

            settingsAssetContents = settingsAssetContents.SetJSONArrayValueHelper("typeOptions", typesToGenerate.Select(type => type.FullName));

            File.WriteAllText(SETTINGS_ASSET_PATH, settingsAssetContents);

            PlayerPrefs.SetString(GENERATED_VS_NODES_VERSION_PREFS_KEY, PackageManagerUtility.currentVersion);
            PlayerPrefs.Save();

            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            CompilationPipeline.RequestScriptCompilation();
        }

        private static string SetJSONArrayValueHelper(this string target, string arrayContainerName, IEnumerable<string> arrayContents)
        {
            // Match pattern of "<arrayContainerName>":{"$content":[<any # of characters>]
            Regex reg = new Regex($"(\"{arrayContainerName}\":{{\"\\$content\":\\[)(.*)(\\])");
            string jsonArrayContents = string.Join(',', arrayContents.Select(s => $"\"{s}\""));
            // Only replace the second capture group, since that contains current array contents.
            return reg.Replace(target, $"$1{jsonArrayContents}$3");
        }

#if !SPATIAL_UNITYSDK_INTERNAL
        [InitializeOnLoadMethod]
        private static void OnScriptsReloaded()
        {
            if (PlayerPrefs.GetString(GENERATED_VS_NODES_VERSION_PREFS_KEY) == PackageManagerUtility.currentVersion)
            {
                UnitBase.Rebuild();
            }
        }
#endif
    }
}
