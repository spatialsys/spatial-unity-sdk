using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SpatialSys.UnitySDK.Editor
{
    [CustomEditor(typeof(SpatialNetworkBehaviour), editorForChildClasses: true)]
    public class SpatialNetworkBehaviourEditor : UnityEditor.Editor
    {
        private bool _initialized;
        private Texture2D _iconTexture;
        private GUIStyle _areaStyle;
        private GUIStyle _logoStyle;
        private GUIStyle _titleStyle;
        private SpatialNetworkObject _associatedNetworkObject;

        private void InitializeIfNecessary(UnityEngine.Object target)
        {
            if (_initialized)
                return;

            _initialized = true;

            // GetComponentInParent does not search the current GameObject when viewing a root level prefab GameObject in inspector
            _associatedNetworkObject = (target as Component).GetComponent<SpatialNetworkObject>();
            if (_associatedNetworkObject == null)
                _associatedNetworkObject = (target as Component).GetComponentInParent<SpatialNetworkObject>();

            _iconTexture = SpatialGUIUtility.LoadGUITexture("Icons/icon_syncedObject.png");

            _areaStyle = new GUIStyle() {
                border = new RectOffset(8, 8, 8, 8),
                padding = new RectOffset(8, 4, 4, 4),
                alignment = TextAnchor.MiddleLeft,
            };
            _areaStyle.normal.background = SpatialGUIUtility.LoadGUITexture("GUI/TooltipBackground.png");
            _areaStyle.normal.textColor = Color.white;

            _logoStyle = new GUIStyle() {
                fixedHeight = 16,
                fixedWidth = 16,
                padding = new RectOffset(0, 0, 0, 0),
                border = new RectOffset(0, 0, 0, 0),
            };

            _titleStyle = new GUIStyle() {
                fontStyle = FontStyle.Bold,
                fontSize = 14,
                wordWrap = true,
                alignment = TextAnchor.MiddleLeft,
            };
            _titleStyle.normal.textColor = Color.white;
        }


        public override void OnInspectorGUI()
        {
            var editorTarget = target as UnityEngine.Object;
            InitializeIfNecessary(editorTarget);

            GUILayout.Space(-4);// Top margin hack
            GUILayout.BeginHorizontal();
            GUILayout.Space(-18);// Left margin hack

            // GUILayout.BeginVertical();
            GUILayout.BeginVertical(_areaStyle);
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Box(_iconTexture, _logoStyle);
                    GUILayout.Space(4);

                    // Align to the center of the icon
                    GUILayout.BeginVertical();
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("Network Behaviour", _titleStyle);
                    GUILayout.FlexibleSpace();
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();

            }
            GUILayout.EndVertical();

            GUILayout.Space(-4);// Right Margin hack
            GUILayout.EndHorizontal();

            if (_associatedNetworkObject == null)
            {
                EditorGUILayout.HelpBox("All network behaviours must either have a Spatial Network Object on the same GameObject or on one of its parents.", MessageType.Warning);
            }

            DrawDefaultInspector();
        }
    }
}
