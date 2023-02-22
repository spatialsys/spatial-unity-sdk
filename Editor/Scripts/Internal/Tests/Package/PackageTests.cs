using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

namespace SpatialSys.UnitySDK.Editor
{
    /// <summary>
    /// These package tests affects multiple/all package types.
    /// </summary>
    public static class PackageTests
    {
        public static readonly HashSet<string> UNSUPPORTED_MODEL_FORMATS = new HashSet<string>(new string[] { ".glb", ".gltf", ".ma", ".mb", ".max", ".c4d" });

        private static string[] GetPackageDependencies(PackageConfig config)
        {
            return AssetDatabase.GetDependencies(AssetDatabase.GetAssetPath(config))
                .Where(d => !d.StartsWith("Packages/io.spatial.unitysdk"))
                .ToArray();
        }

        [PackageTest]
        public static void CheckPackageFileSizeLimit(PackageConfig config)
        {
            string[] dependencies = GetPackageDependencies(config);

            // Get size of each dependency
            Tuple<string, FileInfo>[] dependencyInfos = dependencies.Select(d => new Tuple<string, FileInfo>(d, new FileInfo(d))).ToArray();
            long totalSize = dependencyInfos.Sum(d => d.Item2.Length);
            if (totalSize > BuildUtility.MAX_PACKAGE_SIZE)
            {
                var orderedDependencies = dependencyInfos.OrderByDescending(d => d.Item2.Length).Select(d => $"{d.Item2.Length / 1024 / 1024f:0.0000}MB - {d.Item1}").Take(25);
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(
                        null,
                        TestResponseType.Fail,
                        "Package is too large to upload to Spatial",
                        $"The package is {totalSize / 1024f / 1024f}MB, but the maximum size is {BuildUtility.MAX_PACKAGE_SIZE / 1024 / 1024}MB. " +
                        "The size of the package is equal to the raw file size of all your assets which get uploaded to Spatial. Import settings will not change this. " +
                        "Sometimes, texture and audio source assets can be very large on disk, and it can help to downscale them or re-export them in a different format." +
                        $"Here's a list of assets ordered by largest to smallest:\n - {string.Join("\n - ", orderedDependencies)}"
                    )
                );
            }
        }

