using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace SpatialSys.UnitySDK.Editor
{
    // The following class strips pro builder components if they exist on a game object. We are using reflection and not types because
    // we don't want to include pro builder as a dependency for the unity sdk. This method allows us to support
    // pro builder stripping on projects that include pro builder, without it being a dependency.
    public static class ProBuilderStripUtility
    {
        /// <summary>
        /// Determines if a specified type is a pro builder mesh
        /// </summary>
        public static bool IsProBuilderMesh(this Type type)
        {
            return type.Name == "ProBuilderMesh";
        }

        /// <summary>
        /// Strips pro builder components from a gameObject. Before doing so, deep copies the pro builder mesh into the mesh filter.
        /// </summary>
        public static void Strip(Component pb)
        {
            GameObject go = pb.gameObject;

            Mesh pbMesh = (Mesh)pb.GetType().GetProperty("mesh", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(pb);
            
            if (pbMesh == null)
            {
                DestroyProBuilderMeshAndDependencies(go, pb);
                return;
            }
            Mesh m = MeshUtility.DeepCopy(pbMesh);

            DestroyProBuilderMeshAndDependencies(go, pb);

            go.GetComponent<MeshFilter>().sharedMesh = m;
            if (go.TryGetComponent(out MeshCollider meshCollider))
                meshCollider.sharedMesh = m;
        }
        
        private static void DestroyProBuilderMeshAndDependencies(GameObject go, Component pb)
        {
            TryDestroyComponent(go, "PolyShape");
            TryDestroyComponent(go, "BezierShape");
            TryDestroyComponent(go, "ProBuilderShape");
            TryDestroyComponent(go, "PolyShape");

            UnityEngine.Object.DestroyImmediate(pb);

            TryDestroyComponent(go, "Entity");
        }

        private static void TryDestroyComponent(GameObject go, string componentName)
        {
            Component component = go.GetComponent(componentName);
            if (component)
            {
                UnityEngine.Object.DestroyImmediate(component);
            }
        }
    }
}