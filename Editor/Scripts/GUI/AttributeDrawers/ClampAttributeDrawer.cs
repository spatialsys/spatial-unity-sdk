using UnityEditor;
using UnityEngine;

namespace SpatialSys.UnitySDK.Editor
{
    [CanEditMultipleObjects]
    [CustomPropertyDrawer(typeof(ClampAttribute))]
    public class ClampAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMultipleDifferentValues;
            EditorGUI.PropertyField(position, prop, label);
            bool propChanged = EditorGUI.EndChangeCheck();

            if (propChanged)
            {
                var attr = (ClampAttribute)attribute;

                if (prop.propertyType == SerializedPropertyType.Integer)
                {
                    prop.intValue = Mathf.Clamp(prop.intValue, Mathf.RoundToInt(attr.min), Mathf.RoundToInt(attr.max));
                }
                else if (prop.propertyType == SerializedPropertyType.Float)
                {
                    prop.floatValue = Mathf.Clamp(prop.floatValue, attr.min, attr.max);
                }
            }
        }
    }
}
