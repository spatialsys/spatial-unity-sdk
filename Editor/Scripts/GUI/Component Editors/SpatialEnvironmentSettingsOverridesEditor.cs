using UnityEngine;
using UnityEditor;

namespace SpatialSys.UnitySDK.Editor
{
    [CustomEditor(typeof(SpatialEnvironmentSettingsOverrides))]
    public class SpatialEnvironmentSettingsOverridesEditor : SpatialComponentEditorBase
    {
        SerializedProperty localAvatarMovingSpeed;

        void OnEnable()
        {
            localAvatarMovingSpeed = serializedObject.FindProperty("environmentSettings.localAvatarMovingSpeed");
        }

        public override void DrawFields()
        {
            base.DrawFields();

            SpatialEnvironmentSettingsOverrides targetComponent = target as SpatialEnvironmentSettingsOverrides;

            // Show movingspeed if avatarControlSettings is Override
            if (targetComponent.environmentSettings.avatarControlSettings == EnvironmentSettings.AvatarControlSettings.Override)
            {
                EditorGUI.indentLevel += 2;
                EditorGUILayout.PropertyField(localAvatarMovingSpeed);
                EditorGUI.indentLevel -= 2;
            }
        }
    }
}