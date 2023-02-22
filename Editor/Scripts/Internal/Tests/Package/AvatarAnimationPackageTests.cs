using UnityEngine;

namespace SpatialSys.UnitySDK.Editor
{
    public static class AvatarAnimationPackageTests
    {
        /// <summary>
        /// Checks that the prefab is assigned in config, the prefab has a target animation clip assigned, and the clip has humanoid motion.
        /// </summary>
        [PackageTest(PackageType.AvatarAnimation)]
        public static void EnsureAnimationHasHumanoidMotion(AvatarAnimationConfig config)
        {
            if (config.prefab == null)
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(config, TestResponseType.Fail, "A prefab with the Spatial Avatar Animation component must be assigned in the config")
                );
                return;
            }

            if (config.prefab.targetClip == null)
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(
                        config.prefab,
                        TestResponseType.Fail,
                        "The avatar animation prefab does not have an animation clip assigned",
                        "Assign an animation clip under your imported model asset to the prefab to fix this issue."
                    )
                );
                return;
            }

            if (!config.prefab.targetClip.humanMotion)
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(
                        config.prefab.targetClip,
                        TestResponseType.Fail,
                        "The animation clip is not compatible with a humanoid rig/avatar",
                        "Go to the model that this animation belongs to and switch the rig type to 'Humanoid' to fix this issue."
                    )
                );
            }
        }
    }
}
