using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.IO;

namespace SpatialSys.UnitySDK.Editor
{
    public static class AvatarPackageTests
    {
        private static readonly Dictionary<HumanBodyBones, Quaternion> _targetTPoseBoneRotations = new Dictionary<HumanBodyBones, Quaternion>()
        {
            { HumanBodyBones.Hips, new Quaternion(0f, 0f, 0f, 1f) },
            { HumanBodyBones.LeftUpperLeg, new Quaternion(0f, -0.0064f, 1f, 0.003f) },
            { HumanBodyBones.RightUpperLeg, new Quaternion(0f, -0.0063f, 1f, -0.003f) },
            { HumanBodyBones.LeftLowerLeg, new Quaternion(0.0001f, -0.0245f, 0.9997f, -0.0029f) },
            { HumanBodyBones.RightLowerLeg, new Quaternion(-0.0001f, -0.0245f, 0.9997f, 0.0029f) },
            { HumanBodyBones.LeftFoot, new Quaternion(0.0135f, 0.5199f, 0.8539f, 0.0221f) },
            { HumanBodyBones.RightFoot, new Quaternion(-0.0135f, 0.5199f, 0.8539f, -0.0221f) },
            { HumanBodyBones.Spine, new Quaternion(-0.0607f, 0f, 0f, 0.9982f) },
            { HumanBodyBones.Chest, new Quaternion(-0.0605f, 0f, 0f, 0.9982f) },
            { HumanBodyBones.Neck, new Quaternion(0f, 0f, 0f, 1f) },
            { HumanBodyBones.Head, new Quaternion(0f, 0f, 0f, 1f) },
            { HumanBodyBones.LeftShoulder, new Quaternion(0.4526f, -0.5433f, 0.5527f, 0.4411f) },
            { HumanBodyBones.RightShoulder, new Quaternion(-0.4526f, -0.5433f, 0.5527f, -0.4411f) },
            { HumanBodyBones.LeftUpperArm, new Quaternion(0.5f, -0.5f, 0.5f, 0.5f) },
            { HumanBodyBones.RightUpperArm, new Quaternion(0.5f, 0.5f, -0.5f, 0.5f) },
            { HumanBodyBones.LeftLowerArm, new Quaternion(0.5f, -0.5f, 0.5f, 0.5f) },
            { HumanBodyBones.RightLowerArm, new Quaternion(0.5f, 0.5f, -0.5f, 0.5f) },
            { HumanBodyBones.LeftHand, new Quaternion(0.5f, -0.5f, 0.5f, 0.5f) },
            { HumanBodyBones.RightHand, new Quaternion(0.5f, 0.5f, -0.5f, 0.5f) },
            { HumanBodyBones.LeftToes, new Quaternion(-0.0016f, 0.7008f, 0.7133f, -0.0116f) },
            { HumanBodyBones.RightToes, new Quaternion(0.0014f, 0.7008f, 0.7133f, 0.0113f) },
            { HumanBodyBones.LeftThumbProximal, new Quaternion(0.6281f, -0.1769f, 0.4133f, 0.6351f) },
            { HumanBodyBones.LeftThumbIntermediate, new Quaternion(0.6281f, -0.1769f, 0.4133f, 0.6351f) },
            { HumanBodyBones.LeftThumbDistal, new Quaternion(0.6281f, -0.1769f, 0.4133f, 0.6351f) },
            { HumanBodyBones.LeftIndexProximal, new Quaternion(0.5f, -0.5f, 0.5f, 0.5f) },
            { HumanBodyBones.LeftIndexIntermediate, new Quaternion(0.5f, -0.5f, 0.5f, 0.5f) },
            { HumanBodyBones.LeftIndexDistal, new Quaternion(0.5f, -0.5f, 0.5f, 0.5f) },
            { HumanBodyBones.LeftMiddleProximal, new Quaternion(0.5f, -0.5f, 0.5f, 0.5f) },
            { HumanBodyBones.LeftMiddleIntermediate, new Quaternion(0.5f, -0.5f, 0.5f, 0.5f) },
            { HumanBodyBones.LeftMiddleDistal, new Quaternion(0.5f, -0.5f, 0.5f, 0.5f) },
            { HumanBodyBones.LeftRingProximal, new Quaternion(0.5f, -0.5f, 0.5f, 0.5f) },
            { HumanBodyBones.LeftRingIntermediate, new Quaternion(0.5f, -0.5f, 0.5f, 0.5f) },
            { HumanBodyBones.LeftRingDistal, new Quaternion(0.5f, -0.5f, 0.5f, 0.5f) },
            { HumanBodyBones.LeftLittleProximal, new Quaternion(0.5f, -0.5f, 0.4999f, 0.5001f) },
            { HumanBodyBones.LeftLittleIntermediate, new Quaternion(-0.4999f, 0.5001f, -0.5001f, -0.4999f) },
            { HumanBodyBones.LeftLittleDistal, new Quaternion(0.5f, -0.5f, 0.5f, 0.5f) },
            { HumanBodyBones.RightThumbProximal, new Quaternion(0.6281f, 0.1769f, -0.4133f, 0.6351f) },
            { HumanBodyBones.RightThumbIntermediate, new Quaternion(0.6281f, 0.1769f, -0.4133f, 0.6351f) },
            { HumanBodyBones.RightThumbDistal, new Quaternion(0.6281f, 0.1769f, -0.4133f, 0.6351f) },
            { HumanBodyBones.RightIndexProximal, new Quaternion(0.5f, 0.5f, -0.5f, 0.5f) },
            { HumanBodyBones.RightIndexIntermediate, new Quaternion(0.5f, 0.5f, -0.5f, 0.5f) },
            { HumanBodyBones.RightIndexDistal, new Quaternion(0.5f, 0.5f, -0.5f, 0.5f) },
            { HumanBodyBones.RightMiddleProximal, new Quaternion(0.5f, 0.5f, -0.5f, 0.5f) },
            { HumanBodyBones.RightMiddleIntermediate, new Quaternion(0.5f, 0.5f, -0.5f, 0.5f) },
            { HumanBodyBones.RightMiddleDistal, new Quaternion(0.5f, 0.5f, -0.5f, 0.5f) },
            { HumanBodyBones.RightRingProximal, new Quaternion(0.5f, 0.5f, -0.5f, 0.5f) },
            { HumanBodyBones.RightRingIntermediate, new Quaternion(-0.5f, -0.5f, 0.5f, -0.5f) },
            { HumanBodyBones.RightRingDistal, new Quaternion(-0.5f, -0.5f, 0.5f, -0.5f) },
            { HumanBodyBones.RightLittleProximal, new Quaternion(0.5f, 0.5f, -0.5f, 0.5f) },
            { HumanBodyBones.RightLittleIntermediate, new Quaternion(-0.5f, -0.5f, 0.5f, -0.5f) },
            { HumanBodyBones.RightLittleDistal, new Quaternion(0.5f, 0.5f, -0.5f, 0.5f) },
            { HumanBodyBones.UpperChest, new Quaternion(-0.0028f, 0f, 0f, 1f) },
        };

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

