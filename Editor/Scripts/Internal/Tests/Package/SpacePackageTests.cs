using System.Collections.Generic;
using UnityEditor;
using SpatialSys.UnitySDK;

namespace SpatialSys.UnitySDK.Editor
{
    public static class SpacePackageTests
    {
        [PackageTest(PackageType.Space)]
        public static void EnsureSceneIsAssigned(SpaceConfig config)
        {
            if (config.scene == null)
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(
                        config,
                        TestResponseType.Fail,
                        $"There is no scene assigned to the SpaceConfig"
                    )
                );
            }
        }
    }
}
