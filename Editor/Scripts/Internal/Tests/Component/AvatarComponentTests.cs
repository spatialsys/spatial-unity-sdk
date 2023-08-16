using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SpatialSys.UnitySDK.Editor
{
    public static class AvatarComponentTests
    {
        /// <summary>
        /// Checks that the avatar has identity transform and the scale of each child gameobject is normalized.
        /// </summary>
        [ComponentTest(typeof(SpatialAvatar))]
        public static void EnsureTransformsMeetGuidelines(SpatialAvatar avatarPrefab)
        {
            Transform transform = avatarPrefab.transform;
            if (!transform.IsIdentity())
            {
                var resp = new SpatialTestResponse(
                    avatarPrefab,
                    TestResponseType.Fail,
                    "The avatar must have an identity transform",
                    "Make sure the avatar's transform position is set to (0,0,0), rotation is set to (0,0,0), and scale is set to (1,1,1)."
                );
                resp.SetAutoFix(true, "Reset transform", (component) => {
                    Transform prefabTransform = ((SpatialAvatar)component).transform;
                    prefabTransform.localPosition = Vector3.zero;
                    prefabTransform.localRotation = Quaternion.identity;
                    prefabTransform.localScale = Vector3.one;
                });
                SpatialValidator.AddResponse(resp);
            }

            foreach (Transform child in transform.GetComponentsInChildren<Transform>())
            {
                if (!child.localScale.WithinThreshold(Vector3.one, distanceThreshold: 0.0001f)) // <= 0.01% scale difference is negligible.
                {
                    string childTransformPath = child.GetHierarchyPath(separator: " -> ");
                    var resp = new SpatialTestResponse(
                        child,
                        TestResponseType.Fail,
                        $"The scale of all child GameObjects of {avatarPrefab.name} avatar must be normalized: {child.name}",
                        $"Make sure the transform scale of ({childTransformPath}) of the avatar is set to (1,1,1)."
                    );
                    SpatialValidator.AddResponse(resp);
                }
            }
        }

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

            // Cannot automatically fix this, since prefab will still have no transform heirarchy generated. The avatar prefab needs to be recreated.
            if (avatarPrefab.TryGetComponent<Animator>(out animator) && !animator.hasTransformHierarchy)
            {
                SpatialValidator.AddResponse(new SpatialTestResponse(
                    avatarPrefab.gameObject,
                    TestResponseType.Fail,
                    "The avatar must have a transform hierarchy",
                    "This can be enabled in the Animator component by disabling the 'Optimize Game Objects' setting under the Rig tab of the model importer. " +
                        "Afterwards, you will need to recreate the avatar prefab from the model and ensure that there is a transform hierarchy inside the prefab representing the bone structure."
                ));
            }
        }

        [ComponentTest(typeof(SpatialAvatar))]
        public static void EnsureAnimOverridesLoop(SpatialAvatar avatarPrefab)
        {
            // All these animations need to loop to work properly
            var animsThatShouldLoop = new AnimationClip[] {
                avatarPrefab.animOverrides.idle,
                avatarPrefab.animOverrides.walk,
                avatarPrefab.animOverrides.jog,
                avatarPrefab.animOverrides.run,
                avatarPrefab.animOverrides.jumpInAir,
                avatarPrefab.animOverrides.fall,
                avatarPrefab.animOverrides.sit,
                avatarPrefab.animOverrides.climbIdle,
                avatarPrefab.animOverrides.climbUp,
            };

            AnimationClip[] animsThatFailedTest = animsThatShouldLoop.Where(anim => anim != null && !anim.isLooping).ToArray();
            if (animsThatFailedTest.Length > 0)
            {
                SpatialValidator.AddResponse(new SpatialTestResponse(
                    animsThatFailedTest[0],
                    TestResponseType.Fail,
                    "Some avatar animation override clips are not set to loop",
                    "The following animation clips must be set to loop: " + string.Join(", ", animsThatFailedTest.Select(anim => anim.name))
                        + "\nLook for the 'Loop Time' setting inside the animation clip inspector, or the model importer settings if the animation is imported from a model."
                ));
            }
        }
    }
}
