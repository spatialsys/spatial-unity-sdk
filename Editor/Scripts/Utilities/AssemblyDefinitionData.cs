using UnityEditorInternal;
using UnityEngine;

namespace SpatialSys.UnitySDK.Editor
{
    /// <summary>
    /// Helper class that can parses an AssemblyDefinitionAsset into a defined structure.
    /// This structure may change between Unity versions.
    /// </summary>
    [System.Serializable]
    public class AssemblyDefinitionData
    {
        [System.Serializable]
        public struct VersionDefine
        {
            public string name;
            public string expression;
            public string define;
        }

        public string name;
        public string rootNamespace;
        public string[] references; // Can be GUIDs or names
        public string[] includePlatforms;
        public string[] excludePlatforms;
        public bool allowUnsafeCode;
        public bool overrideReferences;
        public string[] precompiledReferences;
        public bool autoReferenced;
        public string[] defineConstraints;
        public VersionDefine[] versionDefines;
        public bool noEngineReferences;

        public AssemblyDefinitionData() { }
        public AssemblyDefinitionData(string name)
        {
            this.name = name;
            references = new string[0];
            includePlatforms = new string[0];
            excludePlatforms = new string[0];
            precompiledReferences = new string[0];
            autoReferenced = true;
            defineConstraints = new string[0];
            versionDefines = new AssemblyDefinitionData.VersionDefine[0];
        }

        public AssemblyDefinitionData(AssemblyDefinitionAsset asset)
        {
            FromAsset(asset);
        }

        public void FromAsset(AssemblyDefinitionAsset asset)
        {
            FromJSON(asset.text);
        }

        public void FromJSON(string json)
        {
            JsonUtility.FromJsonOverwrite(json, this);
        }

        public override string ToString() => ToJSON();
        public string ToJSON()
        {
            return JsonUtility.ToJson(this, prettyPrint: true);
        }
    }
}
