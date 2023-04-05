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

        [PackageTest(PackageType.PrefabObject)]
        public static void EnsurePrefabHasColliders(PrefabObjectConfig config)
        {
            if (config.prefab == null)
                return;

            var colliders = config.prefab.GetComponentsInChildren<Collider>();

            if (colliders.Length == 0)
            {
                SpatialValidator.AddResponse(new SpatialTestResponse(
                    config,
                    TestResponseType.Fail,
                    "The prefab does not have any colliders attached",
                    "Users will not be able to move this object without colliders. Add any type of collider to fix this issue."
                ));
            }
        }
    }
}
