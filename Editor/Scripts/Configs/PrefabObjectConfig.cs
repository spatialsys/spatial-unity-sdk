using UnityEditor;
using UnityEngine;

namespace SpatialSys.UnitySDK.Editor
{
    public class PrefabObjectConfig : PackageConfig
    {
        public SpatialPrefabObject prefab;

        public override PackageType packageType => PackageType.PrefabObject;
        public override Vector2Int thumbnailDimensions => new Vector2Int(512, 512);
        public override string bundleName => EditorUtility.GetAssetBundleName(prefab);

        public override bool IsMainAssetForPackage(Object asset)
        {
            return asset == prefab;
        }
    }
}
