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
            SpatialHandles.RadiusHandle(t.transform.position, ref t.radius);
        }
    }
}
