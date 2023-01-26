using System;
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
        public static readonly HashSet<string> UNSUPPORTED_MODEL_FORMATS = new HashSet<string>(new string[] { ".glb", ".gltf", ".ma", ".mb", ".max", ".c4d" });

        private static string[] GetPackageDependencies(PackageConfig package)
        {
            return AssetDatabase.GetDependencies(AssetDatabase.GetAssetPath(package))
                .Where(d => !d.StartsWith("Packages/io.spatial.unitysdk"))
                .ToArray();
        }

        [ProjectTest]
        public static void CheckPackageFileSizeLimit()
        {
            string[] dependencies = GetPackageDependencies(ProjectConfig.activePackage);

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
                        "Try to reduce the size of source assets referenced by scenes in your project. " +
                        "Sometimes, texture source assets can be very large on disk, and it can help to downscale them. " +
                        $"Here's a list of the largest assets in the project:\n - {string.Join("\n - ", orderedDependencies)}"
                    )
                );
            }
        }

        [ProjectTest]
        public static void CheckPackageUnsupportedFilesLimit()
        {
            string[] dependencies = GetPackageDependencies(ProjectConfig.activePackage);

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

        [ProjectTest]
        public static void CheckPackageForAssetsWithLongPaths()
        {
            // We need to account for the fact that the package will be processed on a windows machine where the 
            // path to the project is something like: "C:\agent\_work\6\s\_b\Assets\..."
            const int MAX_PATH_LENGTH = 200;
            string[] dependencies = GetPackageDependencies(ProjectConfig.activePackage);

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

        [ProjectTest]
        public static void EnsureVariantExists()
        {
            EnvironmentConfig config = ProjectConfig.activePackage as EnvironmentConfig;

            if (config.variants.Length == 0)
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(config, TestResponseType.Fail, "Package Config has no environments set.")
                );
            }
        }

        [ProjectTest]
        public static void EnsureVariantsHaveNonEmptyUniqueNames()
        {
            EnvironmentConfig config = ProjectConfig.activePackage as EnvironmentConfig;

            var variantNames = new HashSet<string>();
            for (int i = 0; i < config.variants.Length; i++)
            {
                var variant = config.variants[i];
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
            EnvironmentConfig config = ProjectConfig.activePackage as EnvironmentConfig;

            var variantScenes = new HashSet<SceneAsset>();
            for (int i = 0; i < config.variants.Length; i++)
            {
                var variant = config.variants[i];
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
            EnvironmentConfig config = ProjectConfig.activePackage as EnvironmentConfig;

            var variantScenes = new HashSet<SceneAsset>();
            for (int i = 0; i < config.variants.Length; i++)
            {
                var variant = config.variants[i];
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
            EnvironmentConfig config = ProjectConfig.activePackage as EnvironmentConfig;

            for (int i = 0; i < config.variants.Length; i++)
            {
                var variant = config.variants[i];
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
            EnvironmentConfig config = ProjectConfig.activePackage as EnvironmentConfig;

            for (int i = 0; i < config.variants.Length; i++)
            {
                var variant = config.variants[i];
                CheckVariantThumbnail(i, "thumbnail", variant.thumbnail, ProjectConfig.THUMBNAIL_TEXTURE_WIDTH, ProjectConfig.THUMBNAIL_TEXTURE_HEIGHT);
                CheckVariantThumbnail(i, "mini thumbnail", variant.miniThumbnail, ProjectConfig.MINI_THUMBNAIL_TEXTURE_WIDTH, ProjectConfig.MINI_THUMBNAIL_TEXTURE_HEIGHT);
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