        /// <summary>
        /// Totals up vertices, triangles, and sub-meshes for all meshes from the avatar, and make sure none of them exceed the limit.
        /// </summary>
        [PackageTest(PackageType.Avatar)]
        public static void EnsureAvatarMeshesMeetGuidelines(AvatarConfig config)
        {
            if (config.prefab == null)
                return;

            ValidationUtility.EnsureObjectMeshesMeetGuidelines(config.prefab,
                vertexCountLimit: (config.usageContext == AvatarConfig.Scope.Universal) ? 50000 : 200000,
                triangleCountLimit: (config.usageContext == AvatarConfig.Scope.Universal) ? 22500 : 200000,
                subMeshCountLimit: (config.usageContext == AvatarConfig.Scope.Universal) ? 4 : 100,
                boundsSizeMinLimit: 0.1f,
                boundsSizeMaxLimit: (config.usageContext == AvatarConfig.Scope.Universal) ? 2.5f : 25f
            );
        }

        /// <summary>
        /// Checks each texture dependency associated with the avatar prefab.
        /// </summary>
        [PackageTest(PackageType.Avatar)]
        public static void EnsureAvatarTexturesMeetGuidelines(AvatarConfig config)
        {
            if (config.prefab == null)
                return;

            int textureSizeLimit = (config.usageContext == AvatarConfig.Scope.Universal) ? 1024 : 4096;
            Object[] assetDeps = UnityEditor.EditorUtility.CollectDependencies(new Object[] { config.prefab });

            foreach (Object asset in assetDeps)
            {
                if (asset is Texture textureAsset)
                {
                    if (textureAsset.width > textureSizeLimit || textureAsset.height > textureSizeLimit)
                    {
                        SpatialValidator.AddResponse(
                            new SpatialTestResponse(
                                textureAsset,
                                TestResponseType.Fail,
                                $"A texture on the avatar is too large ({textureAsset.width}x{textureAsset.height}). The dimensions must not exceed {textureSizeLimit}x{textureSizeLimit}.",
                                "Reducing the texture size will reduce strain on memory and support more devices. Reduce the size of the texture through the import settings to comply with this guideline."
                            )
                        );
                    }
                }
            }
        }

