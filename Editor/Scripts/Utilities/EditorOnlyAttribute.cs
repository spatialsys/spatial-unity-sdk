using System;

namespace SpatialSys.UnitySDK.Editor
{
#if UNITY_EDITOR
    /// <summary>
    /// This can be used to mark a MonoBehavior as editor-only.
    /// When this tag is present, the component will be removed during scene build-time
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class EditorOnlyAttribute : Attribute
    {
    }
#endif
}
