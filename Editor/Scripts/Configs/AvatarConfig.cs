using UnityEngine;
using System.Collections.Generic;

namespace SpatialSys.UnitySDK.Editor
{
    public class AvatarConfig : PackageConfig
    {
        public enum UsageContext
        {
            Global,
            Ecosystem
        }

        public SpatialAvatar prefab;
        public UsageContext usageContext = UsageContext.Global;

        public override PackageType packageType => PackageType.Avatar;
        public override Vector2Int thumbnailDimensions => new Vector2Int(512, 512);
        public override string bundleName => EditorUtility.GetAssetBundleName(prefab);
        public override IEnumerable<Object> assets
        {
            get
            {
                if (prefab != null)
                    yield return prefab;
            }
        }
    }
}
