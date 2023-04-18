using UnityEngine;
using UnityEditor;

namespace SpatialSys.UnitySDK.Editor
{
    [CustomEditor(typeof(SpatialEnvironmentSettingsOverrides))]
    public class SpatialEnvironmentSettingsOverridesEditor : SpatialComponentEditorBase
    {
        private SerializedProperty _environmentSettings;
        private SerializedProperty _localAvatarMovingSpeed;
        private SerializedProperty _localAvatarRunSpeed;
        private SerializedProperty _localAvatarJumpHeight;
        private SerializedProperty _localAvatarGravityMultiplier;
        private SerializedProperty _localAvatarFallingGravityMultiplier;
        private SerializedProperty _localAvatarUseVariableHeightJump;
        private SerializedProperty _localAvatarMaxJumpCount;

        void OnEnable()
        {
            _environmentSettings = serializedObject.FindProperty("environmentSettings");
            _localAvatarMovingSpeed = _environmentSettings.FindPropertyRelative("localAvatarMovingSpeed");
            _localAvatarRunSpeed = _environmentSettings.FindPropertyRelative("localAvatarRunSpeed");
            _localAvatarJumpHeight = _environmentSettings.FindPropertyRelative("localAvatarJumpHeight");
            _localAvatarGravityMultiplier = _environmentSettings.FindPropertyRelative("localAvatarGravityMultiplier");
            _localAvatarFallingGravityMultiplier = _environmentSettings.FindPropertyRelative("localAvatarFallingGravityMultiplier");
            _localAvatarUseVariableHeightJump = _environmentSettings.FindPropertyRelative("localAvatarUseVariableHeightJump");
            _localAvatarMaxJumpCount = _environmentSettings.FindPropertyRelative("localAvatarMaxJumpCount");
        }

        public override void DrawFields()
        {
            base.DrawFields();

            SpatialEnvironmentSettingsOverrides targetComponent = target as SpatialEnvironmentSettingsOverrides;

            if (_environmentSettings.isExpanded && targetComponent.environmentSettings.avatarControlSettings == EnvironmentSettings.AvatarControlSettings.Override)
            {
                EditorGUI.indentLevel += 2;
                EditorGUILayout.PropertyField(_localAvatarMovingSpeed, new GUIContent("Walk Speed (m/s)"));
                EditorGUILayout.PropertyField(_localAvatarRunSpeed, new GUIContent("Run Speed (m/s)"));
                EditorGUILayout.PropertyField(_localAvatarJumpHeight, new GUIContent("Jump Height (m)"));
                EditorGUILayout.PropertyField(_localAvatarGravityMultiplier, new GUIContent("Gravity Multiplier"));
                EditorGUILayout.PropertyField(_localAvatarFallingGravityMultiplier, new GUIContent("Falling Gravity Multiplier"));
                EditorGUILayout.PropertyField(_localAvatarUseVariableHeightJump, new GUIContent("Use Variable Height Jump"));
                EditorGUILayout.PropertyField(_localAvatarMaxJumpCount, new GUIContent("Maximum Jump Count"));
                EditorGUI.indentLevel -= 2;
            }
        }
    }
}