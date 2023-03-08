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

        [PackageTest(PackageType.AvatarAnimation)]
        public static void WarnPrefabShouldHaveNoChildren(AvatarAnimationConfig config)
        {
            if (config.prefab != null && config.prefab.transform.childCount > 0)
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(
                        config.prefab,
                        TestResponseType.Warning,
                        "The prefab should not have any children attached",
                        $"There are {config.prefab.transform.childCount} child object(s) parented to this prefab. " +
                            "Remove all child objects from the prefab to fix this issue. This will help reduce the potential size of the asset and make it load faster."
                    )
                );
            }
        }
    }
}
