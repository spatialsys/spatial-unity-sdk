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
    }
}
