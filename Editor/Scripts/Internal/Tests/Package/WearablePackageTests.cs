using UnityEngine;

namespace SpatialSys.UnitySDK.Editor
{
    public static class WearablePackageTests
    {
        [PackageTest(PackageType.Wearable)]
        public static void EnsurePrefabIsAssigned(WearableConfig config)
        {
            if (config.prefab == null)
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(config, TestResponseType.Fail, "A prefab with the Spatial Wearable component must be assigned in the config.")
                );
            }
        }
    }
}
