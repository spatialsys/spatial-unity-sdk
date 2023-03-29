using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SpatialSys.UnitySDK.Editor
{
    [CustomEditor(typeof(SpatialQuest))]
    public class SpatialQuestEditor : SpatialComponentEditorBase
    {
        private Texture2D _markerTexture;
        private GUIStyle _markerIconStyle;
        private GUIStyle _markerTextStyle;
        
        public void OnSceneGUI()
        {
            if (!_markerTexture)
            {
                _markerTexture = SpatialGUIUtility.LoadGUITexture("Icons/icon_quest_marker.png");
                _markerIconStyle = new GUIStyle();
                _markerIconStyle.fixedWidth = 30;
                _markerIconStyle.fixedHeight = 30;
                _markerIconStyle.alignment = TextAnchor.MiddleCenter;

                _markerTextStyle = new GUIStyle();
                _markerTextStyle.alignment = TextAnchor.MiddleCenter;
                _markerTextStyle.normal.textColor = Color.white;
                _markerTextStyle.fontStyle = FontStyle.Bold;
                _markerTextStyle.padding = new RectOffset(15, 0, 30, 0);
            }

            Handles.color = Color.black;

            var t = target as SpatialQuest;
            foreach (var task in t.tasks)
            {
                if (task.taskMarkers == null)
                    continue;
                
                foreach (var marker in task.taskMarkers)
                {
                    if (marker != null)
                    {
                        Vector3 markerPos = marker.transform.position;
                        Handles.DrawLine(t.transform.position, markerPos, 1);
                        Handles.Label(markerPos, new GUIContent(_markerTexture), _markerIconStyle);
                        Handles.Label(markerPos, task.id.ToString(), _markerTextStyle);
                    }
                }
            }
        }
    }
}
