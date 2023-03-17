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
            Object sourcePrefab = UnityEditor.PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
            var importer = UnityEditor.AssetImporter.GetAtPath(UnityEditor.AssetDatabase.GetAssetPath(sourcePrefab)) as UnityEditor.ModelImporter;
            importer.optimizeGameObjects = false;
            importer.SaveAndReimport();
        }
    }
}
