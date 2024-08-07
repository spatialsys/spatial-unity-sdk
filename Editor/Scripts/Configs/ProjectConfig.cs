using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEditor;

namespace SpatialSys.UnitySDK.Editor
{
    /// <summary>
    /// Spatial UnitySDK Project Configuration; One per Unity project
    /// </summary>
    public class ProjectConfig : ScriptableObject
    {
        public const string CONFIG_DIRECTORY = "Assets/Spatial";
        public const string ASSET_PATH = "Assets/Spatial/ProjectConfig.asset";
        public const int LATEST_VERSION = 1;

        private const string ACTIVE_PACKAGE_INDEX_PREFS_KEY = "Spatial_ActivePackageIndex";

        private static ProjectConfig _cachedInstance;
        public static ProjectConfig instance
        {
            get
            {
                if (_cachedInstance == null)
                    _cachedInstance = AssetDatabase.LoadAssetAtPath<ProjectConfig>(ASSET_PATH);
                return _cachedInstance;
            }
        }

        public static string defaultWorldID
        {
            get => instance?._defaultWorldID;
            set
            {
                if (instance == null)
                    return;
                instance._defaultWorldID = value;
                UnityEditor.EditorUtility.SetDirty(instance);
                AssetDatabase.SaveAssets();
            }
        }
        public static bool hasPackages => instance != null && instance._packages.Count > 0;
        public static IReadOnlyList<PackageConfig> packages
        {
            get
            {
                int newActiveIndex = activePackageIndex;
                for (int i = 0; i < instance?._packages.Count; i++)
                {
                    if (instance?._packages[i] != null)
                        continue;

                    instance?._packages.RemoveAt(i);

                    if (i < activePackageIndex)
                        newActiveIndex--;
                }

                if (activePackageIndex != newActiveIndex)
                    activePackageIndex = newActiveIndex;

                return instance?._packages;
            }
        }
        public static PackageConfig activePackageConfig => hasPackages ? packages[activePackageIndex] : null;
        public static int activePackageIndex
        {
            get
            {
                if (!hasPackages)
                    return -1;
                int localValue = SessionState.GetInt(ACTIVE_PACKAGE_INDEX_PREFS_KEY, defaultValue: 0);
                return Mathf.Clamp(localValue, 0, instance._packages.Count - 1);
            }
            set
            {
                if (!hasPackages)
                    return;
                int newLocalValue = Mathf.Clamp(value, 0, instance._packages.Count - 1);
                if (activePackageIndex != newLocalValue)
                {
                    SessionState.SetInt(ACTIVE_PACKAGE_INDEX_PREFS_KEY, newLocalValue);
                    PlayerPrefs.SetInt(ACTIVE_PACKAGE_INDEX_PREFS_KEY, newLocalValue);
                    PlayerPrefs.Save();
                }
            }
        }

        private static Dictionary<PackageType, Type> _packageTypeToConfigTypes = new Dictionary<PackageType, Type>() {
            { PackageType.Space, typeof(SpaceConfig) },
            { PackageType.SpaceTemplate, typeof(SpaceTemplateConfig) },
            { PackageType.Avatar, typeof(AvatarConfig) },
            { PackageType.AvatarAnimation, typeof(AvatarAnimationConfig) },
            { PackageType.PrefabObject, typeof(PrefabObjectConfig) },
            { PackageType.AvatarAttachment, typeof(AvatarAttachmentConfig) }
        };

        static ProjectConfig()
        {
            // Use delayCall since reading from PlayerPrefs is not allowed in the static constructor.
            EditorApplication.delayCall += () => {
                activePackageIndex = PlayerPrefs.GetInt(ACTIVE_PACKAGE_INDEX_PREFS_KEY, defaultValue: 0);
            };
        }

#pragma warning disable 414
        [SerializeField, HideInInspector] private int _configVersion; // version of this config model; Used for making backwards-compatible changes
#pragma warning restore 414
        [FormerlySerializedAs("_worldID")]
        [Tooltip("The world that packages (such as spaces) will be added to by default when publishing. Changing this will not move existing packages to the new world.")]
        [SerializeField] private string _defaultWorldID;

        [SerializeField] private List<PackageConfig> _packages = new List<PackageConfig>();

