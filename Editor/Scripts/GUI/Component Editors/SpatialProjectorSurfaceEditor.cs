using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SpatialSys.UnitySDK.Editor
{
    [CustomEditor(typeof(SpatialProjectorSurface))]
    public class SpatialProjectorSurfaceEditor : SpatialComponentEditorBase
    {
        public void OnSceneGUI()
        {
            var t = target as SpatialProjectorSurface;
            Vector2 scale = t.size * new Vector2(2f, 1f);
            SpatialHandles.EvenlyScaleableRectangleHandle(t.transform.position, ref scale, t.transform.localToWorldMatrix);
            t.size = scale.y;

            Vector3 ne = t.transform.position + (t.transform.right * (scale.x * .5f)) + (t.transform.up * (scale.y * .5f));
            Vector3 se = t.transform.position + (t.transform.right * (scale.x * .5f)) + (-t.transform.up * (scale.y * .5f));
            Vector3 sw = t.transform.position + (-t.transform.right * (scale.x * .5f)) + (-t.transform.up * (scale.y * .5f));
            Vector3 nw = t.transform.position + (-t.transform.right * (scale.x * .5f)) + (t.transform.up * (scale.y * .5f));

            Handles.color = Color.white;
            Handles.ConeHandleCap(-1, ne + t.transform.forward * HandleUtility.GetHandleSize(ne) * .5f, Quaternion.LookRotation(t.transform.forward), HandleUtility.GetHandleSize(ne) * .2f, EventType.Repaint);
            Handles.ConeHandleCap(-1, se + t.transform.forward * HandleUtility.GetHandleSize(ne) * .5f, Quaternion.LookRotation(t.transform.forward), HandleUtility.GetHandleSize(se) * .2f, EventType.Repaint);
            Handles.ConeHandleCap(-1, sw + t.transform.forward * HandleUtility.GetHandleSize(ne) * .5f, Quaternion.LookRotation(t.transform.forward), HandleUtility.GetHandleSize(sw) * .2f, EventType.Repaint);
            Handles.ConeHandleCap(-1, nw + t.transform.forward * HandleUtility.GetHandleSize(ne) * .5f, Quaternion.LookRotation(t.transform.forward), HandleUtility.GetHandleSize(nw) * .2f, EventType.Repaint);
        }
    }
}
