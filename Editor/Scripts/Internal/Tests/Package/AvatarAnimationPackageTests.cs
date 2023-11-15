using UnityEngine;

namespace SpatialSys.UnitySDK.Editor
{
    public static class AvatarAnimationPackageTests
    {
        [PackageTest(PackageType.AvatarAnimation)]
        public static void EnsurePrefabIsAssigned(AvatarAnimationConfig config)
        {
            if (config.prefab == null)
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(config, TestResponseType.Fail, "A prefab with the Spatial Avatar Animation component must be assigned in the config")
                );
            }
        }
    }
}
