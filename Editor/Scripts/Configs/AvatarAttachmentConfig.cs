using UnityEngine;
using System.Collections.Generic;

namespace SpatialSys.UnitySDK.Editor
{
    public class AvatarAttachmentConfig : PackageConfig
    {
        private const PackageType PACKAGE_TYPE = PackageType.AvatarAttachment;

        public SpatialAvatarAttachment prefab;
        public Scope usageContext = Scope.World;

        public override PackageType packageType => PACKAGE_TYPE;
        public override Vector2Int thumbnailDimensions => new Vector2Int(512, 512);
        public override string bundleName => EditorUtility.GetAssetBundleName(prefab);
        public override string validatorID => prefab != null ? GetValidatorID(usageContext, prefab.primarySlot) : null;
        public override Scope validatorUsageContext => usageContext;

        public override IEnumerable<Object> assets
        {
            get
            {
                if (prefab != null)
                    yield return prefab;
            }
        }

        public static string GetValidatorID(Scope usageContext, SpatialAvatarAttachment.Slot primarySlot)
        {
            if (primarySlot == SpatialAvatarAttachment.Slot.Aura)
                return $"{PACKAGE_TYPE}_{usageContext}_{primarySlot}";

            return $"{PACKAGE_TYPE}_{usageContext}";
        }
    }
}
