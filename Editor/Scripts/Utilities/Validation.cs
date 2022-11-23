using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SpatialSys.UnitySDK.Editor
{
    /// <summary>
    /// Very basic/imperfect initial version of a validation system for Spatial SDK packages
    /// </summary>
    public class Validation
    {
        public static bool Validate(out Exception error)
        {
            error = null;
            try
            {
                ValidateConfiguration();
            }
            catch (Exception ex)
            {
                error = ex;
            }
            return error == null;
        }

        // TODO: make sure that all textures that are referenced within the environment are marked as non-readable

        /// <summary>
        /// Validates and enforces correct configuration of the package
        /// </summary>
        private static void ValidateConfiguration()
        {
            PackageConfig config = PackageConfig.instance;
            if (config == null)
                throw new System.Exception("Configuration asset does not exist");

            // Ensure there is at least one environment variant defined.
            if (config.environment.variants == null || config.environment.variants.Length == 0)
                throw new System.Exception("There must be at least one environment variant defined");

            // Ensure the variants each have a unique non-empty name
            var variantNames = new HashSet<string>();
            foreach (PackageConfig.Environment.Variant variant in config.environment.variants)
            {
                if (string.IsNullOrEmpty(variant.name))
                    throw new System.Exception("Environment variant name cannot be empty");

                if (!variantNames.Add(variant.name))
                    throw new System.Exception($"Environment variant name '{variant.name}' is not unique");
            }

            // Ensure that a scene is assigned to each variant.
            if (config.environment.variants.Any(v => v.scene == null))
                throw new System.Exception("One or more environment variants are missing a scene");

            // Ensure that a thumbnail is assigned to each variant
            if (config.environment.variants.Any(v => v.thumbnail == null))
                throw new System.Exception("One or more environment variants are missing a thumbnail");

            // Enforce thumbnail texture import settings
            BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
            void EnforceThumbnailImportSettings(Texture2D texture, int maxSize)
            {
                string path = AssetDatabase.GetAssetPath(texture);
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                importer.isReadable = true;
                importer.SetPlatformTextureSettings(new TextureImporterPlatformSettings() {
                    name = buildTargetGroup.ToString(),
                    overridden = true,
                    maxTextureSize = maxSize,
                    format = TextureImporterFormat.RGB24,
                    // needed for PNG format; this is typically not embedded in the bundle
                    textureCompression = TextureImporterCompression.Uncompressed,
                });
                AssetDatabase.ImportAsset(path);
            }
            foreach (PackageConfig.Environment.Variant variant in config.environment.variants)
            {
                EnforceThumbnailImportSettings(variant.thumbnail, 1024);
                if (variant.miniThumbnail != null)
                    EnforceThumbnailImportSettings(variant.miniThumbnail, 64);
            }

            // Ensure that the thumbnail is 1024x512
            if (config.environment.variants.Any(v => v.thumbnail.width != 1024 || v.thumbnail.height != 512))
                throw new System.Exception("One or more environment variants have a thumbnail that is not 1024x512");

            // If any mini thumbnails are assigned, ensure that they are 64x64
            if (config.environment.variants.Any(v => v.miniThumbnail != null && (v.miniThumbnail.width != 64 || v.miniThumbnail.height != 64)))
                throw new System.Exception("One or more environment variants have a mini thumbnail that is not 64x64");

            // Ensure that each variant is assigned a unique scene
            var sceneSet = new HashSet<SceneAsset>();
            foreach (PackageConfig.Environment.Variant variant in config.environment.variants)
            {
                if (sceneSet.Contains(variant.scene))
                    throw new System.Exception("One or more environment variants are assigned the same scene");

                sceneSet.Add(variant.scene);
            }

            // Clear all asset bundle assets in the project
            foreach (string name in AssetDatabase.GetAllAssetBundleNames())
                AssetDatabase.RemoveAssetBundleName(name, forceRemove: true);
            AssetDatabase.Refresh();

            // Assign a unique asset bundle name to each scene
            foreach (PackageConfig.Environment.Variant variant in config.environment.variants)
            {
                string scenePath = AssetDatabase.GetAssetPath(variant.scene);
                AssetImporter importer = AssetImporter.GetAtPath(scenePath);
                if (importer == null)
                    throw new System.Exception("Failed to get asset importer for scene: " + scenePath);

                importer.assetBundleName = variant.bundleName;
                AssetDatabase.SaveAssetIfDirty(importer);
            }
        }
    }
}