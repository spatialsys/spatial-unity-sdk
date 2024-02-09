using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace SpatialSys.UnitySDK.Editor
{
    [CustomPropertyDrawer(typeof(SpatialQuest.Reward))]
    public class QuestRewardInspector : UnityEditor.PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            SerializedProperty typeProp = property.FindPropertyRelative(nameof(SpatialQuest.Reward.type));
            SerializedProperty idProp = property.FindPropertyRelative(nameof(SpatialQuest.Reward.id));

            EditorGUI.BeginProperty(rect, label, property);

            float startY = rect.y;

            // Type
            Rect typeRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(typeRect, typeProp);
            rect.y += typeRect.height + EditorGUIUtility.standardVerticalSpacing;

            // Reward type dropdown
            if (typeProp.enumValueIndex == (int)RewardType.Badge)
            {
                Rect idRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(idRect, idProp);
                rect.y += idRect.height + EditorGUIUtility.standardVerticalSpacing;
            }
            else if (typeProp.enumValueIndex == (int)RewardType.Item)
            {
                SerializedProperty amountProp = property.FindPropertyRelative(nameof(SpatialQuest.Reward.amount));

                Rect idRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(idRect, idProp);
                rect.y += idRect.height + EditorGUIUtility.standardVerticalSpacing;

                Rect amountRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(amountRect, amountProp);
                rect.y += amountRect.height + EditorGUIUtility.standardVerticalSpacing;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty typeProp = property.FindPropertyRelative(nameof(SpatialQuest.Reward.type));
            int lines = typeProp.enumValueIndex == (int)RewardType.Badge ? 2 : 3;
            return EditorGUIUtility.singleLineHeight * lines + EditorGUIUtility.standardVerticalSpacing * (lines - 1);
        }
    }
}