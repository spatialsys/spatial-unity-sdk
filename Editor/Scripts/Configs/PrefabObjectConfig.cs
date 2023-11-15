using UnityEngine;
using System.Collections.Generic;

namespace SpatialSys.UnitySDK.Editor
{
    public class PrefabObjectConfig : PackageConfig
    {
        private const PackageType PACKAGE_TYPE = PackageType.PrefabObject;

        public SpatialPrefabObject prefab;

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
