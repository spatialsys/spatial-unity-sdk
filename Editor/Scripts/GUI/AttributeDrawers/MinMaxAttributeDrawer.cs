using UnityEditor;
using UnityEngine;

namespace SpatialSys.UnitySDK.Editor
{
    [CustomPropertyDrawer(typeof(MinMaxAttribute))]
    public class MinMaxAttributeDrawer : PropertyDrawer
    {
        MinMaxAttribute minMax { get { return ((MinMaxAttribute)attribute); } }

        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            return base.GetPropertyHeight(prop, label) * 2f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //The following drawer doesnt work with multiple objects selected, so just do default vec2 drawer
            if (property.serializedObject.isEditingMultipleObjects)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            Rect sliderPosition = EditorGUI.PrefixLabel(position, label);
            SerializedProperty min = property.FindPropertyRelative("x");
            SerializedProperty max = property.FindPropertyRelative("y");

            // draw the range and the reset button first so that the slider doesn't grab all the input
            Rect rangePosition = sliderPosition;
            rangePosition.y += rangePosition.height * 0.5f;
            rangePosition.height *= 0.5f;
            Rect contentPosition = rangePosition;
            EditorGUI.indentLevel = 0;
            EditorGUIUtility.labelWidth = 30f;
            contentPosition.width *= 0.3f;
            EditorGUI.PropertyField(contentPosition, min, new GUIContent("Min"));
            contentPosition.x += contentPosition.width + 20f;
            EditorGUI.PropertyField(contentPosition, max, new GUIContent("Max"));
            contentPosition.x += contentPosition.width + 20f;
            contentPosition.width = 50.0f;
            if (GUI.Button(contentPosition, "Reset"))
            {
                min.floatValue = minMax.minDefaultVal;
                max.floatValue = minMax.maxDefaultVal;
            }
            float minValue = min.floatValue;
            float maxValue = max.floatValue;

#if UNITY_2017_1_OR_NEWER
            EditorGUI.MinMaxSlider(sliderPosition, GUIContent.none, ref minValue, ref maxValue, minMax.min, minMax.max);
#else
            EditorGUI.MinMaxSlider( GUIContent.none, sliderPosition, ref minValue, ref maxValue, minMax.min, minMax.max );
#endif
            // round to readable values
            min.floatValue = Mathf.Round(minValue / 0.01f) * 0.01f;
            max.floatValue = Mathf.Round(maxValue / 0.01f) * 0.01f;

            // clamp to each-other
            min.floatValue = Mathf.Clamp(min.floatValue, minMax.min, maxValue);
            max.floatValue = Mathf.Clamp(max.floatValue, minValue, minMax.max);

            //final clamp to absolute min/max
            min.floatValue = Mathf.Clamp(min.floatValue, minMax.min, minMax.max);
            max.floatValue = Mathf.Clamp(max.floatValue, minMax.min, minMax.max);
        }
    }
}
