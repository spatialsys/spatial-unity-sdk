using UnityEngine;
using UnityEditor;

namespace SpatialSys.UnitySDK.Editor
{
    public static class AvatarPackageTests
    {
        [PackageTest(PackageType.Avatar)]
        public static void EnsurePrefabIsAssigned(AvatarConfig config)
        {
            if (config.prefab == null)
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(config, TestResponseType.Fail, "A prefab with the Spatial Avatar component must be assigned in the config.")
                );
            }
        }

        [PackageTest(PackageType.Avatar)]
        public static void WarnIfAvatarCategoryIsUnspecified(AvatarConfig config)
        {
            if (config.category == AvatarConfig.Category.Unspecified)
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(
                        null,
                        TestResponseType.Warning,
                        "This avatar does not have a category specified",
                        "Some spaces may impose a \"dress code\" to restrict certain avatar categories from joining. " +
                            "This avatar won't be able to join these types of spaces at all if the category is unspecified. " +
                            "Select a category that best suits this avatar to fix this warning."
                    )
                );
            }
        }
    }
}
