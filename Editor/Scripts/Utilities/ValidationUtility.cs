using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Profiling;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Reflection;

namespace SpatialSys.UnitySDK.Editor
{
    public static class ValidationUtility
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

        /// <summary>
        /// Standardize bone orientations.
        /// </summary>
        public static void ProcessPackagePrefab(SpatialPackageAsset packagePrefab)
        {
            // Create folder next to the prefab to store newly generated package resources
            string prefabPath = AssetDatabase.GetAssetPath(packagePrefab);
            string generatedAssetsDirPath = Path.Combine(Path.GetDirectoryName(prefabPath), Path.GetFileNameWithoutExtension(prefabPath) + "_GeneratedAssets");
            if (!AssetDatabase.IsValidFolder(generatedAssetsDirPath))
                Directory.CreateDirectory(generatedAssetsDirPath);

            // Create new preview scene to use animations without affecting any other open scenes
            Scene previewScene = EditorSceneManager.NewPreviewScene();
            try
            {
                SpatialPackageAsset packageInstance = (SpatialPackageAsset)PrefabUtility.InstantiatePrefab(packagePrefab, previewScene);

                // Makes a copy of the animation clip and saves it to the generated assets folder
                UniquefyAllAvatarAnimationClips(generatedAssetsDirPath, packageInstance.gameObject);

                if (packageInstance is SpatialAvatar avatarPrefabInstance)
                    EnforceValidAvatarRagdollSetup(avatarPrefabInstance);

                // Enforce valid bone orientations for avatar-based prefabs
                if (packagePrefab is SpatialAvatar || (packagePrefab is SpatialAvatarAttachment attachment && attachment.isSkinnedToHumanoidSkeleton))
                    EnforceValidBoneOrientations(generatedAssetsDirPath, packageInstance.gameObject);

                // Save processed prefab
                UnityEditor.EditorUtility.SetDirty(packageInstance);
                PrefabUtility.ApplyPrefabInstance(packageInstance.gameObject, InteractionMode.AutomatedAction);
            }
            finally
            {
                EditorSceneManager.ClosePreviewScene(previewScene);
            }
        }

