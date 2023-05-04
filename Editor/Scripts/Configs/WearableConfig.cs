using UnityEngine;
using System.Collections.Generic;

namespace SpatialSys.UnitySDK.Editor
{
    public class WearableConfig : PackageConfig
    {
        private const PackageType PACKAGE_TYPE = PackageType.Wearable;

        public SpatialWearable prefab;
        public Scope usageContext = Scope.Universal; // AKA "wearable scope"

        public override PackageType packageType => PACKAGE_TYPE;
        public override Vector2Int thumbnailDimensions => new Vector2Int(512, 512);
        public override string bundleName => EditorUtility.GetAssetBundleName(prefab);
        public override string validatorID => prefab != null ? GetValidatorID(usageContext, prefab.type) : null;
        public override IEnumerable<Object> assets
        {
            get
            {
                if (prefab != null)
                    yield return prefab;
            }
        }

        public static string GetValidatorID(Scope usageContext, SpatialWearable.Type type)
        {
            return $"{PACKAGE_TYPE}_{usageContext}_{type}";
        }
    }
}
