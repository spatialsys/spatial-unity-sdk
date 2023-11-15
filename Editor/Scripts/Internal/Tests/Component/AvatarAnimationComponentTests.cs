using UnityEngine;

namespace SpatialSys.UnitySDK.Editor
{
    public static class AvatarAnimationComponentTests
    {
        [ComponentTest(typeof(SpatialAvatarAnimation))]
        public static void WarnPrefabShouldHaveNoChildren(SpatialAvatarAnimation avatarPrefab)
        {
            if (avatarPrefab.transform.childCount <= 0)
                return;

            SpatialValidator.AddResponse(
                new SpatialTestResponse(
                    avatarPrefab,
                    TestResponseType.Warning,
                    "The prefab should not have any children attached",
                    $"There are {avatarPrefab.transform.childCount} child object(s) parented to this prefab. " +
                        "Remove all child objects from the prefab to fix this issue. This will help reduce the size of the asset and make it load faster."
                )
            );
        }

        /// <summary>
        /// Checks that the prefab has a target animation clip assigned, and the clip has humanoid motion.
        /// </summary>
        [ComponentTest(typeof(SpatialAvatarAnimation))]
        public static void EnsureAnimationHasHumanoidMotion(SpatialAvatarAnimation avatarPrefab)
        {
            if (avatarPrefab.targetClip == null)
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(
                        avatarPrefab,
                        TestResponseType.Fail,
                        "The prefab does not have an animation clip assigned",
                        "Assign an animation clip under your imported model asset to the prefab to fix this issue."
                    )
                );
                return;
            }

            if (!avatarPrefab.targetClip.humanMotion)
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(
                        avatarPrefab.targetClip,
                        TestResponseType.Fail,
                        "The animation clip is not compatible with a humanoid rig/avatar",
                        "Go to the model that this animation belongs to and switch the rig type to 'Humanoid' to fix this issue."
                    )
                );
            }
        }
    }
}
