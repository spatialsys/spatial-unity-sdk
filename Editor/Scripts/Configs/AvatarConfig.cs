using UnityEngine;
using System.Collections.Generic;

namespace SpatialSys.UnitySDK.Editor
{
    public class AvatarConfig : PackageConfig
    {
        private const PackageType PACKAGE_TYPE = PackageType.Avatar;

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
        public Scope usageContext = Scope.Universal; // AKA "scope"
        public Category category = Category.Unspecified;

        public override PackageType packageType => PACKAGE_TYPE;
        public override Vector2Int thumbnailDimensions => new Vector2Int(256, 256);
        public override string bundleName => EditorUtility.GetAssetBundleName(prefab);
        public override string validatorID => GetValidatorID(usageContext);

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

        public static string GetValidatorID(Scope usageContext)
        {
            return $"{PACKAGE_TYPE}_{usageContext}";
        }
    }
}
