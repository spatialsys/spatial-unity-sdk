using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEditor;

namespace SpatialSys.UnitySDK.Editor
{
    /// <summary>
    /// The configuration of a SpatialSDK package
    /// </summary>
    [Obsolete("Use ProjectConfig instead")]
    public class PackageConfig_OLD : ScriptableObject
    {
        public static PackageConfig_OLD instance { get; private set; }

        //--------------------------------------------------------------------------------------------------------------
        // Shared Config
        //--------------------------------------------------------------------------------------------------------------

        public const int LATEST_VERSION = 2;

        [HideInInspector]
        public int configVersion; // version of this config model; Used for making backwards-compatible changes
        [HideInInspector]
        public string sku = "";
        public string packageName = "My Package";
        public string description = "My new Spatial SDK package";
        [HideInInspector]
        public PackageType packageType = PackageType.Environment; // only envrioment is supported for now

        public enum PackageType
        {
            Environment = 0,
            Avatar = 1, // not supported yet
            Object = 2  // not supported yet
        }

        //--------------------------------------------------------------------------------------------------------------
        // Environment Config
        //--------------------------------------------------------------------------------------------------------------

        public Environment environment = new Environment();

        [System.Serializable]
        public class Environment
        {
            public string[] useCases = new string[0]; // Valid entries here is defined by the UseCases enum
            public Variant[] variants = new Variant[1] { new Variant() };

            [System.Serializable]
            public class Variant
            {
                public string name = "My Environment";
                [HideInInspector]
                public string id; // We want this to be unique and never change
                public SceneAsset scene = null;
                public Texture2D thumbnail = null; // 1024x512
                public Texture2D miniThumbnail = null; // 64x64
                public Color thumbnailColor = Color.blue;

                private static readonly string[] INVALID_BUNDLE_NAME_CHARS = new string[] { " ", "_", ".", ",", "(", ")", "[", "]", "{", "}", "!", "@", "#", "$", "%", "^", "&", "*", "+", "=", "|", "\\", "/", "?", "<", ">", "`", "~", "'", "\"", ":", ";" };

                public string bundleName
                {
                    get
                    {
                        if (scene == null)
                            return null;

                        string pathSafeName = name.Replace(" ", "-").Replace("_", "-").ToLower();
                        foreach (string c in INVALID_BUNDLE_NAME_CHARS)
                            pathSafeName = pathSafeName.Replace(c, "");
                        return id + "_" + pathSafeName;
                    }
                }

                public static string NewID()
                {
                    return System.Guid.NewGuid().ToString().Replace("-", "");
                }
            }

            public enum UseCases
            {
                Event,
                Auditorium,
                Meeting,
                Gallery,
                WatchParty,
                Education,
                Workplace,
                Training,
                Retail,
                Gaming,
                Social,
                Concert,
                Party,
                Theater,
            }
        }

        private void OnValidate()
        {
            UpgradeDataIfNecessary();

            // If a new variant is added, unity will essentially duplicate the last variant in the array so we need to clear it
            if (environment.variants.Length >= 2)
            {
                // Check if a duplicate was just made
                Environment.Variant beforeLastVariant = environment.variants[environment.variants.Length - 2];
                Environment.Variant lastVariant = environment.variants[environment.variants.Length - 1];
                if (beforeLastVariant.id == lastVariant.id &&
                    beforeLastVariant.scene == lastVariant.scene &&
                    beforeLastVariant.thumbnail == lastVariant.thumbnail)
                {
                    environment.variants[environment.variants.Length - 1] = new Environment.Variant();
                }
            }

            // Assign unique IDs to variants if they don't have one
            foreach (Environment.Variant variant in environment.variants)
            {
                if (string.IsNullOrEmpty(variant.id))
                    variant.id = Environment.Variant.NewID();
            }
        }

