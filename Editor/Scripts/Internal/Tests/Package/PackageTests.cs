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
                    if (variant.thumbnail == null)
                    {
                        SpatialValidator.AddResponse(
                            new SpatialTestResponse(envConfig, TestResponseType.Fail, $"Package Config has variant with no thumbnail assigned. Each variant must have a thumbnail assigned of size 1024x512. Index: {i}")
                        );
                    }
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
                    CheckVariantThumbnail(i, "thumbnail", variant.thumbnail, ProjectConfig.THUMBNAIL_TEXTURE_WIDTH, ProjectConfig.THUMBNAIL_TEXTURE_HEIGHT);
                    CheckVariantThumbnail(i, "mini thumbnail", variant.miniThumbnail, ProjectConfig.MINI_THUMBNAIL_TEXTURE_WIDTH, ProjectConfig.MINI_THUMBNAIL_TEXTURE_HEIGHT);
                }
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
                var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                importer.ClearPlatformTextureSettings("WebGL");
                importer.ClearPlatformTextureSettings("Standalone");
                importer.ClearPlatformTextureSettings("iPhone");
                importer.ClearPlatformTextureSettings("Android");
                importer.SetPlatformTextureSettings(new TextureImporterPlatformSettings() {
                    name = "DefaultTexturePlatform",
                    maxTextureSize = width,
                    format = TextureImporterFormat.RGB24,
                    textureCompression = TextureImporterCompression.Uncompressed,
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