        private static void EnforceValidBoneOrientations(string generatedAssetsDirPath, GameObject packagePrefabObj)
        {
            // Only continue if avatar or skinned attachment
            SkinnedMeshRenderer[] renderers = packagePrefabObj.GetComponentsInChildren<SkinnedMeshRenderer>();
            Animator animator = packagePrefabObj.GetComponent<Animator>();

            // Force into T-pose through animation
            // Since avatars can be in different default poses, we need to force it into a generic T-pose to correctly apply the bone rotations relative to a target T-pose.
            var tPoseAnimClip = EditorUtility.LoadAssetFromPackagePath<AnimationClip>("Editor/Animator/T-Pose.anim");
            if (tPoseAnimClip == null)
                throw new System.Exception("Internal error: Could not find T-Pose.anim");

            animator.runtimeAnimatorController = null;
            tPoseAnimClip.SampleAnimation(packagePrefabObj, time: 0f);

            // Cache original bone rotations
            Dictionary<HumanBodyBones, Quaternion> origBoneRotation = new Dictionary<HumanBodyBones, Quaternion>();
            foreach (HumanBodyBones targetBone in _targetTPoseBoneRotations.Keys)
            {
                Transform bone = animator.GetBoneTransform(targetBone);
                if (bone != null)
                    origBoneRotation[targetBone] = bone.rotation;
            }

            // Store the original renderer bounds
            Dictionary<SkinnedMeshRenderer, Bounds> origRendererBounds = new Dictionary<SkinnedMeshRenderer, Bounds>();
            foreach (SkinnedMeshRenderer renderer in renderers)
                origRendererBounds[renderer] = renderer.bounds;

            // Update each bone rotation to match the target t-pose
            // Rebind each mesh of the package to the updated bone rotations
            foreach (SkinnedMeshRenderer renderer in renderers)
            {
                Mesh mesh = Mesh.Instantiate(renderer.sharedMesh);
                Transform[] rendererBones = renderer.bones;
                Matrix4x4[] bindPoses = renderer.sharedMesh.bindposes;

                foreach ((HumanBodyBones targetBone, Quaternion targetRotation) in _targetTPoseBoneRotations)
                {
                    // Not all avatars/attachments will have all the bones in the target T-pose and that is expected
                    // We match the bones that exist to the target T-pose and ignore the ones that don't exist
                    Transform bone = animator.GetBoneTransform(targetBone);
                    if (bone == null)
                        continue;

                    for (int i = 0; i < rendererBones.Length; i++)
                    {
                        // If we find the bone's corresponding renderer bone index, update the bone and bindpose
                        if (rendererBones[i] == bone)
                        {
                            // Save original world positions of child bones
                            Dictionary<Transform, Vector3> origChildWorldPositions = new Dictionary<Transform, Vector3>();
                            foreach (Transform child in rendererBones[i])
                                origChildWorldPositions[child] = child.position;

                            // Calculate new bindpose
                            Quaternion origToTPoseRotationOffset = Quaternion.Inverse(origBoneRotation[targetBone]) * targetRotation; // calculate rotation diff
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
            description.skeleton = CreateSkeleton(packagePrefabObj);
            Avatar avatar = AvatarBuilder.BuildHumanAvatar(packagePrefabObj, description);
            avatar.name = animator.avatar.name;

            if (!avatar.isValid || !avatar.isHuman)
                throw new System.Exception("Avatar is not using a valid humanoid rig");

            animator.avatar = avatar;
            AssetDatabase.CreateAsset(avatar, Path.Combine(generatedAssetsDirPath, avatar.name + ".asset"));

            // Force back into T-pose after updating the avatar
            tPoseAnimClip.SampleAnimation(packagePrefabObj, time: 0f);

            // Correct renderer bounds based on original bounds in world space
            foreach ((SkinnedMeshRenderer renderer, Bounds bounds) in origRendererBounds)
            {
                if (renderer.rootBone != null)
                    renderer.localBounds = InverseTransformBounds(renderer.rootBone, bounds);
            }
        }

        /// <summary>
        /// All avatar animations need to be unique. Due to unity editor animator limitations, we need to identify
        /// "Animation Clip Type" by the animation clip, and if both "run" and "walk" animations are the same, we cannot
        /// differentiate them in the animator.
        /// </summary>
        private static void UniquefyAllAvatarAnimationClips(string generatedAssetsDirPath, GameObject packagePrefabObj)
        {
            AnimationClip CreateAnimClipCopy(AvatarAnimationClipType clipType, AnimationClip original)
            {
                AnimationClip duplicatedClip = new AnimationClip();
                UnityEditor.EditorUtility.CopySerialized(original, duplicatedClip);
                duplicatedClip.name = $"{original.name}_copy_{clipType}";

                string path = Path.Combine(generatedAssetsDirPath, duplicatedClip.name + ".anim");
                AssetDatabase.CreateAsset(duplicatedClip, path);
                return AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            }

            HashSet<AnimationClip> uniqueClips = new();
            if (packagePrefabObj.TryGetComponent(out SpatialAvatar avatarComponent))
            {
                FieldInfo[] animClipFields = typeof(SpatialAvatar).GetFields(BindingFlags.Public | BindingFlags.Instance).Where(f => f.FieldType == typeof(AnimationClip)).ToArray();
                foreach (FieldInfo field in animClipFields)
                {
                    AnimationClip clip = (AnimationClip)field.GetValue(avatarComponent);
                    if (clip != null && !uniqueClips.Add(clip))
                    {
                        AnimationClip duplicatedClip = CreateAnimClipCopy((AvatarAnimationClipType)field.GetValue(avatarComponent), clip);
                        field.SetValue(avatarComponent, duplicatedClip);
                    }
                }
            }
            else if (packagePrefabObj.TryGetComponent(out SpatialAvatarAttachment avatarAttachment))
            {
                avatarAttachment.avatarAnimSettings.Init();

                FieldInfo[] animConfigs = typeof(SpatialAttachmentAvatarAnimSettings).GetFields(BindingFlags.Public | BindingFlags.Instance).Where(f => f.FieldType == typeof(AttachmentAvatarAnimConfig)).ToArray();
                foreach (FieldInfo animConfigField in animConfigs)
                {
                    AttachmentAvatarAnimConfig animConfig = (AttachmentAvatarAnimConfig)animConfigField.GetValue(avatarAttachment.avatarAnimSettings);
                    AnimationClip clip = animConfig.overrideClip;
                    if (clip != null && !uniqueClips.Add(clip))
                    {
                        AnimationClip duplicatedClip = CreateAnimClipCopy(animConfig.clipType, clip);
                        animConfig.overrideClip = duplicatedClip;
                    }
                    clip = animConfig.overrideClipMale;
                    if (clip != null && !uniqueClips.Add(clip))
                    {
                        AnimationClip duplicatedClip = CreateAnimClipCopy(animConfig.clipType, clip);
                        animConfig.overrideClipMale = duplicatedClip;
                    }
                }
            }
        }

        private static void EnforceValidAvatarRagdollSetup(SpatialAvatar avatarInstance)
        {
            // The existence of at least one joint will denote an avatar ragdoll setup.
            avatarInstance.ragdollJoints = avatarInstance.GetComponentsInChildren<CharacterJoint>(includeInactive: true);
            if (avatarInstance.ragdollJoints == null || avatarInstance.ragdollJoints.Length == 0)
                return;

            // There should always be exactly 1 in the prefab if validation passed before entering this function.
            // N+1 bodies connected by N joints.
            int bodyCount = avatarInstance.ragdollJoints.Length + 1;
            // Both arrays are paired together; each index corresponds to the same object.
            avatarInstance.ragdollRigidbodies = new Rigidbody[bodyCount];
            avatarInstance.ragdollColliders = new Collider[bodyCount];

            for (int i = 0; i < avatarInstance.ragdollJoints.Length; i++)
            {
                CharacterJoint joint = avatarInstance.ragdollJoints[i];

                if (!joint.enableProjection)
                {
                    // Projection generally gives favorable results on preventing "stretchy ragdolls".
                    joint.enableProjection = true;
                    joint.projectionDistance = 0.1f;
                    joint.projectionAngle = 90f;
                }

                avatarInstance.ragdollRigidbodies[i + 1] = joint.GetComponent<Rigidbody>();
                avatarInstance.ragdollColliders[i + 1] = joint.GetComponent<Collider>();

                if (avatarInstance.ragdollRigidbodies[0] == null && joint.connectedBody.GetComponent<CharacterJoint>() == null)
                {
                    // If validation passed, then there should always be a connected body assigned to each joint.
                    // The connected body has no character joint (aka the ragdoll base).
                    avatarInstance.ragdollRigidbodies[0] = joint.connectedBody;
                    avatarInstance.ragdollColliders[0] = joint.connectedBody.GetComponent<Collider>();
                }
            }

            for (int i = 0; i < bodyCount; i++)
            {
                avatarInstance.ragdollColliders[i].isTrigger = false;
                avatarInstance.ragdollColliders[i].enabled = false;
                avatarInstance.ragdollRigidbodies[i].isKinematic = true;
            }
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

        /// <summary>
        /// Totals up vertices, triangles, and sub-meshes for all meshes from the object, and make sure none of them exceed the limit.
        /// </summary>
        public static void EnsureObjectMeshesMeetGuidelines(SpatialPackageAsset prefab, int vertexCountLimit, int triangleCountLimit, int subMeshCountLimit, float? boundsSizeMinLimit, float? boundsSizeMaxLimit, long textureMemoryLimit)
        {
            int totalVertexCount = 0;
            int totalTriangleCount = 0;
            int totalSubMeshCount = 0;
            Bounds totalBounds = new Bounds();

            // Don't include inactive gameobjects in the calculation.
            var renderers = prefab.GetComponentsInChildren<Renderer>();

            foreach (Renderer renderer in renderers)
            {
                // Ignore disabled renderer components too.
                if (!renderer.enabled)
                    continue;

                Mesh mesh;

                MeshFilter meshFilter = renderer.GetComponent<MeshFilter>();
                if (meshFilter != null)
                {
                    mesh = meshFilter.sharedMesh;
                }
                else
                {
                    SkinnedMeshRenderer skinnedMeshRenderer = renderer.GetComponent<SkinnedMeshRenderer>();
                    if (skinnedMeshRenderer != null)
                    {
                        mesh = skinnedMeshRenderer.sharedMesh;
                    }
                    else
                    {
                        continue;
                    }
                }

                if (mesh == null)
                    continue;

                bool isFirstRenderer = totalSubMeshCount == 0;
                totalVertexCount += mesh.vertexCount;
                totalTriangleCount += mesh.triangles.Length / 3;
                totalSubMeshCount += mesh.subMeshCount;

                if (isFirstRenderer)
                {
                    totalBounds = renderer.bounds;
                }
                else
                {
                    totalBounds.Encapsulate(renderer.bounds);
                }
            }

            // Get texture memory usage
            string[] allDependencies = AssetDatabase.GetDependencies(AssetDatabase.GetAssetPath(prefab));
            List<(string, long)> textureMemoryUsage = new List<(string, long)>();
            long totalTextureMemory = 0;
            foreach (string assetPath in allDependencies)
            {
                var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);

                // Don't include sdk assets in texture count
                string sdkPath = Path.Combine("Packages", "io.spatial.unitysdk") + Path.DirectorySeparatorChar;
                if (asset is Texture && !assetPath.StartsWith(sdkPath))
                {
                    long textureMemory = Profiler.GetRuntimeMemorySizeLong(asset);
                    totalTextureMemory += textureMemory;
                    textureMemoryUsage.Add((assetPath, textureMemory));
                }
            }

            ValidateObjectVertexCount(prefab, vertexCountLimit, totalVertexCount);
            ValidateObjectTriangleCount(prefab, triangleCountLimit, totalTriangleCount);
            ValidateObjectSubMeshCount(prefab, subMeshCountLimit, totalSubMeshCount);
            ValidateObjectBoundsSize(prefab, boundsSizeMinLimit, boundsSizeMaxLimit, totalBounds.size);
            ValidateObjectTextureMemory(prefab, textureMemoryLimit, totalTextureMemory, textureMemoryUsage);
        }

        private static void ValidateObjectVertexCount(SpatialPackageAsset prefab, int vertexCountLimit, int vertexCount)
        {
            if (vertexCount > vertexCountLimit)
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(
                        prefab,
                        TestResponseType.Fail,
                        $"The object has too many vertices ({vertexCount.FormatNumber()}). It must have no more than {vertexCountLimit.FormatNumber()}.",
                        "This object will likely overload mobile devices, especially with multiple instances of this object. Generate a lower level of detail to comply with this limit."
                    )
                );
            }
        }

        private static void ValidateObjectTriangleCount(SpatialPackageAsset prefab, int triangleCountLimit, int triangleCount)
        {
            if (triangleCount > triangleCountLimit)
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(
                        prefab,
                        TestResponseType.Fail,
                        $"The object has too many triangles ({triangleCount.FormatNumber()}). It must have no more than {triangleCountLimit.FormatNumber()}.",
                        "This object will likely overload mobile devices, especially with multiple instances of this object. Generate a lower level of detail to comply with this limit."
                    )
                );
            }
        }

