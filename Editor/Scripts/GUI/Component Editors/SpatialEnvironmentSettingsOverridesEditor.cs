using UnityEngine;
using UnityEditor;

namespace SpatialSys.UnitySDK.Editor
{
    [CustomEditor(typeof(SpatialEnvironmentSettingsOverrides))]
    public class SpatialEnvironmentSettingsOverridesEditor : SpatialComponentEditorBase
    {
        SerializedProperty localAvatarMovingSpeed;
        SerializedProperty localAvatarRunSpeed;

        void OnEnable()
        {
            localAvatarMovingSpeed = serializedObject.FindProperty("environmentSettings.localAvatarMovingSpeed");
            localAvatarRunSpeed = serializedObject.FindProperty("environmentSettings.localAvatarRunSpeed");
        }

        public override void DrawFields()
        {
            base.DrawFields();

            SpatialEnvironmentSettingsOverrides targetComponent = target as SpatialEnvironmentSettingsOverrides;

            // Show movingspeed if avatarControlSettings is Override
            if (targetComponent.environmentSettings.avatarControlSettings == EnvironmentSettings.AvatarControlSettings.Override)
            {
                EditorGUI.indentLevel += 2;
                EditorGUILayout.PropertyField(localAvatarMovingSpeed, new GUIContent("Local Avatar Walk Speed (m/s)"));
                EditorGUILayout.PropertyField(localAvatarRunSpeed, new GUIContent("Local Avatar Run Speed (m/s)"));
                EditorGUI.indentLevel -= 2;
            }
        }
    }
}