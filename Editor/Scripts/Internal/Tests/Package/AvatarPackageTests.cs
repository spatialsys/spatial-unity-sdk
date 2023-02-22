using UnityEngine;

namespace SpatialSys.UnitySDK.Editor
{
    public static class AvatarPackageTests
    {
        /// <summary>
        /// Checks for the following in order: Prefab is assigned in config, there's an animator component on the prefab, and the animator has a valid humanoid rig.
        /// </summary>
        [PackageTest(PackageType.Avatar)]
        public static void EnsureAnimatorRigIsHumanoid(AvatarConfig config)
        {
            if (config.prefab == null)
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(config, TestResponseType.Fail, "A prefab with the Spatial Avatar component must be assigned in the config.")
                );
                return;
            }

            Animator animator;
            if (!config.prefab.TryGetComponent<Animator>(out animator))
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(config, TestResponseType.Fail, "The avatar prefab does not have an animator attached to it.")
                );
                return;
            }

            if (animator.avatar == null || !animator.avatar.isHuman)
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(config.prefab, TestResponseType.Fail, "The avatar must have a valid humanoid rig. Non-humanoid rigs are not supported.")
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

            int totalVertexCount = 0;
            int totalTriangleCount = 0;
            int totalSubMeshCount = 0;
            Bounds totalBounds = new Bounds();

            // Don't include inactive gameobjects in the calculation.
            var skinnedRenderers = config.prefab.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer renderer in skinnedRenderers)
            {
                // Ignore disabled renderer components too.
                if (!renderer.enabled)
                    continue;

                Mesh mesh = renderer.sharedMesh;
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

            ValidateAvatarVertexCount(config, totalVertexCount);
            ValidateAvatarTriangleCount(config, totalTriangleCount);
            ValidateAvatarSubMeshCount(config, totalSubMeshCount);
            ValidateAvatarBoundsSize(config, totalBounds.size);
        }

        private static void ValidateAvatarVertexCount(AvatarConfig config, int vertexCount)
        {
            int vertexCountLimit = (config.usageContext == AvatarConfig.UsageContext.Global) ? 8000 : 100000;

            if (vertexCount > vertexCountLimit)
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(
                        config.prefab,
                        TestResponseType.Fail,
                        $"The avatar has too many vertices ({vertexCount.FormatInteger()}). It must have no more than {vertexCountLimit.FormatInteger()}.",
                        "This avatar will likely overload mobile devices, especially with multiple instances of this avatar. Generate a lower level of detail to comply with this limit."
                    )
                );
            }
        }

        private static void ValidateAvatarTriangleCount(AvatarConfig config, int triangleCount)
        {
            int triangleCountLimit = (config.usageContext == AvatarConfig.UsageContext.Global) ? 15000 : 200000;

            if (triangleCount > triangleCountLimit)
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(
                        config.prefab,
                        TestResponseType.Fail,
                        $"The avatar has too many triangles ({triangleCount.FormatInteger()}). It must have no more than {triangleCountLimit.FormatInteger()}.",
                        "This avatar will likely overload mobile devices, especially with multiple instances of this avatar. Generate a lower level of detail to comply with this limit."
                    )
                );
            }
        }

        private static void ValidateAvatarSubMeshCount(AvatarConfig config, int subMeshCount)
        {
            // Submeshes correspond to a unique material and mesh, which we can use to approximate the draw calls it will use.
            int subMeshCountLimit = (config.usageContext == AvatarConfig.UsageContext.Global) ? 4 : 100;

            if (subMeshCount > subMeshCountLimit)
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(
                        config.prefab,
                        TestResponseType.Fail,
                        $"The avatar has too many sub-meshes ({subMeshCount.FormatInteger()}). It must have no more than {subMeshCountLimit.FormatInteger()}.",
                        "This can negatively impact performance on lower-end devices. Combine meshes and materials to comply with this limit."
                    )
                );
            }
        }

        private static void ValidateAvatarBoundsSize(AvatarConfig config, Vector3 boundsSize)
        {
            // We're using a float since the avatar can be oriented in any axis (e.g. X-axis might be the avatar's height, depending on where it's exported from).
            float boundsSizeLimit = (config.usageContext == AvatarConfig.UsageContext.Global) ? 2.5f : 25f;

            if (boundsSize.x > boundsSizeLimit || boundsSize.y > boundsSizeLimit || boundsSize.z > boundsSizeLimit)
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(
                        config.prefab,
                        TestResponseType.Fail,
                        $"The avatar occupies too much physical space ({FormatDimensions(boundsSize)}). It must not exceed {boundsSizeLimit}m in any dimension.",
                        "This helps standardize avatars so that they are more consistent with other avatars in the community. Reduce the scale of the avatar to comply with this guideline."
                    )
                );
            }
        }

        private static string FormatDimensions(Vector3 dimensions)
        {
            return $"{dimensions.x.ToString("F2")}m x {dimensions.y.ToString("F2")}m x {dimensions.z.ToString("F2")}m";
        }

        /// <summary>
        /// Checks each texture dependency associated with the avatar prefab.
        /// </summary>
        [PackageTest(PackageType.Avatar)]
        public static void EnsureAvatarTexturesMeetGuidelines(AvatarConfig config)
        {
            if (config.prefab == null)
                return;

            int textureSizeLimit = (config.usageContext == AvatarConfig.UsageContext.Global) ? 1024 : 4096;
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
    }
}
