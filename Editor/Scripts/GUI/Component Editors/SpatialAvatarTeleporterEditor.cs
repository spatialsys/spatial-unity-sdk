using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SpatialSys.UnitySDK.Editor
{
    [CustomEditor(typeof(SpatialAvatarTeleporter))]
    public class SpatialAvatarTeleporterEditor : SpatialComponentEditorBase
    {
        public void OnSceneGUI()
        {
            var t = target as SpatialAvatarTeleporter;
            if (t.targetLocation)
            {
                SpatialHandles.DrawGroundPoint(t.targetLocation.position, .25f);
                SpatialHandles.TargetTransformHandle(t.transform, ref t.targetLocation);
            }
        }
    }
}