        /// <summary>
        /// If the avatar is intended for Universal usage context, ensure there are no scripting-related components attached.
        /// </summary>
        [PackageTest(PackageType.Avatar)]
        public static void EnsureUniversalAvatarHasNoScriptingComponents(AvatarConfig config)
        {
            if (config.usageContext != AvatarConfig.Scope.Universal)
                return;

            if (config.prefab == null)
                return;

            LudiqBehaviour[] scriptingComponents = config.prefab.GetComponentsInChildren<LudiqBehaviour>();
            if (scriptingComponents.Length > 0)
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(
                        scriptingComponents[0],
                        TestResponseType.Fail,
                        "Visual Scripting is not allowed on avatars intended for a Universal usage context",
                        "This restriction is to keep functionality consistent across all spaces. " +
                            "You may learn more about these restrictions and avatar usage contexts by reading the Avatar packages documentation. " +
                            $"Remove the following components to fix this issue:\n{EditorUtility.GetComponentNamesWithInstanceCountString(scriptingComponents)}"
                    )
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

        /// <summary>
        /// Standardize avatar bone orientations.
        /// </summary>
        public static void EnforceValidBoneOrientations(SpatialAvatar avatarPrefab)
        {
            // Create folder next to the prefab to store newly generated avatar resources
            string prefabPath = AssetDatabase.GetAssetPath(avatarPrefab);
            string generatedAssetsDirPath = Path.Combine(Path.GetDirectoryName(prefabPath), "SpatialAvatarAssets");
            if (AssetDatabase.IsValidFolder(generatedAssetsDirPath))
                AssetDatabase.DeleteAsset(generatedAssetsDirPath);
            Directory.CreateDirectory(generatedAssetsDirPath);

            // Create new preview scene for avatar prefab to use animations without affecting any other open scenes
            Scene previewScene = EditorSceneManager.NewPreviewScene();
            SpatialAvatar avatarInstance = (SpatialAvatar)PrefabUtility.InstantiatePrefab(avatarPrefab, previewScene);
            GameObject avatarObj = avatarInstance.gameObject;
            SkinnedMeshRenderer[] renderers = avatarObj.GetComponentsInChildren<SkinnedMeshRenderer>();
            Animator animator = avatarObj.GetComponent<Animator>();

            // Force avatar into T-pose through animation
            // Since avatars can be in different default poses, we need to force it into a generic T-pose to correctly 
            // Apply the bone rotations relative to a target T-pose.
            RuntimeAnimatorController animatorController = (RuntimeAnimatorController)AssetDatabase.LoadAssetAtPath("Packages/io.spatial.unitysdk/Editor/Animator/processAvatar.controller", typeof(RuntimeAnimatorController));
            if (animatorController == null)
                throw new System.Exception("Internal error: Could not find processAvatar.controller");

            animator.runtimeAnimatorController = animatorController;
            var prevCullingMode = animator.cullingMode;
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            animator.Update(0f);

            // Cache original bone rotations
            Dictionary<HumanBodyBones, Quaternion> origBoneRotation = new Dictionary<HumanBodyBones, Quaternion>();
            foreach (HumanBodyBones bone in _targetTPoseBoneRotations.Keys)
            {
                Transform avatarBone = animator.GetBoneTransform(bone);
                if (avatarBone != null)
                    origBoneRotation[bone] = avatarBone.rotation;
            }

            // Store the original renderer bounds
            Dictionary<SkinnedMeshRenderer, Bounds> origRendererBounds = new Dictionary<SkinnedMeshRenderer, Bounds>();
            foreach (SkinnedMeshRenderer renderer in renderers)
                origRendererBounds[renderer] = renderer.bounds;

            // Update each bone rotation of the avatar to match the target t-pose
            // Rebind each mesh of the avatar to the updated bone rotations
            foreach (SkinnedMeshRenderer renderer in renderers)
            {
                Mesh mesh = Mesh.Instantiate(renderer.sharedMesh);
                Transform[] rendererBones = renderer.bones;
                Matrix4x4[] bindPoses = renderer.sharedMesh.bindposes;

                foreach ((HumanBodyBones bone, Quaternion targetRotation) in _targetTPoseBoneRotations)
                {
                    // Not all avatars will have all the bones in the target T-pose and that is expected
                    // We match the bones that exist to the target T-pose and ignore the ones that don't exist
                    Transform avatarBone = animator.GetBoneTransform(bone);
                    if (avatarBone == null)
                        continue;

                    for (int i = 0; i < rendererBones.Length; i++)
                    {
                        // If we find the bone's corresponding renderer bone index, update the bone and bindpose
                        if (rendererBones[i] == avatarBone)
                        {
                            // Save original world positions of child bones
                            Dictionary<Transform, Vector3> origChildWorldPositions = new Dictionary<Transform, Vector3>();
                            foreach (Transform child in rendererBones[i])
                                origChildWorldPositions[child] = child.position;

                            // Calculate new bindpose
                            Quaternion origToTPoseRotationOffset = Quaternion.Inverse(origBoneRotation[bone]) * targetRotation; // calculate rotation diff
                            Matrix4x4 origBoneWorldToLocal = bindPoses[i] * renderer.transform.worldToLocalMatrix; // undo the renderer matrix; matrix is in world space
                            Matrix4x4 origLocalToWorld = origBoneWorldToLocal.inverse; // matrix is now in local space
                            Matrix4x4 newLocalToWorld = origLocalToWorld * Matrix4x4.Rotate(origToTPoseRotationOffset); // apply the rotation diff in local space
                            Matrix4x4 newBoneWorldToLocal = newLocalToWorld.inverse;
                            bindPoses[i] = newBoneWorldToLocal * renderer.transform.localToWorldMatrix;

                            // Update bone rotation
                            Quaternion boneRotationOffset = Quaternion.Inverse(rendererBones[i].rotation) * targetRotation;
                            rendererBones[i].rotation = targetRotation;

                            // Rotate child bone positions back to original position
                            foreach (var (child, oldPosition) in origChildWorldPositions)
                            {
                                child.localRotation = Quaternion.Inverse(boneRotationOffset) * child.localRotation;
                                child.position = oldPosition;
                            }

                            break;
                        }
                    }
                }

                mesh.bindposes = bindPoses;
                renderer.sharedMesh = mesh;
                AssetDatabase.CreateAsset(mesh, Path.Combine(generatedAssetsDirPath, mesh.name + ".mesh"));
            }

            // Create avatar with new skeleton
            HumanDescription description = animator.avatar.humanDescription;
            description.skeleton = CreateSkeleton(avatarObj);
            Avatar avatar = AvatarBuilder.BuildHumanAvatar(avatarObj, description);
            avatar.name = animator.avatar.name;

            if (!avatar.isValid || !avatar.isHuman)
                throw new System.Exception("Avatar is not using a valid humanoid rig");

            animator.avatar = avatar;
            AssetDatabase.CreateAsset(avatar, Path.Combine(generatedAssetsDirPath, avatar.name + ".asset"));

            // Force back into T-pose after updating the avatar
            animator.Update(0f);
            animator.cullingMode = prevCullingMode;

            // Correct renderer bounds based on original bounds in world space
            foreach ((SkinnedMeshRenderer renderer, Bounds bounds) in origRendererBounds)
                renderer.localBounds = InverseTransformBounds(renderer.rootBone, bounds);

            // Save processed avatar prefab
            PrefabUtility.ApplyPrefabInstance(avatarObj, InteractionMode.AutomatedAction);
            EditorSceneManager.ClosePreviewScene(previewScene);
        }

        private static SkeletonBone[] CreateSkeleton(GameObject avatarRoot)
        {
            List<SkeletonBone> skeleton = new List<SkeletonBone>();

            Transform[] avatarTransforms = avatarRoot.GetComponentsInChildren<Transform>();
            foreach (Transform avatarTransform in avatarTransforms)
            {
                skeleton.Add(new SkeletonBone() {
                    name = avatarTransform.name,
                    position = avatarTransform.localPosition,
                    rotation = avatarTransform.localRotation,
                    scale = avatarTransform.localScale
                });
            }
            return skeleton.ToArray();
        }

        private static Bounds InverseTransformBounds(Transform parent, Bounds bounds)
        {
            var center = parent.InverseTransformPoint(bounds.center);
            var points = GetBoundsCorners(bounds);

            var result = new Bounds(center, Vector3.zero);
            foreach (var point in points)
                result.Encapsulate(parent.InverseTransformPoint(point));

            return result;
        }

        private static Vector3[] GetBoundsCorners(Bounds bounds)
        {
            Vector3 min = bounds.min;
            Vector3 max = bounds.max;
            return new Vector3[8] {
                new Vector3(min.x, min.y, min.z),
                new Vector3(max.x, min.y, min.z),
                new Vector3(min.x, max.y, min.z),
                new Vector3(min.x, min.y, max.z),
                new Vector3(min.x, max.y, max.z),
                new Vector3(max.x, max.y, min.z),
                new Vector3(max.x, min.y, max.z),
                new Vector3(max.x, max.y, max.z),
            };
        }
    }
}
