using System.Collections.Generic;
using SpatialSys.UnitySDK.Internal;

namespace SpatialSys.UnitySDK.Editor
{
    public static class SpacePackageTests
    {
        [PackageTest(PackageType.Space)]
        public static void EnsureSceneIsAssigned(SpaceConfig config)
        {
            if (config.scene == null)
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(
                        config,
                        TestResponseType.Fail,
                        $"There is no scene assigned to the SpaceConfig"
                    )
                );
            }
        }

        [PackageTest(PackageType.Space)]
        public static void ValidateEmbeddedPackageAssets(SpaceConfig config)
        {
            // Validate that IDs are assigned and unique
            int noIdsAssignedCount = 0;
            HashSet<string> ids = new();
            HashSet<string> duplicateIds = new();
            HashSet<string> invalidAssets = new();
            foreach (EmbeddedPackageAsset em in config.embeddedPackageAssets)
            {
                if (string.IsNullOrEmpty(em.id))
                {
                    noIdsAssignedCount++;
                    continue;
                }

                if (ids.Contains(em.id))
                {
                    duplicateIds.Add(em.id);
                    continue;
                }
                ids.Add(em.id);

                if (em.asset == null)
                {
                    invalidAssets.Add(em.id);
                    continue;
                }
            }
            if (noIdsAssignedCount > 0)
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(
                        config,
                        TestResponseType.Fail,
                        $"There are {noIdsAssignedCount} embedded packages with no ID assigned"
                    )
                );
            }
            if (duplicateIds.Count > 0)
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(
                        config,
                        TestResponseType.Fail,
                        $"There are {duplicateIds.Count} embedded package assets with non-unique ids",
                        $"The following ids are duplicated: {string.Join(", ", duplicateIds)}"
                    )
                );
            }
            if (invalidAssets.Count > 0)
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(
                        config,
                        TestResponseType.Fail,
                        $"There are {invalidAssets.Count} embedded package assets with no asset assigned",
                        $"The following embedded package assets have no asset assigned: {string.Join(", ", invalidAssets)}"
                    )
                );
            }

            // Check that each embedded asset type is supported.
            foreach (EmbeddedPackageAsset em in config.embeddedPackageAssets)
            {
                if (em.asset == null)
                    continue;

                bool isSupportedType = em.asset is SpatialAvatarAttachment avatarAttachment ||
                    em.asset is SpatialAvatar avatar ||
                    em.asset is SpatialAvatarAnimation avatarAnimation ||
                    em.asset is SpatialPrefabObject prefabObject;

                if (!isSupportedType)
                {
                    SpatialValidator.AddResponse(new SpatialTestResponse(
                        config, TestResponseType.Fail,
                        $"The embedded package '{em.asset.name}' (ID: {em.id}) is not a valid package type",
                        "Only embedded packages of type Avatar, AvatarAnimation, AvatarAttachment, and PrefabObject are currently supported"
                    ));
                }
            }
        }

        [PackageTest(PackageType.Space)]
        public static void EnsureCapacityDoesNotExceedMaximum(SpaceConfig config)
        {
            if (config.settings.serverCapacitySetting != ServerCapacitySetting.Custom)
                return;

            if (config.settings.serverInstanceCapacity < 1 || config.settings.serverInstanceCapacity > SpaceConfig.PLATFORM_MAX_CAPACITY)
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(
                        config,
                        TestResponseType.Fail,
                        "Invalid space maximum capacity",
                        $"The maximum capacity for a Space should be at between 1 and {SpaceConfig.PLATFORM_MAX_CAPACITY}"
                    )
                );
            }
        }
    }
}
