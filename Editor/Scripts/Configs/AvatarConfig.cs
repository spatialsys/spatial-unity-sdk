using UnityEngine;
using System.Collections.Generic;

namespace SpatialSys.UnitySDK.Editor
{
    public class AvatarConfig : PackageConfig
    {
        public enum Scope
        {
            Global,
            Ecosystem // AKA "World"
        }

        public enum Category
        {
            /// <summary>
            /// Not allowed to join any space with a "dress code". Validation will warn if the avatar category is not specified.
            /// </summary>
            Unspecified = 0,
            Human,
            Fantasy,
            Robotic,
            Animal,
            Abstract
        }

        public SpatialAvatar prefab;
        public Scope usageContext = Scope.Global; // AKA "avatar scope"
        public Category category = Category.Unspecified;

        public override PackageType packageType => PackageType.Avatar;
        public override Vector2Int thumbnailDimensions => new Vector2Int(256, 256);
        public override string bundleName => EditorUtility.GetAssetBundleName(prefab);

        public override bool allowOpaqueThumbnails => false;
        public override float thumbnailMinTransparentBgRatio => 0.25f; // at least 25% of the thumbnail should be transparent

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
