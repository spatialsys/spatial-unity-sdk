using UnityEngine;
using System.Collections.Generic;

namespace SpatialSys.UnitySDK.Editor
{
    // TODO: use the same enum as the Spatial API
    public enum AvatarAnimationType
    {
        Emote = 0,
        Idle = 1,
        MovementSet = 2,
        JumpSet = 3,
        Sit = 4
    }

    public class AvatarAnimationConfig : PackageConfig
    {
        private const PackageType PACKAGE_TYPE = PackageType.AvatarAnimation;

        public SpatialAvatarAnimation prefab;
        public AvatarAnimationType type;

        public override PackageType packageType => PACKAGE_TYPE;
        public override Vector2Int thumbnailDimensions => new Vector2Int(512, 512);
        public override string bundleName => EditorUtility.GetAssetBundleName(prefab);
        public override string validatorID => GetValidatorID();
        public override Scope validatorUsageContext => Scope.Universal;

        public override IEnumerable<Object> assets
        {
            get
            {
                if (prefab != null)
                    yield return prefab;
            }
        }

        public static string GetValidatorID()
        {
            return PACKAGE_TYPE.ToString();
        }
    }
}
