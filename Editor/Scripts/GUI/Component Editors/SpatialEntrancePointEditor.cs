using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SpatialSys.UnitySDK.Editor
{
    [CustomEditor(typeof(SpatialEntrancePoint))]
    public class SpatialEntrancePointEditor : SpatialComponentEditorBase
    {
        public void OnSceneGUI()
        {
            var t = target as SpatialEntrancePoint;

            SpatialHandles.DrawGroundPoint(t.transform.position, .25f);

            Vector3 dir = t.transform.forward;
            if (t.transform.forward == Vector3.up || t.transform.forward == Vector3.down)
            {
                dir = t.transform.right;
            }
            SpatialHandles.RadiusAndDirectionHandle(t.transform.position, ref t.radius, ref dir);
            t.transform.forward = dir;
        }
    }
}
