using System.Collections.Generic;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;

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
        public static void ValidateNetworkPrefabs(SpaceConfig config)
        {
            // All networkPrefabs must have unique IDs
            bool hasMissingReferences = false;
            HashSet<int> ids = new();
            foreach (SpatialNetworkObjectReferenceData networkPrefabRefData in config.networkPrefabs)
            {
                if (networkPrefabRefData.referenceType == NetworkPrefabReferenceType.Prefab)
                {
                    if (networkPrefabRefData.networkObject == null)
                    {
                        hasMissingReferences = true;
                        continue;
                    }

                    if (ids.Contains(networkPrefabRefData.networkObject.networkPrefabGuid))
                    {
                        SpatialTestResponse resp = new(
                            config,
                            TestResponseType.Fail,
                            "Network prefab GUID conflict",
                            $"There are multiple network prefabs with the same GUID ({networkPrefabRefData.networkObject.networkPrefabGuid}) assigned to the SpaceConfig"
                        );
                        resp.SetAutoFix(isSafe: true, "Remove duplicates from the networkPrefabs list", configObj => {
                            List<SpatialNetworkObjectReferenceData> uniqueNetworkPrefabs = new();
                            HashSet<int> uniqueIDs = new();
                            foreach (SpatialNetworkObjectReferenceData refData in config.networkPrefabs)
                            {
                                if (uniqueIDs.Contains(refData.networkObject.networkPrefabGuid))
                                    continue;
                                uniqueIDs.Add(refData.networkObject.networkPrefabGuid);
                                uniqueNetworkPrefabs.Add(refData);
                            }
                            config.networkPrefabs = uniqueNetworkPrefabs.ToArray();
                        });
                        SpatialValidator.AddResponse(resp);
                        continue;
                    }
                    ids.Add(networkPrefabRefData.networkObject.networkPrefabGuid);
                }
                else
                {
                    SpatialValidator.AddResponse(
                        new SpatialTestResponse(config, TestResponseType.Warning, "Only Prefab reference types are supported in SpaceConfig networkPrefabs")
                    );
                }
            }

            if (hasMissingReferences)
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(config, TestResponseType.Warning, "There are missing network prefabs in the SpaceConfig")
                );
            }
        }

        [PackageTest(PackageType.Space)]
        public static void ValidateNoAddressableScenesBuilt(SpaceConfig config)
        {
            if (!AddressablesUtility.isActiveInProject || !config.supportsAddressables)
                return;

            // Verify that there are no addressable scenes referenced.
            AddressableAssetSettings addressableSettings = AddressableAssetSettingsDefaultObject.Settings;
            List<AddressableAssetEntry> sceneEntries = new();
            foreach (AddressableAssetGroup group in addressableSettings.groups)
            {
                if (group == null)
                    continue;

                // Skip validation on any groups that won't be included in the build.
                BundledAssetGroupSchema schema = group.GetSchema<BundledAssetGroupSchema>();
                if (schema == null || !schema.IncludeInBuild)
                    continue;

                sceneEntries.Clear();
                group.GatherAllAssets(sceneEntries, includeSelf: true, recurseAll: true, includeSubObjects: false,
                    entryFilter: (entry) => entry.MainAsset != null && entry.IsScene
                );

                foreach (AddressableAssetEntry entry in sceneEntries)
                {
                    SpatialValidator.AddResponse(new SpatialTestResponse(
                        entry.MainAsset,
                        TestResponseType.Fail,
                        $"Scene '{entry.MainAsset.name}' from group '{group.Name}' cannot be marked as an Addressable",
                        $"Loading scenes via Addressables is not supported yet. Either remove the scene entry or disable 'Include in Build' in '{group.Name}'."
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
