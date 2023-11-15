using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace SpatialSys.UnitySDK.Editor
{
    /// <summary>
    /// These package tests affects multiple/all package types.
    /// </summary>
    public static class PackageTests
    {
        public static readonly HashSet<string> UNSUPPORTED_MODEL_FILE_FORMATS = new HashSet<string>(new string[] {
            // We don't have a GLTF importer on the build side
            ".glb", ".gltf",
            // We can't support these unless we have licensed Maya, Max or Cinema 4D licenses on the build side
            ".ma", ".mb", ".max", ".c4d"
        });
        public static readonly HashSet<string> UNSUPPORTED_TEXTURE_FILE_FORMATS = new HashSet<string>(new string[] {
            // DDS is a DirectX format, and cannot be encoded into another format for iOS or Android platforms
            ".dds",
        });
        public static readonly HashSet<string> THUMBNAIL_TEXTURE_FORMATS = new HashSet<string>(new string[] { ".png", ".jpg", ".jpeg" });

        private static string[] GetPackageDependencies(PackageConfig config)
        {
            return AssetDatabase.GetDependencies(AssetDatabase.GetAssetPath(config))
                .Where(d => !d.StartsWith(PackageManagerUtility.PACKAGE_DIRECTORY_PATH))
                .ToArray();
        }

        [PackageTest]
        public static void CheckPackageName(PackageConfig config)
        {
            if (string.IsNullOrEmpty(config.packageName))
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(
                        config,
                        TestResponseType.Fail,
                        "Package name is empty",
                        "The package name is empty. Please enter a name for the package."
                    )
                );
            }
            else if (config.packageName.Length > PackageConfig.MAX_NAME_LENGTH)
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(
                        config,
                        TestResponseType.Fail,
                        "Package name is too long",
                        $"The package name is too long. Please enter a name that is {PackageConfig.MAX_NAME_LENGTH} characters or less."
                    )
                );
            }
        }

        [PackageTest]
        public static void CheckPackageFileSizeLimit(PackageConfig config)
        {
            string[] dependencies = GetPackageDependencies(config);
            int maxPackageSize = BuildUtility.MAX_PACKAGE_SIZE;
            if (Application.isBatchMode)
                maxPackageSize += 5 * 1024 * 1024; // Add 5MB to the limit in batch mode to account for file size differences between operating systems

            // Get size of each dependency
            Tuple<string, FileInfo>[] dependencyInfos = dependencies.Select(d => new Tuple<string, FileInfo>(d, new FileInfo(d))).ToArray();
            long totalSize = dependencyInfos.Sum(d => d.Item2.Length);
            if (totalSize > maxPackageSize)
            {
                var orderedDependencies = dependencyInfos.OrderByDescending(d => d.Item2.Length).Select(d => $"{d.Item2.Length / 1024 / 1024f:0.0000}MB - {d.Item1}").Take(25);
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(
                        null,
                        SpatialValidator.runContext == ValidationRunContext.UploadingToSandbox ? TestResponseType.Warning : TestResponseType.Fail,
                        "Package is too large to publish to Spatial",
                        $"The package is {totalSize / 1024f / 1024f}MB, but the maximum size is {maxPackageSize}MB. " +
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

            List<string> unsupportedModelFiles = dependencies.Where(d => UNSUPPORTED_MODEL_FILE_FORMATS.Contains(Path.GetExtension(d))).ToList();
            if (unsupportedModelFiles.Count > 0)
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(
                        null,
                        TestResponseType.Fail,
                        "Package contains unsupported model files",
                        $"This package contains file formats that are not supported (Unsupported: {string.Join(",", UNSUPPORTED_MODEL_FILE_FORMATS)}). " +
                        $"It is recommended to use the FBX format instead.\n - {string.Join("\n - ", unsupportedModelFiles)}"
                    )
                );
            }

            List<string> unsupportedTextureFiles = dependencies.Where(d => UNSUPPORTED_TEXTURE_FILE_FORMATS.Contains(Path.GetExtension(d))).ToList();
            if (unsupportedTextureFiles.Count > 0)
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(
                        null,
                        TestResponseType.Fail,
                        "Package contains unsupported texture files",
                        $"This package contains file formats that are not supported (Unsupported: {string.Join(",", UNSUPPORTED_TEXTURE_FILE_FORMATS)}). " +
                        $"It is recommended to use the PNG, JPG, or TGA format instead. This is to ensure encoding works on all Spatial's target platforms.\n - {string.Join("\n - ", unsupportedTextureFiles)}"
                    )
                );
            }
        }

        [PackageTest]
        public static void EnsureNoMultiplePackageAssetComponents(PackageConfig config)
        {
            foreach (GameObject go in config.gameObjectAssets)
            {
                SpatialPackageAsset[] pkgComponents = go.GetComponentsInChildren<SpatialPackageAsset>();

                if (pkgComponents.Length > 1)
                {
                    SpatialValidator.AddResponse(
                        new SpatialTestResponse(
                            go,
                            TestResponseType.Fail,
                            "This game object contains multiple package asset components on the object and it's children",
                            "The component names are listed below. There should only be one of these attached to this object.\n" +
                                EditorUtility.GetComponentNamesWithInstanceCountString(pkgComponents)
                        )
                    );
                }
            }
        }

        private const string LAST_COMMUNITY_GUIDELINES_ACCEPTED_DATE_PREFS_KEY = "SpatialSDK_CommunityGuidelinesAcceptedDate";

        [PackageTest]
        public static void AskUserIfPackageConformsToCommunityGuidelines(PackageConfig config)
        {
            // In batch mode, there's no human available to click the confirmation dialog
            if (Application.isBatchMode)
                return;

            if (SpatialValidator.runContext != ValidationRunContext.PublishingPackage)
                return;

            // Don't ask if they already accepted the prompt within the last 7 days.
            if (EditorUtility.TryGetDateTimeFromEditorPrefs(LAST_COMMUNITY_GUIDELINES_ACCEPTED_DATE_PREFS_KEY, out DateTime lastPromptTime) &&
                (DateTime.Now - lastPromptTime).TotalDays < 7.0)
            {
                return;
            }

            if (UnityEditor.EditorUtility.DisplayDialog("Content verification required",
                "Does the contents of your package abide by our Community Guidelines? You may review them at https://www.spatial.io/guidelines.",
                "Yes",
                "No, I will remove the violating content"
            ))
            {
                EditorUtility.SetDateTimeToEditorPrefs(LAST_COMMUNITY_GUIDELINES_ACCEPTED_DATE_PREFS_KEY, DateTime.Now);
            }
            else
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(
                        null,
                        TestResponseType.Fail,
                        "User did not confirm that the package abides by the Spatial Community Guidelines",
                        "Verify that the contents of this package does not violate of our Community Guidelines found at https://www.spatial.io/guidelines."
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
            if (config is SpaceTemplateConfig spaceTemplateConfig)
            {
                for (int i = 0; i < spaceTemplateConfig.variants.Length; i++)
                {
                    SpaceTemplateConfig.Variant variant = spaceTemplateConfig.variants[i];
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
                SpatialTestResponse testResponse;
                var responseType = SpatialValidator.runContext == ValidationRunContext.UploadingToSandbox ? TestResponseType.Warning : TestResponseType.Fail;

                string thumbnailSizeString = $"{config.thumbnailDimensions.x}x{config.thumbnailDimensions.y}";
                if (variantIndex.HasValue)
                {
                    testResponse = new SpatialTestResponse(
                        config,
                        responseType,
                        $"Package config has a variant (index {variantIndex.Value}) with no thumbnail assigned",
                        $"Assign a thumbnail of size {thumbnailSizeString} to this variant to fix this issue."
                    );
                }
                else
                {
                    testResponse = new SpatialTestResponse(
                        config,
                        responseType,
                        "Package config has no thumbnail assigned",
                        $"Assign a thumbnail of size {thumbnailSizeString} to fix this issue."
                    );
                }

                if (SpatialValidator.runContext == ValidationRunContext.UploadingToSandbox)
                    testResponse.description += " Thumbnails will be required when publishing.";

                SpatialValidator.AddResponse(testResponse);
            }
        }

        [PackageTest]
        public static void ValidateThumbnailSpecifications(PackageConfig config)
        {
            if (config is SpaceTemplateConfig spaceTemplateConfig)
            {
                for (int i = 0; i < spaceTemplateConfig.variants.Length; i++)
                {
                    SpaceTemplateConfig.Variant variant = spaceTemplateConfig.variants[i];
                    CheckThumbnailInternal(config, variant.thumbnail, config.thumbnailDimensions, "thumbnail");

                    // Only need to check mini thumbnail if there are multiple variants
                    if (spaceTemplateConfig.variants.Length > 1)
                        CheckThumbnailInternal(config, variant.miniThumbnail, SpaceTemplateConfig.MINI_THUMBNAIL_TEXTURE_DIMENSIONS, "mini thumbnail");
                }
            }
            else
            {
                CheckThumbnailInternal(config, config.thumbnail, config.thumbnailDimensions);
            }
        }

        private static void CheckThumbnailInternal(PackageConfig config, Texture2D texture, Vector2Int targetDimensions, string wording = "thumbnail")
        {
            if (texture == null)
                return;

            string path = AssetDatabase.GetAssetPath(texture);

            // Some formats like .exr (HDR) always have an alpha channel (used for extra precision); We don't want to assume
            // that this is the alpha channel that the user wants to use for transparency, so we'll just disallow it.
            // The EXR format seems to be allowed to be RGB on standalone, but on WebGL it's always RGBA.
            string extension = Path.GetExtension(path);
            if (!THUMBNAIL_TEXTURE_FORMATS.Contains(extension.ToLower()))
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(
                        texture,
                        TestResponseType.Fail,
                        $"Texture format {extension} is not supported for package {wording}",
                        $"Convert the texture into any of these file formats: " + string.Join(", ", THUMBNAIL_TEXTURE_FORMATS.Select(f => f.ToUpper()))
                    )
                );
            }

            // !! NOTE: If you change the enforcement here, you must also re-evaluate the "package-builder" project thumbnail
            // upload logic to ensure that it is compatible with the enforcement here.
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            int maxTextureSize = Mathf.Max(targetDimensions.x, targetDimensions.y);
            importer.ApplySettingsForThumbnailEncoding(config.allowTransparentThumbnails, maxTextureSize);

            var responseType = SpatialValidator.runContext == ValidationRunContext.UploadingToSandbox ? TestResponseType.Warning : TestResponseType.Fail;

            string BuildTestResponseDescription(string message)
            {
                if (SpatialValidator.runContext == ValidationRunContext.UploadingToSandbox)
                    message += " This will need to be fixed before publishing.";
                return message;
            }

            // Size can still be different from what was set on the importer.
            if (texture.width != targetDimensions.x || texture.height != targetDimensions.y)
            {
                var testResponse = new SpatialTestResponse(
                    texture,
                    responseType,
                    $"Package config has a {wording} with the incorrect size",
                    BuildTestResponseDescription($"Each variant must have a {wording} assigned of size {targetDimensions.x}x{targetDimensions.y}.")
                );

                if (SpatialValidator.runContext == ValidationRunContext.UploadingToSandbox)
                    testResponse.description += " This will need to be fixed before publishing.";

                SpatialValidator.AddResponse(testResponse);
                return;
            }

            // Check thumbnail transparency.
            bool thumbnailHasTransparency = texture.HasTransparency();
            bool thumbnailIsOpaque = !thumbnailHasTransparency;

            if (!config.allowTransparentThumbnails && thumbnailHasTransparency)
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(
                        texture,
                        responseType,
                        $"Package config has a {wording} with transparency",
                        BuildTestResponseDescription($"Each variant must have a {wording} without transparency. You can use a JPG file to ensure that there's no transparency in the image.")
                    )
                );
            }

            if (!config.allowOpaqueThumbnails && thumbnailIsOpaque)
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(
                        texture,
                        responseType,
                        $"Package config has a {wording} with no transparency",
                        BuildTestResponseDescription($"Each variant must have a {wording} with transparency.")
                    )
                );
            }
            else if (config.allowTransparentThumbnails && thumbnailHasTransparency && config.thumbnailMinTransparentBgRatio > 0f)
            {
                // If we allow transparent thumbnails, and this is a transparent thumbnail, check transparent background ratio (if applicable)
                // Just round to the nearest percent, we don't need to be too exact.
                int transparentPercentage = Mathf.FloorToInt(texture.GetTransparentBackgroundRatio() * 100f);
                int minTransparentPercentage = Mathf.RoundToInt(config.thumbnailMinTransparentBgRatio * 100f);

                if (transparentPercentage < minTransparentPercentage)
                {
                    SpatialValidator.AddResponse(
                        new SpatialTestResponse(
                            texture,
                            responseType,
                            $"Package config has a {wording} with not enough transparent pixels",
                            BuildTestResponseDescription($"At least {minTransparentPercentage}% of the pixels in the {wording} must be transparent (currently {transparentPercentage}%).\n" +
                                "This is to ensure that the avatar thumbnail displays properly when this avatar is selected.")
                        )
                    );
                }
            }
        }
    }
}
