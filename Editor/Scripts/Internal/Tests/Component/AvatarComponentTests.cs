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
                if (child.localScale != Vector3.one)
                {
                    string childTransformPath = child.GetHierarchyPath(separator: " -> ");
                    var resp = new SpatialTestResponse(
                        child,
                        TestResponseType.Fail,
                        $"The scale of all child GameObjects of {avatarPrefab.name} avatar must be normalized: {child.name}",
                        $"Make sure the transform scale of ({childTransformPath}) of the avatar is set to (1,1,1)."
                    );
                    // Resetting the scale can mess up the rig, mark as unsafe.
                    resp.SetAutoFix(isSafe: false, "Reset transform scale", (child) => {
                        var childTransform = (Transform)child;
                        childTransform.localScale = Vector3.one;
                    });
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
            if (avatarPrefab.TryGetComponent<Animator>(out animator))
            {
                if (!animator.hasTransformHierarchy)
                {
                    if (!Application.isBatchMode)
                    {
                        var resp = new SpatialTestResponse(
                            avatarPrefab.gameObject,
                            TestResponseType.Fail,
                            "The avatar must have transform hierarchy enabled. This can be enabled in the Animator component."
                        );
                        resp.SetAutoFix(true, "Disable Optimize GameObjects Setting", AutoFixAnimatorOptimizeGameObjects);
                        SpatialValidator.AddResponse(resp);
                    }
                    else
                    {
                        AutoFixAnimatorOptimizeGameObjects(avatarPrefab.gameObject);
                    }
                }
            }
        }

        private static void AutoFixAnimatorOptimizeGameObjects(Object gameObject)
        {
            Object sourcePrefab = PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
            var importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(sourcePrefab)) as ModelImporter;
            importer.optimizeGameObjects = false;
            importer.SaveAndReimport();
        }

        [ComponentTest(typeof(SpatialAvatar))]
        public static void EnsureAnimOverrridesLoop(SpatialAvatar avatarPrefab)
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
