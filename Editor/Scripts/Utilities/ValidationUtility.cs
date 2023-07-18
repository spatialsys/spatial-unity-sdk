using UnityEngine;

namespace SpatialSys.UnitySDK.Editor
{
    public static class ValidationUtility
    {
        /// <summary>
        /// Totals up vertices, triangles, and sub-meshes for all meshes from the object, and make sure none of them exceed the limit.
        /// </summary>
        public static void EnsureObjectMeshesMeetGuidelines(SpatialPackageAsset prefab, int vertexCountLimit, int triangleCountLimit, int subMeshCountLimit, float boundsSizeLimit)
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

            ValidateObjectVertexCount(prefab, vertexCountLimit, totalVertexCount);
            ValidateObjectTriangleCount(prefab, triangleCountLimit, totalTriangleCount);
            ValidateObjectSubMeshCount(prefab, subMeshCountLimit, totalSubMeshCount);
            ValidateObjectBoundsSize(prefab, boundsSizeLimit, totalBounds.size);
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

        private static void ValidateObjectBoundsSize(SpatialPackageAsset prefab, float boundsSizeLimit, Vector3 boundsSize)
        {
            if (boundsSize.x > boundsSizeLimit || boundsSize.y > boundsSizeLimit || boundsSize.z > boundsSizeLimit)
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(
                        prefab,
                        TestResponseType.Fail,
                        $"The object occupies too much physical space ({FormatDimensions(boundsSize)}). It must not exceed {boundsSizeLimit}m in any dimension.",
                        "This helps standardize objects so that they are more consistent with other objects in the community. Reduce the scale of the object to comply with this guideline."
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