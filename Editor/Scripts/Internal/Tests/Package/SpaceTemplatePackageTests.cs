using System.Collections.Generic;

using UnityEditor;

namespace SpatialSys.UnitySDK.Editor
{
    public static class SpaceTemplatePackageTests
    {
        [PackageTest(PackageType.SpaceTemplate)]
        public static void EnsureVariantExists(SpaceTemplateConfig config)
        {
            if (config.variants.Length == 0)
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(config, TestResponseType.Fail, "There are no space template variants set.")
                );
            }
        }

        [PackageTest(PackageType.SpaceTemplate)]
        public static void EnsureVariantsHaveNonEmptyUniqueNames(SpaceTemplateConfig config)
        {
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

        [PackageTest(PackageType.SpaceTemplate)]
        public static void EnsureVariantsHaveUniqueScenesAssigned(SpaceTemplateConfig config)
        {
            var variantScenes = new HashSet<SceneAsset>();
            for (int i = 0; i < config.variants.Length; i++)
            {
                SpaceTemplateConfig.Variant variant = config.variants[i];
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
        [PackageTest(PackageType.SpaceTemplate)]
        public static void EnsureVariantSceneImporterCanBeFound(SpaceTemplateConfig config)
        {
            var variantScenes = new HashSet<SceneAsset>();
            for (int i = 0; i < config.variants.Length; i++)
            {
                SpaceTemplateConfig.Variant variant = config.variants[i];
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
    }
}
