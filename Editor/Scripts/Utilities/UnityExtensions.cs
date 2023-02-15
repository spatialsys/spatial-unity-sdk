using System.Text;
using UnityEngine;
using UnityEditor;

namespace SpatialSys.UnitySDK.Editor
{
    public static class UnityExtensions
    {
        public static string GetPath(this GameObject obj, Transform relativeRoot = null)
        {
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            GetPathRecursiveInternal(obj.transform, stringBuilder, relativeRoot);
            return stringBuilder.ToString();
        }

        private static void GetPathRecursiveInternal(Transform t, System.Text.StringBuilder stringBuilder, Transform root)
        {
            if (t.parent != null && t.parent != root)
                GetPathRecursiveInternal(t.parent, stringBuilder, root);

            stringBuilder.AppendFormat("/{0}", t.gameObject.name);
        }
    }
}
