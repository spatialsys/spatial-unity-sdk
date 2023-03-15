using UnityEngine;

namespace SpatialSys.UnitySDK.Editor
{
    public static class AvatarComponentTests
    {
        /// <summary>
        /// Checks that there's an animator component on the prefab, and the animator has a valid humanoid rig.
        /// </summary>
        [ComponentTest(typeof(SpatialAvatar))]
        public static void EnsureAnimatorRigIsHumanoid(SpatialAvatar avatarPrefab)
        {
            Animator animator;
            if (!avatarPrefab.TryGetComponent<Animator>(out animator))
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(avatarPrefab, TestResponseType.Fail, "The prefab does not have an animator attached to it.")
                );
                return;
            }

            if (animator.avatar == null || !animator.avatar.isHuman)
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(avatarPrefab, TestResponseType.Fail, "The avatar must have a valid humanoid rig. Non-humanoid rigs are not supported.")
                );
            }
        }

        /// <summary>
        /// Checks that the animator has transform hiearchy enabled.
        /// </summary>
        [ComponentTest(typeof(SpatialAvatar))]
        public static void EnsureAnimatorRigHasTransformHierarchy(SpatialAvatar avatarPrefab)
        {
            Animator animator;
            if (avatarPrefab.TryGetComponent<Animator>(out animator))
            {
                if (!animator.hasTransformHierarchy)
                {
                    SpatialValidator.AddResponse(
                        new SpatialTestResponse(avatarPrefab, TestResponseType.Fail, "The avatar must have exposed transform hierarchy. Disable \"Optimize Game Objects\" in the model's Rig import settings.")
                    );
                }
            }
        }
    }
}
