using UnityEngine;
using UnityEditor;
using System;

namespace SpatialSys.UnitySDK
{
    [CustomPropertyDrawer(typeof(ToggleWithEnum))]
    public class ToggleWithEnumDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ToggleWithEnum toggleAtt = (ToggleWithEnum)attribute;
            SerializedProperty toggleProperty = property.serializedObject.FindProperty(toggleAtt.targetPropertyName);

            if (Array.IndexOf(toggleAtt.validOptions, toggleProperty.enumValueIndex) >= 0)
                EditorGUI.PropertyField(position, property, label);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            ToggleWithEnum toggleAtt = (ToggleWithEnum)attribute;
            SerializedProperty toggleProperty = property.serializedObject.FindProperty(toggleAtt.targetPropertyName);

            if (Array.IndexOf(toggleAtt.validOptions, toggleProperty.enumValueIndex) >= 0)
            {
                return EditorGUI.GetPropertyHeight(property, label);
            }
            else
            {
                return -EditorGUIUtility.standardVerticalSpacing;
            }
        }
    }
}
