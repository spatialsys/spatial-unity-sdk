using UnityEngine;
using UnityEditor;

namespace SpatialSys.UnitySDK.Editor
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

        public static EditorConfig instance { get; private set; }
        static EditorConfig()
        {
            EditorApplication.update += DelayedInitialize;
            EditorApplication.projectChanged += SearchProjectForConfig;
        }

        /// <summary>
        /// A workaround to allow us to run Unity functions in the static constructor.
        /// </summary>
        private static void DelayedInitialize()
        {
            EditorApplication.update -= DelayedInitialize;
            SearchProjectForConfig();
        }

        private static void SearchProjectForConfig()
        {
            if (instance != null)
                return;

            string[] configGuids = AssetDatabase.FindAssets($"t:{nameof(EditorConfig)}");

            if (configGuids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(configGuids[0]);
                instance = AssetDatabase.LoadAssetAtPath<EditorConfig>(path);
            }
        }

        public string packageName = "My Package";
        public string description = "My new Spatial SDK package";
        public UsageType usageType = UsageType.Uncategorized;
        public EnvironmentVariant[] environmentVariants = new EnvironmentVariant[0];
    }
}