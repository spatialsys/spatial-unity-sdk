using Proyecto26;
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
            // We can't support these unless we have licensed Maya, Max or Cinema 4D licenses on the build side
            ".ma", ".mb", ".max", ".c4d"
        });
        public static readonly HashSet<string> UNSUPPORTED_TEXTURE_FILE_FORMATS = new HashSet<string>(new string[] {
            // DDS is a DirectX format, and cannot be encoded into another format for iOS or Android platforms
            ".dds",
        });
        public static readonly HashSet<string> THUMBNAIL_TEXTURE_FORMATS = new HashSet<string>(new string[] { ".png", ".jpg", ".jpeg" });

        static HashSet<TextureFormat> compressedTextureFormats = new HashSet<TextureFormat>() {
                TextureFormat.DXT5Crunched, TextureFormat.DXT1Crunched,
                TextureFormat.DXT5, TextureFormat.DXT1
            };

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
        public static RSG.IPromise CheckPackageFileSizeLimit(PackageConfig config)
        {
            // Package file size calculation is slow, and package limits should not be enforced on sandbox.
            // For publishing, validation will be run manually right before the package upload, since there can be extra asset processing that can affect final size.
            if (SpatialValidator.runContext != ValidationRunContext.ManualRun)
                return RSG.Promise.Resolved();

            // Do not run on package builder. It will have its own validation process.
            if (Application.isBatchMode)
                return RSG.Promise.Resolved();

            return ValidatePackageSize(config, SpatialValidator.runContext);
        }

        public static RSG.IPromise ValidatePackageSize(PackageConfig config, ValidationRunContext validationContext, string packagePath = null)
        {
            // If the package path is defined, a temporary package will not be generated, and will instead validate the file stored at `packagePath`.
            string tempExportPath = Path.Combine(Path.GetTempPath(), Path.ChangeExtension(Path.GetRandomFileName(), ".unitypackage"));
            SpatialAPI.GetPackageLimitsResponse packageLimits = new SpatialAPI.GetPackageLimitsResponse() {
                currentPlan = "",
                packageSizeLimit = BuildUtility.DEFAULT_MAX_PACKAGE_SIZE
            };
            RSG.IPromise<SpatialAPI.GetPackageLimitsResponse> getLimitsPromise = config.hasDynamicPackageSizeLimit
                ? SpatialAPI.GetPackageLimits(string.IsNullOrWhiteSpace(config.sku) ? "null" : config.sku)
                : RSG.Promise<SpatialAPI.GetPackageLimitsResponse>.Resolved(default);

            return getLimitsPromise
                .Then(resp => {
                    if (config.hasDynamicPackageSizeLimit)
                        packageLimits = resp;

                    if (packageLimits.packageSizeLimit <= 0)
                        return;

                    // Only create the package if no existing package path was supplied.
                    FileInfo packageInfo;
                    if (string.IsNullOrEmpty(packagePath))
                    {
                        BuildUtility.PackageProject(tempExportPath);
                        packageInfo = new(tempExportPath);
                    }
                    else
                    {
                        packageInfo = new(packagePath);
                    }

                    if (!packageInfo.Exists)
                    {
                        SpatialValidator.AddResponse(new SpatialTestResponse(
                            targetObject: null,
                            TestResponseType.Fail,
                            "Cannot validate package file size - file does not exist",
                            "Failed to locate the unitypackage file for validation. This should not happen."
                        ));
                        return;
                    }

                    if (packageInfo.Length > packageLimits.packageSizeLimit)
                    {
                        string[] dependencies = GetPackageDependencies(config);
                        // Get size of each dependency
                        IEnumerable<string> orderedDependencies = dependencies
                            .Select(d => new System.Tuple<string, FileInfo>(d, new FileInfo(d)))
                            .OrderByDescending(d => d.Item2.Length)
                            .Take(100)
                            .Select(d => $"{d.Item2.Length / 1024 / 1024f:0.000}MB - {d.Item1}");

                        SpatialTestResponse testResp = new SpatialTestResponse(
                            targetObject: null,
                            TestResponseType.Fail,
                            "Package size limit exceeded - file size is too large.",
                            $"The package size is {packageInfo.Length / 1024f / 1024f:0.00}MB, but the limit is {packageLimits.packageSizeLimit / 1024 / 1024}MB. " +
                            "The size of the package is equivalent to the .unitypackage file that gets uploaded to Spatial. The Unity package is a zipped file that " +
                            "contains the files stored within the project, so changing the import settings will not affect this. Some texture and audio files can be " +
                            "very large (e.g. WAV, TGA, TIF, etc.), so it can help to downscale them or re-export them in a different format (JPG, PNG, MP3, etc.)." +
                            $"\nHere's a list of the largest assets:\n - {string.Join("\n - ", orderedDependencies)}"
                        );

                        testResp.SetAutoFix(
                            isSafe: false,
                            "Optimize associated textures",
                            (target) => {
                                List<Texture2D> textures = new List<Texture2D>();
                                foreach (var dep in dependencies)
                                {
                                    Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(dep);
                                    if (tex != null)
                                        textures.Add(tex);
                                }
                                TextureOptimizer.OptimizeTextures(textures);
                            }
                        );
                        SpatialValidator.AddResponse(testResp);
                    }
                })
                .Catch(ex => {
                    if (ex is RequestException requestEx)
                    {
                        bool isPublishing = validationContext == ValidationRunContext.PublishingPackage;
                        string responseDescription = $"Make sure that you are signed in with a stable network connection.\n{requestEx.Message}";
                        if (!isPublishing)
                            responseDescription += "\n\nThis will be escalated to an error when publishing.";
                        SpatialValidator.AddResponse(new SpatialTestResponse(
                            targetObject: null,
                            responseType: isPublishing ? TestResponseType.Fail : TestResponseType.Warning,
                            title: "Failed to fetch package size limits",
                            description: responseDescription
                        ));
                        Debug.LogException(ex);
                        return; // Exception handled.
                    }

                    // Internal exception. Rethrow to bubble up the exception to the validator.
                    throw ex;
                })
                .Finally(() => {
                    File.Delete(tempExportPath);
                });
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

        [PackageTest]
        public static void CheckDependenciesForCompressionIssues(PackageConfig config)
        {
            // don't run if asset processing is disabled
            if (EditorPrefs.HasKey("DisableAssetProcessing") && EditorPrefs.GetBool("DisableAssetProcessing"))
                return;

            HashSet<Texture> thumbnails = new() { config.thumbnail };

            if (config is SpaceTemplateConfig spaceTemplateConfig)
            {
                foreach (var variant in spaceTemplateConfig.variants)
                {
                    thumbnails.Add(variant.thumbnail);
                    thumbnails.Add(variant.miniThumbnail);
                }
            }

            List<Tuple<string, Texture>> nonCompressableTextures = new List<Tuple<string, Texture>>();
            List<Tuple<string, TextureFormat>> suboptimalCompressionTextures = new List<Tuple<string, TextureFormat>>();

            void ValidateCompression(string texturePath, Texture texture)
            {
                // textures with dimensions that aren't a multiple of 4 can't be compressed using DXT for web
                if (texture.height % 4 != 0 || texture.width % 4 != 0)
                {
                    nonCompressableTextures.Add(new Tuple<string, Texture>(texturePath, texture));
                    return; // return early since we don't want uncompressed texture appearing in suboptimal compression list
                }

                // only check texture format on webgl
                if (texture is Texture2D tex2d && EditorUserBuildSettings.activeBuildTarget == BuildTarget.WebGL)
                {
                    if (!compressedTextureFormats.Contains(tex2d.format))
                    {
                        suboptimalCompressionTextures.Add(new Tuple<string, TextureFormat>(texturePath, tex2d.format));
                    }
                }
            }

            foreach (string path in GetPackageDependencies(config))
            {
                // ensure path exists and isn't a built-in texture or a .asset extension (like fonts, which shouldn't be compressed)
                if (string.IsNullOrEmpty(path) || path == "Resources/unity_builtin_extra" || Path.GetExtension(path) == ".asset")
                    continue;

                UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
                if (obj is Texture)
                {
                    Texture tex = obj as Texture;
                    // don't check compression if texture is a thumbnail - those need to be uncompressed
                    if (thumbnails.Contains(tex))
                        continue;

                    ValidateCompression(path, tex);
                }
            }

            if (nonCompressableTextures.Count > 0)
            {
                SpatialTestResponse resp = new SpatialTestResponse(
                    null,
                    TestResponseType.Tip,
                    $"Package {config.packageName} has non compress-able texture.",
                    "Textures with dimensions that are not divisible by 4 cannot be compressed using DXT1/DXT5 for WebGL. \n"
                        + "Textures with dimensions not divisible by 4: \n - " + string.Join("\n - ", nonCompressableTextures.Take(100).Select(m => $"{m.Item1} - {m.Item2.width}x{m.Item2.height}"))
                );

                resp.SetAutoFix(false, "Resize textures to multiples of 4",
                    (target) => {
                        TextureOptimizer.OptimizeTextures(nonCompressableTextures.Select(t => t.Item2 as Texture2D).ToList(), resize: false, saveOriginals: false, resizeToMultipleOfFour: true);
                    }
                );
                SpatialValidator.AddResponse(resp);
            }

            if (suboptimalCompressionTextures.Count > 0)
            {
                SpatialTestResponse resp = new SpatialTestResponse(
                    null,
                    TestResponseType.Tip,
                    $"Package {config.packageName} has textures compressed using a suboptimal format.",
                    "Use DXT1/DXT5 on web for smaller package size and quicker load times. \n"
                        + "Textures with suboptimal formats:\n - " + string.Join("\n - ", suboptimalCompressionTextures.Take(100).Select(m => $"{m.Item1} - {m.Item2}"))
                );

                resp.SetAutoFix(false, "Set compression format to DXT1/DXT5",
                    (target) => {
                        foreach (string assetPath in suboptimalCompressionTextures.Select(tuple => tuple.Item1))
                        {
                            TextureImporter textureImporter = (TextureImporter)TextureImporter.GetAtPath(assetPath);
                            AssetImportUtility.TextureImportProcess(textureImporter, assetPath, true);
                            AssetDatabase.ImportAsset(assetPath);
                        }
                    }
                );
                SpatialValidator.AddResponse(resp);
            }
        }
    }
}
