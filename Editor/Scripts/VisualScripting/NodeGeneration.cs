using UnityEditor;
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
        private const string VS_VERSION_PREF = "InitializedVSVersion";

        static NodeGeneration()
        {
            if (EditorPrefs.GetInt(VS_VERSION_PREF, -1) != NodeFilter.VS_FILTER_VERSION)
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
                EditorPrefs.SetInt(VS_VERSION_PREF, NodeFilter.VS_FILTER_VERSION);
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
            UnityEditor.EditorPrefs.SetInt(VS_VERSION_PREF, NodeFilter.VS_FILTER_VERSION);
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
        }

        private static string SetJSONArrayValueHelper(this string target, string arrayContainerName, IEnumerable<string> arrayContents)
        {
            // Match pattern of "<arrayContainerName>":{"$content":[<any # of characters>]
            Regex reg = new Regex($"(\"{arrayContainerName}\":{{\"\\$content\":\\[)(.*)(\\])");
            string jsonArrayContents = string.Join(',', arrayContents.Select(s => $"\"{s}\""));
            // Only replace the second capture group, since that contains current array contents.
            return reg.Replace(target, $"$1{jsonArrayContents}$3");
        }

#if !SPATIAL_INTERNAL
        [InitializeOnLoadMethod]
        private static void OnScriptsReloaded()
        {
            if (EditorPrefs.GetInt(VS_VERSION_PREF, -1) == NodeFilter.VS_FILTER_VERSION)
            {
                UnitBase.Rebuild();
            }
        }
#endif
    }
}
