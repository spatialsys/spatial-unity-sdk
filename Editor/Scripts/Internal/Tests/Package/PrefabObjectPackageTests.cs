using UnityEngine;

namespace SpatialSys.UnitySDK.Editor
{
    public static class PrefabObjectPackageTests
    {
        [PackageTest(PackageType.PrefabObject)]
        public static void EnsurePrefabIsAssigned(PrefabObjectConfig config)
        {
            if (config.prefab == null)
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(config, TestResponseType.Fail, "A prefab with the Prefab Object component must be assigned in the config.")
                );
            }
        }
    }
}
