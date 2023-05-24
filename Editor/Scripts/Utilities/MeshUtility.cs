using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace SpatialSys.UnitySDK.Editor
{
    public static class MeshUtility
    {
        /// <summary>
        /// Returns a new mesh containing all attributes and values copied from the specified source mesh.
        /// </summary>
        /// <param name="source">The mesh to copy from.</param>
        /// <returns>A new <see cref="UnityEngine.Mesh"/> object with the same values as the source mesh.</returns>
        public static Mesh DeepCopy(Mesh source)
        {
            Mesh m = new Mesh();
            CopyTo(source, m);
            return m;
        }

        /// <summary>
        /// Copies mesh attribute values from one mesh to another.
        /// </summary>
        /// <param name="source">The mesh from which to copy attribute values.</param>
        /// <param name="destination">The destination mesh to copy attribute values to.</param>
        /// <exception cref="ArgumentNullException">Throws if source or destination is null.</exception>
        public static void CopyTo(Mesh source, Mesh destination)
        {
            if (source == null)
                throw new System.ArgumentNullException("source");

            if (destination == null)
                throw new System.ArgumentNullException("destination");

            Vector3[] v = new Vector3[source.vertices.Length];
            int[][] t = new int[source.subMeshCount][];
            Vector2[] u = new Vector2[source.uv.Length];
            Vector2[] u2 = new Vector2[source.uv2.Length];
            Vector4[] tan = new Vector4[source.tangents.Length];
            Vector3[] n = new Vector3[source.normals.Length];
            Color32[] c = new Color32[source.colors32.Length];

            System.Array.Copy(source.vertices, v, v.Length);

            for (int i = 0; i < t.Length; i++)
                t[i] = source.GetTriangles(i);

            System.Array.Copy(source.uv, u, u.Length);
            System.Array.Copy(source.uv2, u2, u2.Length);
            System.Array.Copy(source.normals, n, n.Length);
            System.Array.Copy(source.tangents, tan, tan.Length);
            System.Array.Copy(source.colors32, c, c.Length);

            destination.Clear();
            destination.name = source.name;

            destination.vertices = v;

            destination.subMeshCount = t.Length;

            for (int i = 0; i < t.Length; i++)
                destination.SetTriangles(t[i], i);

            destination.uv = u;
            destination.uv2 = u2;
            destination.tangents = tan;
            destination.normals = n;
            destination.colors32 = c;
        }
    }
}