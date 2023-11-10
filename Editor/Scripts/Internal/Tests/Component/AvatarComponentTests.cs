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

        /// <summary>
        /// If there is a ragdoll setup on the avatar, then ensure all required components are properly configured.
        /// Any properties that can automatically corrected and set should be done under ValidationUtility.EnforceValidAvatarRagdollSetup(), which occurs during publish.
        /// </summary>
        [ComponentTest(typeof(SpatialAvatar))]
        public static void EnsureAvatarRagdollSetupIsValid(SpatialAvatar avatarPrefab)
        {
            // The existence of at least one joint will denote that the avatar has a ragdoll setup.
            CharacterJoint[] joints = avatarPrefab.GetComponentsInChildren<CharacterJoint>(includeInactive: true);
            if (joints == null || joints.Length == 0)
                return;

            static void AddMisconfiguredPhysicsFailResponse(GameObject go)
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(
                        go,
                        TestResponseType.Fail,
                        $"The object {go.name} in the avatar ragdoll setup is misconfigured",
                        "There should be a rigidbody and collider attached to this object to properly simulate ragdoll physics."
                    )
                );
            }

            const string BASE_RIGIDBODY_DETAILS = "The 'base rigidbody' is the central point of the ragdoll that other limbs connect to. " +
                "There should be exactly 1 rigidbody in the ragdoll that does not have a character joint, and at least 1 other joint which connects to that rigidbody. " +
                "The base rigidbody is typically the Hips bone in a humanoid rig.";

            // There should be exactly one rigidbody in the setup that has no character joint on the same object, which will serve as the base/root of the ragdoll (typically the hips).
            Rigidbody baseRigidbody = null;
            for (int i = 0; i < joints.Length; i++)
            {
                CharacterJoint joint = joints[i];

                if (joint.GetComponent<Rigidbody>() == null && joint.GetComponent<Collider>() == null)
                {
                    AddMisconfiguredPhysicsFailResponse(joint.gameObject);
                }

                if (joint.connectedBody == null)
                {
                    SpatialValidator.AddResponse(
                        new SpatialTestResponse(
                            avatarPrefab,
                            TestResponseType.Fail,
                            $"The avatar ragdoll joint on '{joint.name}' is misconfigured",
                            "The character joint must have a rigidbody assigned to the 'Connected Body' field"
                        )
                    );
                    continue;
                }

                // Check for and assign base rigidbody in ragdoll setup
                if (joint.connectedBody.GetComponent<CharacterJoint>() == null)
                {
                    if (baseRigidbody != null && baseRigidbody != joint.connectedBody)
                    {
                        SpatialValidator.AddResponse(
                            new SpatialTestResponse(
                                joint.connectedBody,
                                TestResponseType.Fail,
                                "Ragdoll setup has multiple base rigidbodies",
                                BASE_RIGIDBODY_DETAILS + $"\n\nCurrent Base Rigidbody: {baseRigidbody.name}\nFound: {joint.connectedBody.name}"
                            )
                        );
                    }
                    else
                    {
                        baseRigidbody = joint.connectedBody;
                    }
                }
            }

            if (baseRigidbody == null)
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(
                        avatarPrefab,
                        TestResponseType.Fail,
                        "Ragdoll setup has no base rigidbody",
                        BASE_RIGIDBODY_DETAILS
                    )
                );
            }
            else
            {
                if (baseRigidbody.GetComponent<Collider>() == null)
                    AddMisconfiguredPhysicsFailResponse(baseRigidbody.gameObject);
            }
        }
    }
}
