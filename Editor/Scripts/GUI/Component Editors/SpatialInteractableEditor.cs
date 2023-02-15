using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace SpatialSys.UnitySDK.Editor
{
    [CustomEditor(typeof(SpatialInteractable))]
    public class SpatialInteractableEditor : SpatialComponentEditorBase
    {
        public void OnSceneGUI()
        {
            var t = target as SpatialInteractable;
            SpatialHandles.RadiusHandle(t.transform.position, ref t.radius);
        }
    }
}
