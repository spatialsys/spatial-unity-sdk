using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SpatialSys.UnitySDK.Editor
{
    [CustomEditor(typeof(SpatialTriggerEvent))]
    public class SpatialTriggerEventEditor : SpatialComponentEditorBase
    {
        public void OnSceneGUI()
        {
            var t = target as SpatialTriggerEvent;

            SpatialHandles.UnityEventHandle(t.transform, t.onEnter);
            SpatialHandles.UnityEventHandle(t.transform, t.onExit);
        }
    }
}
