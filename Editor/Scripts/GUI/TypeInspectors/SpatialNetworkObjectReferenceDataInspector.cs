using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace SpatialSys.UnitySDK.Editor
{
    [CustomPropertyDrawer(typeof(SpatialNetworkObjectReferenceData))]
    public class SpatialNetworkObjectReferenceDataInspector : UnityEditor.PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            SerializedProperty networkObjectProp = property.FindPropertyRelative(nameof(SpatialNetworkObjectReferenceData.networkObject));
            SpatialNetworkObject networkObject = networkObjectProp.objectReferenceValue as SpatialNetworkObject;

            EditorGUI.BeginProperty(rect, label, property);

            // Draw properties side by side
            float halfWidth = (rect.width / 2) - EditorGUIUtility.standardVerticalSpacing;
            Rect referenceProp = new Rect(rect.x, rect.y, halfWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(referenceProp, networkObjectProp, GUIContent.none);
            Rect guidRect = new Rect(rect.x + halfWidth + EditorGUIUtility.standardVerticalSpacing, rect.y, halfWidth, EditorGUIUtility.singleLineHeight);
            string guidLabel = networkObject != null ? networkObject.networkPrefabGuid.ToString() : "none";
            EditorGUI.LabelField(guidRect, $"(GUID: {guidLabel})");

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}