using UnityEngine;
using UnityEditor;

namespace SpatialSys.UnitySDK
{
    public class EditorConfig : ScriptableObject
    {
        [System.Serializable]
        public class EnvironmentVariant
        {
            public string name = "My Environment";
            public SceneAsset scene = null;
            public Texture2D thumbnail = null;
            public Color thumbnailColor = Color.blue;
        }

        public enum UsageType
        {
            Uncategorized = 0,
            Event = 1,
            Gallery = 2,
            WatchParty = 3,
            Education = 4,
            Workplace = 5
        }

        private static EditorConfig _instance = null;
        public static EditorConfig instance
        {
            get
            {
                if (_instance == null)
                {
                    // Just take the first config we find in the project for now.
                    string[] configGuids = AssetDatabase.FindAssets($"t:{nameof(EditorConfig)}");

                    if (configGuids.Length > 0)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(configGuids[0]);
                        _instance = AssetDatabase.LoadAssetAtPath<EditorConfig>(path);
                    }
                }

                return _instance;
            }
        }

        public string packageName = "My Package";
        public string description = "My new Spatial SDK package";
        public UsageType usageType = UsageType.Uncategorized;
        public EnvironmentVariant[] environmentVariants = new EnvironmentVariant[0];
    }
}