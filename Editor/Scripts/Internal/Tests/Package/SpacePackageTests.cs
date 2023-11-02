using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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

            // Validate each component
            foreach (EmbeddedPackageAsset em in config.embeddedPackageAssets)
            {
                SpatialPackageAsset spatialPackageAsset = em.asset;
                if (em.asset == null)
                    continue;

                if (spatialPackageAsset is SpatialAvatarAttachment avatarAttachment)
                {
                    AvatarAttachmentComponentTests.EnforceValidSetup(avatarAttachment);
                    SpatialValidator.RunTestsOnComponent(SpatialValidator.validationContext, avatarAttachment, additiveResults: true);
                }
                else if (spatialPackageAsset is SpatialAvatar avatar)
                {
                    SpatialValidator.RunTestsOnComponent(SpatialValidator.validationContext, avatar, additiveResults: true);
                }
                else if (spatialPackageAsset is SpatialAvatarAnimation avatarAnimation)
                {
                    SpatialValidator.RunTestsOnComponent(SpatialValidator.validationContext, avatarAnimation, additiveResults: true);
                }
                else if (spatialPackageAsset is SpatialPrefabObject prefabObject)
                {
                    SpatialValidator.RunTestsOnComponent(SpatialValidator.validationContext, prefabObject, additiveResults: true);
                }
                else
                {
                    SpatialValidator.AddResponse(new SpatialTestResponse(
                        config, TestResponseType.Fail,
                        $"The embedded package with id `{em.id}` is not a valid package type",
                        "Only embedded packages of type Avatar, AvatarAnimation, AvatarAttachment, and PrefabObject are currently supported"
                    ));
                }
            }
        }

        [PackageTest(PackageType.Space)]
        public static void EnsureCapacityDoesNotExceedMaximum(SpaceConfig config)
        {
            if (config.settings.serverCapacitySetting == ServerCapacitySetting.Custom)
            {
                if (config.settings.serverInstanceCapacity < 2 || config.settings.serverInstanceCapacity > SpaceConfig.PLATFORM_MAX_CAPACITY)
                {
                    SpatialValidator.AddResponse(
                        new SpatialTestResponse(
                            config,
                            TestResponseType.Fail,
                            "Invalid space maximum capacity",
                            $"The maximum capacity for a Space should be minimum 2 and cannot exceed {SpaceConfig.PLATFORM_MAX_CAPACITY}"
                        )
                    );
                }
            }
        }
    }
}
