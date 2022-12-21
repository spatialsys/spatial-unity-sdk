using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

namespace SpatialSys.UnitySDK.Editor
{
    // Project tests are run once.
    public class ProjectTests
    {
        [ProjectTest]
        public static void CheckPackageFileSizeLimit()
        {
            // Don't run this test on build servers
            if (Application.isBatchMode)
                return;

            // To speed up testing iteration times, don't run this test for test case
            if (SpatialValidator.validationContext == ValidationContext.Testing)
                return;

            string tempOutputPath = "Temp/SpatialPackage.unitypackage";
            BuildUtility.PackageProject(tempOutputPath);

            FileInfo packageInfo = new FileInfo(tempOutputPath);
            if (packageInfo.Length > BuildUtility.MAX_PACKAGE_SIZE)
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(
                        null,
                        TestResponseType.Fail,
                        "Package is too large to upload to Spatial",
                        $"The package is {packageInfo.Length / 1024 / 1024}MB, but the maximum size is {BuildUtility.MAX_PACKAGE_SIZE / 1024 / 1024}MB. " +
                        "Try to reduce the size of source assets referenced by scenes in your project. Sometimes, texture source assets can be very large on disk, and it can help to downscale them."
                    )
                );
            }
        }

        [ProjectTest]
        public static void EnsureVariantExists()
        {
            PackageConfig config = PackageConfig.instance;

            if (config.environment.variants.Length == 0)
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(config, TestResponseType.Fail, "Package Config has no environments set.")
                );
            }
        }

        [ProjectTest]
        public static void EnsureVariantsHaveNonEmptyUniqueNames()
        {
            PackageConfig config = PackageConfig.instance;

            var variantNames = new HashSet<string>();
            for (int i = 0; i < config.environment.variants.Length; i++)
            {
                var variant = config.environment.variants[i];
                if (string.IsNullOrEmpty(variant.name) || string.IsNullOrWhiteSpace(variant.name))
                {
                    SpatialValidator.AddResponse(
                        new SpatialTestResponse(config, TestResponseType.Fail, $"Package Config has variant with no name defined. Index: {i}",
                            "Make sure each variant has the name field filled inside the config tab")
                    );
                }
                else if (!variantNames.Add(variant.name))
                {
                    SpatialValidator.AddResponse(
                        new SpatialTestResponse(config, TestResponseType.Fail, $"Package Config has variant with non-unique name. Name: {variant.name}; Index: {i}",
                            "Make sure that each variant has a different name inside the config tab")
                    );
                }
            }
        }

        [ProjectTest]
        public static void EnsureVariantsHaveUniqueScenesAssigned()
        {
            PackageConfig config = PackageConfig.instance;

            var variantScenes = new HashSet<SceneAsset>();
            for (int i = 0; i < config.environment.variants.Length; i++)
            {
                var variant = config.environment.variants[i];
                if (variant.scene == null)
                {
                    SpatialValidator.AddResponse(
                        new SpatialTestResponse(config, TestResponseType.Fail, $"Package Config has variant with no scene assigned. Each variant must have a unique scene. Index: {i}")
                    );
                }
                else if (!variantScenes.Add(variant.scene))
                {
                    SpatialValidator.AddResponse(
                        new SpatialTestResponse(config, TestResponseType.Fail, $"Package Config has variant with non-unique scene. Each variant must be assigned a unique scene. Scene: {variant.scene}; Index: {i}")
                    );
                }
            }
        }

        // We just need this to succeed since we will need to access the importer to assign the bundle name correctly
        [ProjectTest]
        public static void EnsureVariantSceneImporterCanBeFound()
        {
            PackageConfig config = PackageConfig.instance;

            var variantScenes = new HashSet<SceneAsset>();
            for (int i = 0; i < config.environment.variants.Length; i++)
            {
                var variant = config.environment.variants[i];
                if (variant.scene != null)
                {
                    string scenePath = AssetDatabase.GetAssetPath(variant.scene);
                    AssetImporter importer = AssetImporter.GetAtPath(scenePath);
                    if (importer == null)
                    {
                        SpatialValidator.AddResponse(
                            new SpatialTestResponse(config, TestResponseType.Fail, $"There seems to be an unexpected issue with the scene assigned to variant {variant.name}. Scene: {scenePath}; Index: {i}")
                        );
                    }
                }
            }
        }

        //--------------------------------------------------------------------------------------------------------------
        // Thumbnail tests
        //--------------------------------------------------------------------------------------------------------------

        [ProjectTest]
        public static void EnsureVariantsHaveThumbnailAssigned()
        {
            PackageConfig config = PackageConfig.instance;

            for (int i = 0; i < config.environment.variants.Length; i++)
            {
                var variant = config.environment.variants[i];
                if (variant.thumbnail == null)
                {
                    SpatialValidator.AddResponse(
                        new SpatialTestResponse(config, TestResponseType.Fail, $"Package Config has variant with no thumbnail assigned. Each variant must have a thumbnail assigned of size 1024x512. Index: {i}")
                    );
                }
            }
        }

        [ProjectTest]
        public static void EnsureVariantsHaveCorrectlySizedThumbnails()
        {
            PackageConfig config = PackageConfig.instance;

            for (int i = 0; i < config.environment.variants.Length; i++)
            {
                var variant = config.environment.variants[i];
                CheckVariantThumbnail(i, "thumbnail", variant.thumbnail, PackageConfig.THUMBNAIL_TEXTURE_WIDTH, PackageConfig.THUMBNAIL_TEXTURE_HEIGHT);
                CheckVariantThumbnail(i, "mini thumbnail", variant.miniThumbnail, PackageConfig.MINI_THUMBNAIL_TEXTURE_WIDTH, PackageConfig.MINI_THUMBNAIL_TEXTURE_HEIGHT);
            }
        }

        private static void CheckVariantThumbnail(int variantIndex, string wording, Texture2D texture, int width, int height)
        {
            // Enforce texture size and format
            //
            // !! NOTE: If you change the enforcement here, you must also re-evaluate the "package-builder" project thumbnail
            // upload logic to ensure that it is compatible with the enforcement here.
            if (texture != null)
            {
                string path = AssetDatabase.GetAssetPath(texture);
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                importer.ClearPlatformTextureSettings("WebGL");
                importer.ClearPlatformTextureSettings("Standalone");
                importer.ClearPlatformTextureSettings("iPhone");
                importer.ClearPlatformTextureSettings("Android");
                importer.SetPlatformTextureSettings(new TextureImporterPlatformSettings() {
                    name = "DefaultTexturePlatform",
                    maxTextureSize = width,
                    format = TextureImporterFormat.RGB24,
                });
                AssetDatabase.ImportAsset(path);
            }

            // Is it the correct size and format?
            if (texture != null && (texture.width != width || texture.height != height) && texture.format != TextureFormat.RGB24)
            {
                var resp = new SpatialTestResponse(texture, TestResponseType.Fail, $"Package Config has variant with incorrectly sized {wording}. Each variant must have a {wording} assigned of size {width}x{height} and have the RGB24 format. Index: {variantIndex}");
                SpatialValidator.AddResponse(resp);
            }
        }
    }
}