        public void UpgradeDataIfNecessary()
        {
            if (configVersion == LATEST_VERSION)
                return;

            // Upgrade from version 0 to version 1
            if (configVersion == 0)
            {
#pragma warning disable 0618
                try
                {
                    // Only do the upgrade if the new fields are default values
                    if (environment.useCases.Length == 0 &&
                        (environment.variants.Length == 0 ||
                         environment.variants.Length == 1 && environment.variants[0].scene == null))
                    {
                        if (deprecated_usageType != Deprecated.UsageTypeV0.Uncategorized)
                            environment.useCases = new string[] { deprecated_usageType.ToString() };

                        environment.variants = new Environment.Variant[deprecated_environmentVariants.Length];
                        for (int i = 0; i < deprecated_environmentVariants.Length; i++)
                        {
                            Environment.Variant variant = new Environment.Variant();
                            variant.name = deprecated_environmentVariants[i].name;
                            variant.scene = deprecated_environmentVariants[i].scene;
                            variant.thumbnail = deprecated_environmentVariants[i].thumbnail;
                            variant.thumbnailColor = deprecated_environmentVariants[i].thumbnailColor;
                            environment.variants[i] = variant;
                        }

                        deprecated_usageType = (Deprecated.UsageTypeV0)0;
                        deprecated_environmentVariants = new Deprecated.EnvironmentVariantV0[0];
                    }
                    configVersion = 1;
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to upgrade {nameof(PackageConfig_OLD)} to version 1: {e.Message}");
                }
#pragma warning restore 0618
            }

            // Upgrade from version 1 to version 2
            if (configVersion == 1)
            {
                try
                {
                    // Assign the same id to the variant it previously had, or a new one if it didn't have one
                    foreach (Environment.Variant variant in environment.variants)
                    {
                        string oldID = (variant.scene == null) ? null : AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(variant.scene));
                        if (!string.IsNullOrEmpty(oldID))
                        {
                            variant.id = oldID;
                        }
                        else
                        {
                            variant.id = Environment.Variant.NewID();
                        }
                    }
                    configVersion = 2;
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to upgrade {nameof(PackageConfig_OLD)} to version 2: {e.Message}");
                }
            }

            // Set version to latest
            configVersion = LATEST_VERSION;
            UnityEditor.EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }

        static PackageConfig_OLD()
        {
            EditorApplication.update += DelayedInitialize;
        }

        /// <summary>
        /// A workaround to allow us to run Unity functions in the static constructor.
        /// </summary>
        private static void DelayedInitialize()
        {
            EditorApplication.update -= DelayedInitialize;
            SearchProjectForConfig();

            // Upgrade to ProjectConfig
            if (instance != null && ProjectConfig.instance == null)
            {
                Debug.Log("Updating project configuration files to new format");
                ProjectConfig.Create();
            }
        }

        private static void SearchProjectForConfig()
        {
            if (instance != null)
                return;

            string[] configGuids = AssetDatabase.FindAssets($"t:{nameof(PackageConfig_OLD)}");

            if (configGuids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(configGuids[0]);
                instance = AssetDatabase.LoadAssetAtPath<PackageConfig_OLD>(path);
            }
        }

        #region Deprecated, Keeping for backwards compatibility

        [System.Obsolete("Use environment.useCases instead"), HideInInspector, FormerlySerializedAs("usageType")]
        public Deprecated.UsageTypeV0 deprecated_usageType = Deprecated.UsageTypeV0.Uncategorized;
        [System.Obsolete("Use environment.variants instead"), HideInInspector, FormerlySerializedAs("environmentVariants")]
        public Deprecated.EnvironmentVariantV0[] deprecated_environmentVariants = new Deprecated.EnvironmentVariantV0[0];

        public static class Deprecated
        {
            [System.Obsolete("Use Environment.UseCases instead")]
            public enum UsageTypeV0
            {
                Uncategorized = 0,
                Event = 1,
                Gallery = 2,
                WatchParty = 3,
                Education = 4,
                Workplace = 5
            }

            [System.Serializable]
            [System.Obsolete("Use Environment.Variant instead")]
            public class EnvironmentVariantV0
            {
                public string name = "My Environment";
                public SceneAsset scene = null;
                public Texture2D thumbnail = null;
                public Color thumbnailColor = Color.blue;
            }
        }

        #endregion
    }
}