using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SpatialSys.UnitySDK.Editor
{
    public class SpaceConfig : PackageConfig
    {
        [HideInInspector] public string worldID;

        // "Space Name" would be derived from PackageConfig.packageName
        public SceneAsset scene = null;

        public override PackageType packageType => PackageType.Space;
        public override Vector2Int thumbnailDimensions => new Vector2Int(1024, 512);
        public override string bundleName => EditorUtility.GetAssetBundleName(scene);

        public override bool allowTransparentThumbnails => false;

        public override IEnumerable<Object> assets
        {
            get
            {
                if (scene != null)
                    yield return scene;
            }
        }
    }
}