        private static void ValidateObjectSubMeshCount(SpatialPackageAsset prefab, int subMeshCountLimit, int subMeshCount)
        {
            if (subMeshCount > subMeshCountLimit)
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(
                        prefab,
                        TestResponseType.Fail,
                        $"The object has too many sub-meshes ({subMeshCount.FormatNumber()}). It must have no more than {subMeshCountLimit.FormatNumber()}.",
                        "This can negatively impact performance on lower-end devices. Combine meshes and materials to comply with this limit."
                    )
                );
            }
        }

        private static void ValidateObjectBoundsSize(SpatialPackageAsset prefab, float? boundsSizeMinLimit, float? boundsSizeMaxLimit, Vector3 boundsSize)
        {
            if (boundsSizeMinLimit.HasValue && (boundsSize.x < boundsSizeMinLimit || boundsSize.y < boundsSizeMinLimit || boundsSize.z < boundsSizeMinLimit))
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(
                        prefab,
                        TestResponseType.Fail,
                        $"The object occupies too little physical space ({FormatDimensions(boundsSize)}). It must not be less than {boundsSizeMinLimit}m in any dimension.",
                        "This helps standardize objects so that they are more consistent with other objects in the community. Increase the scale of the object to comply with this guideline."
                    )
                );
            }
            else if (boundsSizeMaxLimit.HasValue && (boundsSize.x > boundsSizeMaxLimit || boundsSize.y > boundsSizeMaxLimit || boundsSize.z > boundsSizeMaxLimit))
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(
                        prefab,
                        TestResponseType.Fail,
                        $"The object occupies too much physical space ({FormatDimensions(boundsSize)}). It must not exceed {boundsSizeMaxLimit}m in any dimension.",
                        "This helps standardize objects so that they are more consistent with other objects in the community. Reduce the scale of the object to comply with this guideline."
                    )
                );
            }
        }

        private static void ValidateObjectTextureMemory(SpatialPackageAsset prefab, long textureMemoryLimit, long totalTextureMemory, List<(string, long)> usedTextures)
        {
            if (totalTextureMemory > textureMemoryLimit)
            {
                double totalTextureMemoryMB = totalTextureMemory / 1024.0 / 1024.0;
                double textureMemoryLimitMB = textureMemoryLimit / 1024.0 / 1024.0;
                // Sort textures by size (descending)
                usedTextures.Sort((a, b) => b.Item2.CompareTo(a.Item2));

                SpatialValidator.AddResponse(
                    new SpatialTestResponse(
                        prefab,
                        TestResponseType.Fail,
                        $"The object uses too much texture memory ({totalTextureMemoryMB.ToString("0.00")} MB)",
                        $"This object uses a total of {totalTextureMemoryMB.ToString("0.00")} MB of texture memory but it must not exceed {textureMemoryLimitMB.ToString("0.00")} MB.\n"
                        + "High memory usage can cause application crashes on lower end devices. Reduce the number of textures, reduce the resolution or modify the compression of textures to comply with this limit.\n\n"
                        + "Here's a list of all the textures used by the avatar attachment:\n - " + string.Join("\n - ", usedTextures.Take(20).Select(m => $"{(m.Item2 / 1024.0 / 1024.0):0.00} MB - {m.Item1}"))
                    )
                );
            }
        }

        private static string FormatDimensions(Vector3 dimensions)
        {
            return $"{dimensions.x.ToString("F2")}m x {dimensions.y.ToString("F2")}m x {dimensions.z.ToString("F2")}m";
        }
    }
}