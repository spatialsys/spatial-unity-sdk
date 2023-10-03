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

        [PackageTest(PackageType.Space)]
        public static void EnsureCapacityDoesNotExceedMaximum(SpaceConfig config)
        {
            if (config.settings.serverCapacitySetting == ServerCapacitySetting.Custom)
            {
                if (config.settings.serverInstanceCapacity < 2 || config.settings.serverInstanceCapacity > SpaceConfig.PLATFORM_MAX_CAPACITY)
                {
                    SpatialValidator.AddResponse(
                        new SpatialTestResponse(
                            config,
                            TestResponseType.Fail,
                            "Invalid space maximum capacity",
                            $"The maximum capacity for a Space should be minimum 2 and cannot exceed {SpaceConfig.PLATFORM_MAX_CAPACITY}"
                        )
                    );
                }
            }
        }
    }
}