        [PackageTest]
        public static void CheckPackageUnsupportedFilesLimit(PackageConfig config)
        {
            string[] dependencies = GetPackageDependencies(config);

            List<string> unsupportedModelFiles = dependencies.Where(d => UNSUPPORTED_MODEL_FORMATS.Contains(Path.GetExtension(d))).ToList();
            if (unsupportedModelFiles.Count > 0)
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(
                        null,
                        TestResponseType.Fail,
                        "Package contains unsupported model files",
                        "This package contains file formats that are not supported. " +
                        $"It is recommended to use the FBX format instead.\n - {string.Join("\n - ", unsupportedModelFiles)}"
                    )
                );
            }
        }

        [PackageTest]
        public static void CheckPackageForAssetsWithLongPaths(PackageConfig config)
        {
            // We need to account for the fact that the package will be processed on a windows machine where the 
            // path to the project is something like: "C:\agent\_work\6\s\_b\Assets\..."
            const int MAX_PATH_LENGTH = 200;
            string[] dependencies = GetPackageDependencies(config);

            List<string> longPathDependencies = dependencies.Where(d => d.Length > MAX_PATH_LENGTH).ToList();
            if (longPathDependencies.Count > 0)
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(
                        null,
                        TestResponseType.Fail,
                        "Package contains files with paths that are too long",
                        "This package contains files that have asset paths that are too long. " +
                        $"To be able to process this package on a windows machine, path lengths cannot exceed {MAX_PATH_LENGTH}. " +
                        $"Try to collapse folder hierarchies or give assets shorter file names.\n - {string.Join("\n - ", longPathDependencies)}"
                    )
                );
            }
        }

        [PackageTest]
        public static void CheckPackageType(PackageConfig config)
        {
            if (SpatialValidator.validationContext != ValidationContext.Publishing)
                return;

            // Temporarily disable publishing for new package types
            if (config is not EnvironmentConfig)
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(
                        config,
                        TestResponseType.Fail,
                        "Publishing for this package type is currently disabled",
                        "Only environment packages are allowed to be published. For now, you can only test within the sandbox. We will open up publishing for this very soon!"
                    )
                );
            }
        }

        //--------------------------------------------------------------------------------------------------------------
        // Thumbnail tests
        //--------------------------------------------------------------------------------------------------------------

        [PackageTest]
        public static void EnsureThumbnailsAreAssigned(PackageConfig config)
        {
            if (config is EnvironmentConfig envConfig)
            {
                for (int i = 0; i < envConfig.variants.Length; i++)
                {
                    EnvironmentConfig.Variant variant = envConfig.variants[i];
                    CheckVariantThumbnailAssigned(config, variant.thumbnail, i);
                }
            }
            else
            {
                CheckVariantThumbnailAssigned(config, config.thumbnail);
            }
        }

        private static void CheckVariantThumbnailAssigned(PackageConfig config, Texture2D thumbnail, int? variantIndex = null)
        {
            if (thumbnail == null)
            {
                string thumbnailSizeString = $"{config.thumbnailDimensions.x}x{config.thumbnailDimensions.y}";
                if (variantIndex.HasValue)
                {
                    SpatialValidator.AddResponse(
                        new SpatialTestResponse(
                            config,
                            TestResponseType.Fail,
                            $"Package config has a variant (index {variantIndex.Value}) with no thumbnail assigned",
                            $"Assign a thumbnail of size {thumbnailSizeString} to this variant to fix this issue."
                        )
                    );
                }
                else
                {
                    SpatialValidator.AddResponse(
                        new SpatialTestResponse(
                            config,
                            TestResponseType.Fail,
                            "Package config has no thumbnail assigned",
                            $"Assign a thumbnail of size {thumbnailSizeString} to fix this issue."
                        )
                    );
                }
            }
        }

        [PackageTest]
        public static void EnsureThumbnailsAreCorrectlySized(PackageConfig config)
        {
            if (config is EnvironmentConfig envConfig)
            {
                for (int i = 0; i < envConfig.variants.Length; i++)
                {
                    EnvironmentConfig.Variant variant = envConfig.variants[i];
                    CheckVariantThumbnail(i, variant.thumbnail, config.thumbnailDimensions, "thumbnail");
                    CheckVariantThumbnail(i, variant.miniThumbnail, EnvironmentConfig.MINI_THUMBNAIL_TEXTURE_DIMENSIONS, "mini thumbnail");
                }
            }
            else
            {
                CheckVariantThumbnail(0, config.thumbnail, config.thumbnailDimensions);
            }
        }

        private static void CheckVariantThumbnail(int variantIndex, Texture2D texture, Vector2Int dimensions, string wording = "thumbnail")
        {
            // Enforce texture size and format
            //
            // !! NOTE: If you change the enforcement here, you must also re-evaluate the "package-builder" project thumbnail
            // upload logic to ensure that it is compatible with the enforcement here.
            if (texture != null)
            {
                string path = AssetDatabase.GetAssetPath(texture);
                var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                importer.ClearPlatformTextureSettings("WebGL");
                importer.ClearPlatformTextureSettings("Standalone");
                importer.ClearPlatformTextureSettings("iPhone");
                importer.ClearPlatformTextureSettings("Android");

                TextureImporterPlatformSettings defaultSettings = importer.GetDefaultPlatformTextureSettings();
                defaultSettings.maxTextureSize = Mathf.Max(dimensions.x, dimensions.y);
                defaultSettings.format = TextureImporterFormat.RGB24;
                defaultSettings.textureCompression = TextureImporterCompression.Uncompressed;
                importer.SetPlatformTextureSettings(defaultSettings);

                importer.SaveAndReimport();

                // Size can still be different from what was set on the importer.
                if (texture.width != dimensions.x || texture.height != dimensions.y)
                {
                    SpatialValidator.AddResponse(
                        new SpatialTestResponse(
                            texture,
                            TestResponseType.Fail,
                            $"Package config has a variant (index {variantIndex}) with a {wording} with the incorrect size",
                            $"Each variant must have a {wording} assigned of size {dimensions.x}x{dimensions.y}."
                        )
                    );
                }
            }
        }
    }
}
