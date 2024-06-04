using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace SpatialSys.UnitySDK.Editor
{
    [CustomPropertyDrawer(typeof(EmbeddedPackageAsset))]
    public class EmbeddedPackageAssetInspector : UnityEditor.PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            SerializedProperty idProp = property.FindPropertyRelative(nameof(EmbeddedPackageAsset.id));
            SerializedProperty assetProp = property.FindPropertyRelative(nameof(EmbeddedPackageAsset.asset));

            EditorGUI.BeginProperty(rect, label, property);

            // Draw properties side by side
            float halfWidth = (rect.width / 2) - EditorGUIUtility.standardVerticalSpacing;
            Rect idRect = new Rect(rect.x, rect.y, halfWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(idRect, idProp, GUIContent.none);
            Rect assetRect = new Rect(rect.x + halfWidth + EditorGUIUtility.standardVerticalSpacing, rect.y, halfWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(assetRect, assetProp, GUIContent.none);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}