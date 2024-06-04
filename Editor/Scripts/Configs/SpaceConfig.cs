using System.Collections.Generic;
using SpatialSys.UnitySDK.Internal;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace SpatialSys.UnitySDK.Editor
{
    public class SpaceConfig : PackageConfig
    {
        private const PackageType PACKAGE_TYPE = PackageType.Space;
        private const int LATEST_VERSION = 1;
        public const int PLATFORM_MAX_CAPACITY = 50;

        [HideInInspector] public string worldID;

        // "Space Name" would be derived from PackageConfig.packageName
        public SceneAsset scene = null;
        [Tooltip("The C# assembly that contains the code for this space. Currently there's a limit of one assembly per space due to technical reasons.")]
        public AssemblyDefinitionAsset csharpAssembly = null;
        [Tooltip("Embedded packages are packages that are bundled together with this space. This is useful if you want to have custom avatars, avatar animations, avatar attachments and prefabs that are specific to this space, but don't want to publish them as separate packages. Note that embedding packages will increase the download size and load time of your space.")]
        public EmbeddedPackageAsset[] embeddedPackageAssets = new EmbeddedPackageAsset[0];
        [Tooltip("Reference to prefabs that contain one or more SpatialNetworkObject components, which can be spawned in this space and replicated to all other clients.")]
        public SpatialNetworkObjectReferenceData[] networkPrefabs = new SpatialNetworkObjectReferenceData[0];

        [Space(10)]
        public SpaceSettings settings;

        public override PackageType packageType => PACKAGE_TYPE;
        public override Vector2Int thumbnailDimensions => new Vector2Int(1024, 512);
        public override string bundleName => EditorUtility.GetAssetBundleName(scene);
        public override string validatorID => GetValidatorID();
        public override Scope validatorUsageContext => Scope.World;

        public override bool allowTransparentThumbnails => false;

        public override IEnumerable<Object> assets
        {
            get
            {
                if (scene != null)
                    yield return scene;

                foreach (EmbeddedPackageAsset embeddedAsset in embeddedPackageAssets)
                {
                    if (embeddedAsset?.asset != null)
                        yield return embeddedAsset.asset;
                }

                foreach (SpatialNetworkObjectReferenceData refData in networkPrefabs)
                {
                    if (refData.referenceType == NetworkPrefabReferenceType.Prefab && refData.networkObject != null)
                        yield return refData.networkObject;
                }
            }
        }

        public SpaceConfig()
        {
            version = LATEST_VERSION;
        }

        public static string GetValidatorID()
        {
            return PACKAGE_TYPE.ToString();
        }

        private void OnValidate()
        {
            UpgradeDataIfNecessary();

            foreach (SpatialNetworkObjectReferenceData refData in networkPrefabs)
            {
                bool isPrefabAsset = refData.networkObject != null && refData.networkObject.gameObject.scene.name == null;
                if (isPrefabAsset && refData.referenceType != NetworkPrefabReferenceType.Prefab)
                    refData.referenceType = NetworkPrefabReferenceType.Prefab;
            }
        }

        public void UpgradeDataIfNecessary()
        {
            if (version == LATEST_VERSION)
                return;

            // Version 0 was the initial version; Converting to V1 is just about setting good defaults
            if (version == 0)
            {
                // For previously created packages, default to the maximum capacity
                settings = new SpaceSettings {
                    serverInstancingEnabled = true,
                    serverCapacitySetting = ServerCapacitySetting.Maximum,
                    serverInstanceCapacity = PLATFORM_MAX_CAPACITY
                };

                version = 1;
            }

            // Add future upgrade paths here
        }
    }

    [System.Serializable]
    public class SpaceSettings
    {
        [Tooltip("If one server instance of the space is full, should we create another for overflow? If false, only 1 instance will ever be created, and users will be unable to join if that instance is full.")]
        public bool serverInstancingEnabled = true;

        [Tooltip("How many users can be in a single server of this space?")]
        public ServerCapacitySetting serverCapacitySetting = ServerCapacitySetting.Custom;

        [ToggleWithEnum("settings.serverCapacitySetting", (int)ServerCapacitySetting.Custom)]
        [Range(1, SpaceConfig.PLATFORM_MAX_CAPACITY), Tooltip("How many participants can be in a single server of this space?")]
        public int serverInstanceCapacity = 16; // Good default for most spaces
    }

    public enum ServerCapacitySetting
    {
        Maximum, // The maximum can change over time, so it's not a good idea to hardcode it
        Custom
    }
}