        public static void Create()
        {
            if (instance != null)
                throw new System.Exception("Cannot create new ProjectConfig because one already exists");

            if (!Directory.Exists(CONFIG_DIRECTORY))
            {
                Directory.CreateDirectory(CONFIG_DIRECTORY);
                AssetDatabase.Refresh();
            }

            var config = CreateInstance<ProjectConfig>();
            config._configVersion = LATEST_VERSION;
            AssetDatabase.CreateAsset(config, ASSET_PATH);

#pragma warning disable 618 // Ignore PackageConfig_OLD deprecation warning
            // Backwards compatibility, if there are any PackageConfig_OLD then convert them to the new format
            var oldPackageConfigs = AssetDatabase.FindAssets($"t:{nameof(PackageConfig_OLD)}");
            if (oldPackageConfigs.Length > 0)
            {
                // Create new configuration assets for all old package configs
                config._packages = new List<PackageConfig>(oldPackageConfigs.Length);
                for (int i = 0; i < oldPackageConfigs.Length; i++)
                {
                    PackageConfig_OLD oldConfig = AssetDatabase.LoadAssetAtPath<PackageConfig_OLD>(AssetDatabase.GUIDToAssetPath(oldPackageConfigs[i]));

                    SpaceTemplateConfig newConfig = AddNewPackage(PackageType.SpaceTemplate, makeActive: false) as SpaceTemplateConfig;
                    newConfig.packageName = oldConfig.packageName;
                    newConfig.sku = oldConfig.sku;

                    newConfig.variants = new SpaceTemplateConfig.Variant[oldConfig.environment.variants.Length];
                    for (int j = 0; j < oldConfig.environment.variants.Length; j++)
                    {
                        newConfig.variants[j] = new SpaceTemplateConfig.Variant();
                        newConfig.variants[j].id = oldConfig.environment.variants[j].id;
                        newConfig.variants[j].name = oldConfig.environment.variants[j].name;
                        newConfig.variants[j].scene = oldConfig.environment.variants[j].scene;
                        newConfig.variants[j].thumbnail = oldConfig.environment.variants[j].thumbnail;
                        newConfig.variants[j].miniThumbnail = oldConfig.environment.variants[j].miniThumbnail;
                        newConfig.variants[j].thumbnailColor = oldConfig.environment.variants[j].thumbnailColor;
                    }
                    UnityEditor.EditorUtility.SetDirty(newConfig);
                }

                // Delete old package config assets
                for (int i = 0; i < oldPackageConfigs.Length; i++)
                {
                    AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(oldPackageConfigs[i]));
                }

                // Delete old "Spatial SDK" folder if empty
                const string OLD_CONFIG_DIRECTORY = "Assets/Spatial SDK";
                if (Directory.Exists(OLD_CONFIG_DIRECTORY) && Directory.GetFiles(OLD_CONFIG_DIRECTORY).Length == 0)
                {
                    Directory.Delete(OLD_CONFIG_DIRECTORY);
                    File.Delete(OLD_CONFIG_DIRECTORY + ".meta");
                }
            }
#pragma warning restore 618

            // Also include all existing configs in the project
            var existingConfigs = AssetDatabase.FindAssets($"t:{nameof(PackageConfig)}");
            for (int i = 0; i < existingConfigs.Length; i++)
            {
                config._packages.Add(AssetDatabase.LoadAssetAtPath<PackageConfig>(AssetDatabase.GUIDToAssetPath(existingConfigs[i])));
            }

            EditorUtility.SaveAssetImmediately(config);
        }

        public static PackageConfig AddNewPackage(PackageType type, bool makeActive)
        {
            if (instance == null)
                throw new System.Exception("ProjectConfig does not exist");

            // Get the C# type from the enum
            Type packageConfigType = _packageTypeToConfigTypes[type];

            // Get a unique name for the new package
            string name = null;
            string assetPath = null;
            for (int i = 1; i < int.MaxValue; i++)
            {
                name = $"{type}_{i}";
                assetPath = Path.Combine(CONFIG_DIRECTORY, $"{name}.asset");

                if (AssetDatabase.FindAssets($"{name} t:{packageConfigType.Name}").Length == 0 && !File.Exists(assetPath))
                    break;
            }

            // Create new package asset
            var package = (PackageConfig)ScriptableObject.CreateInstance(packageConfigType);
            package.packageName = name;
            AssetDatabase.CreateAsset(package, assetPath);
            instance._packages.Add(package);
            EditorUtility.SaveAssetImmediately(instance);

            if (makeActive)
                activePackageIndex = instance._packages.Count - 1;

            return package;
        }

        /// <summary>
        /// Set what the active package should be based on the main source asset for that package.
        /// For space templates, this is a scene asset.
        /// </summary>
        public static void SetActivePackageBySourceAsset(UnityEngine.Object sourceAsset)
        {
            // The active package already uses this source asset.
            if (activePackageConfig != null && activePackageConfig.ContainsAsset(sourceAsset))
                return;

            foreach (PackageConfig package in ProjectConfig.packages)
            {
                if (package.ContainsAsset(sourceAsset))
                {
                    SetActivePackage(package);
                    break;
                }
            }
        }

        public static void SetActivePackage(PackageConfig package)
        {
            if (activePackageConfig == package)
                return;

            if (instance == null)
                throw new System.Exception("ProjectConfig does not exist");

            int index = instance._packages.IndexOf(package);
            if (index == -1)
                throw new System.Exception("Package does not exist in ProjectConfig");

            activePackageIndex = index;
        }

        public static void RemovePackage(PackageConfig package)
        {
            if (instance == null)
                throw new System.Exception("ProjectConfig does not exist");

            int index = instance._packages.IndexOf(package);
            if (index < 0)
                return;

            // Remove
            instance._packages.RemoveAt(index);
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(package));

            // Make sure our selected package remains the same and that index is valid
            if (index <= activePackageIndex)
                activePackageIndex = activePackageIndex;

            // Save changes
            UnityEditor.EditorUtility.SetDirty(instance);
            AssetDatabase.SaveAssets();
        }
    }
}