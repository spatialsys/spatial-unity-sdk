using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
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
        public const int THUMBNAIL_TEXTURE_WIDTH = 1024;
        public const int THUMBNAIL_TEXTURE_HEIGHT = 512;
        public const int MINI_THUMBNAIL_TEXTURE_WIDTH = 64;
        public const int MINI_THUMBNAIL_TEXTURE_HEIGHT = 64;
        public const int LATEST_VERSION = 1;

        public static ProjectConfig instance => AssetDatabase.LoadAssetAtPath<ProjectConfig>(ASSET_PATH);
        public static bool hasPackages => instance != null && instance._packages.Count > 0;
        public static IReadOnlyList<PackageConfig> packages => instance?._packages;
        public static PackageConfig activePackage
        {
            get
            {
                if (instance == null || instance._currentPackageIndex < 0 || instance._currentPackageIndex >= packages.Count)
                    return null;
                return packages[instance._currentPackageIndex];
            }
        }
        public static int activePackageIndex
        {
            get => instance?._currentPackageIndex ?? -1;
            set
            {
                if (instance == null || value < 0 || value >= packages.Count)
                    return;
                instance._currentPackageIndex = value;
                AssetDatabase.SaveAssets();
            }
        }

#pragma warning disable 414
        [HideInInspector]
        [SerializeField] private int _configVersion; // version of this config model; Used for making backwards-compatible changes
#pragma warning restore 414
        [SerializeField] private List<PackageConfig> _packages = new List<PackageConfig>();
        [SerializeField] private int _currentPackageIndex = 0;

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

                    EnvironmentConfig newConfig = AddNewPackage(PackageType.Environment) as EnvironmentConfig;
                    newConfig.packageName = oldConfig.packageName;
                    newConfig.sku = oldConfig.sku;

                    newConfig.variants = new EnvironmentConfig.Variant[oldConfig.environment.variants.Length];
                    for (int j = 0; j < oldConfig.environment.variants.Length; j++)
                    {
                        newConfig.variants[j] = new EnvironmentConfig.Variant();
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

            AssetDatabase.CreateAsset(config, ASSET_PATH);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        public static PackageConfig AddNewPackage(PackageType type)
        {
            if (instance == null)
                throw new System.Exception("ProjectConfig does not exist");

            // Get the C# type from the enum
            Type packageConfigType = null;
            switch (type)
            {
                case PackageType.Environment:
                    packageConfigType = typeof(EnvironmentConfig);
                    break;

                default:
                    throw new System.Exception($"Package type {type} is not yet supported");
            }

            // Get a unique name for the new package
            string name = $"{type}_1";
            int i = 2;
            while (AssetDatabase.FindAssets($"{name} t:{packageConfigType.Name}").Length > 0
                || File.Exists($"{CONFIG_DIRECTORY}/{name}.asset"))
            {
                name = $"{type}_{i}";
                i++;
            }

            // Create new package asset
            var package = CreateInstance(packageConfigType.Name) as PackageConfig;
            package.packageName = name;
            AssetDatabase.CreateAsset(package, $"{CONFIG_DIRECTORY}/{package.packageName}.asset");
            instance._packages.Add(package);
            UnityEditor.EditorUtility.SetDirty(instance);
            AssetDatabase.SaveAssets();

            return package;
        }

        /// <summary>
        /// Set what the active package should be based on the main source asset for that package.
        /// For environments, this is a scene asset.
        /// </summary>
        public static void SetActivePackageBySourceAsset(UnityEngine.Object sourceAsset)
        {
            // If this is the source asset for the currently active package, don't search for it
            // because some packages may use the same scene asset
            if (activePackage != null && activePackage.IsMainAssetForPackage(sourceAsset))
                return;

            // Find the package the current scene is assigned to
            foreach (PackageConfig package in ProjectConfig.packages)
            {
                if (package.IsMainAssetForPackage(sourceAsset))
                {
                    SetActivePackage(package);
                    return;
                }
            }
        }

        public static void SetActivePackage(PackageConfig package)
        {
            if (activePackage == package)
                return;

            if (instance == null)
                throw new System.Exception("ProjectConfig does not exist");

            int index = instance._packages.IndexOf(package);
            if (index >= 0)
            {
                Debug.Log($"Setting active package to {package.packageName}");
                instance._currentPackageIndex = index;
                UnityEditor.EditorUtility.SetDirty(instance);
                AssetDatabase.SaveAssets();
            }
            else
            {
                throw new System.Exception("Package does not exist in ProjectConfig");
            }
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
            if (instance._currentPackageIndex >= index)
                instance._currentPackageIndex--;
            instance._currentPackageIndex = Mathf.Clamp(instance._currentPackageIndex, 0, instance._packages.Count - 1);

            // Save changes
            UnityEditor.EditorUtility.SetDirty(instance);
            AssetDatabase.SaveAssets();
        }
    }
}