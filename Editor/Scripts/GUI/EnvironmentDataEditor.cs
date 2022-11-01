using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SpatialSys.UnitySDK.Editor
{
    [CustomEditor(typeof(EnvironmentData), true), CanEditMultipleObjects]
    public class EnvironmentDataEditor : UnityEditor.Editor
    {
        private bool _initialized;

        [SerializeField]
        private Texture2D backgroundTexture;

        private GUIStyle _areaStyle;
        private GUIStyle _titleStyle;
        private GUIStyle _subTitleStyle;

        private void InitializeIfNecessary()
        {
            if (_initialized)
            {
                return;
            }

            _areaStyle = new GUIStyle() {
                border = new RectOffset(8, 8, 8, 8),
                padding = new RectOffset(8, 8, 8, 8),
            };
            _areaStyle.normal.background = backgroundTexture;

            _titleStyle = new GUIStyle() {
                fontStyle = FontStyle.Bold,
                fontSize = 24,
                wordWrap = true,
            };
            _titleStyle.normal.textColor = Color.red;

            _subTitleStyle = new GUIStyle() {
                fontSize = 12,
                wordWrap = true,
                richText = true,
            };
            _subTitleStyle.normal.textColor = new Color(1, 0, 0, .75f);
        }

        public override void OnInspectorGUI()
        {
            InitializeIfNecessary();
            var editorTarget = target as EnvironmentData;
            serializedObject.Update();

            GUILayout.BeginVertical(_areaStyle);
            GUILayout.Label("For internal use only", _titleStyle);
            GUILayout.Label("This component should not be used in your scene. It will automatically be stripped when published.", _subTitleStyle);
            GUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
