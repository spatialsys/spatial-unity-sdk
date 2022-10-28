using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SpatialSys.UnitySDK
{
    [CustomEditor(typeof(SpatialComponentBase), true), CanEditMultipleObjects]
    public class SpatialComponentEditor : UnityEditor.Editor
    {
        private bool _initialized;
        //used to hide the script field
        private static readonly string[] _excludedProperties = new string[] { "m_Script" };
        [SerializeField]
        private Texture2D backgroundTexture;
        [SerializeField]
        private Texture2D logoTexture;

        private GUIStyle _areaStyle;
        private GUIStyle _logoStyle;
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
            _areaStyle.normal.textColor = Color.white;

            _logoStyle = new GUIStyle() {
                fixedHeight = 32,
                fixedWidth = 32,
            };

            _titleStyle = new GUIStyle() {
                fontStyle = FontStyle.Bold,
                fontSize = 24,
                wordWrap = true,
            };
            _titleStyle.normal.textColor = Color.white;

            _subTitleStyle = new GUIStyle() {
                fontSize = 12,
                wordWrap = true,
                richText = true,
            };
            _subTitleStyle.normal.textColor = new Color(1, 1, 1, .75f);
        }

        public override void OnInspectorGUI()
        {
            InitializeIfNecessary();
            var editorTarget = target as SpatialComponentBase;
            serializedObject.Update();

            GUILayout.BeginVertical(_areaStyle);
            GUILayout.BeginHorizontal();
            GUILayout.Box(logoTexture, _logoStyle);
            GUILayout.Space(4);
            GUILayout.Label(editorTarget.prettyName, _titleStyle);
            if (!string.IsNullOrEmpty(editorTarget.documentationURL))
            {
                GUILayout.Space(4);
                if (GUILayout.Button("Help"))
                {
                    Application.OpenURL(editorTarget.documentationURL);
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Label(editorTarget.tooltip, _subTitleStyle);
            GUILayout.EndVertical();

            GUILayout.Space(8);

            DrawPropertiesExcluding(serializedObject, _excludedProperties);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
