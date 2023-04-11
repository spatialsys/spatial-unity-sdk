using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
namespace SpatialSys.UnitySDK.Editor
{
    [CustomPropertyDrawer(typeof(SpatialQuest.Task))]
    public class QuestTaskInspector : UnityEditor.PropertyDrawer
    {
        private Dictionary<string, float> _heights = new Dictionary<string, float>();

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            SerializedProperty idProp = property.FindPropertyRelative(nameof(SpatialQuest.Task.id));
            SerializedProperty nameProp = property.FindPropertyRelative(nameof(SpatialQuest.Task.name));
            SerializedProperty taskTypeProp = property.FindPropertyRelative(nameof(SpatialQuest.Task.type));
            SerializedProperty progressStepsProp = property.FindPropertyRelative(nameof(SpatialQuest.Task.progressSteps));
            SerializedProperty taskMarkersProp = property.FindPropertyRelative(nameof(SpatialQuest.Task.taskMarkers));
            SerializedProperty onStartedEventProp = property.FindPropertyRelative(nameof(SpatialQuest.Task.onStartedEvent));
            SerializedProperty onCompletedEventProp = property.FindPropertyRelative(nameof(SpatialQuest.Task.onCompletedEvent));
            SerializedProperty onPreviouslyCompletedEventProp = property.FindPropertyRelative(nameof(SpatialQuest.Task.onPreviouslyCompleted));

            EditorGUI.BeginProperty(rect, label, property);

            float startY = rect.y;

            // Name/ID
            Rect nameRect = new Rect(rect.x, rect.y, rect.width * 0.85f - EditorGUIUtility.standardVerticalSpacing, EditorGUIUtility.singleLineHeight);
            Rect idRect = new Rect(rect.x + nameRect.width, rect.y, rect.width * 0.15f, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(nameRect, nameProp);
            GUI.enabled = false;
            EditorGUI.TextField(idRect, $"ID: {idProp.intValue}");
            GUI.enabled = true;
            rect.y += nameRect.height + EditorGUIUtility.standardVerticalSpacing;

            // Type
            Rect typeRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(typeRect, taskTypeProp);
            rect.y += typeRect.height + EditorGUIUtility.standardVerticalSpacing;

            // Progress steps (only for progress bar)
            if (taskTypeProp.enumValueIndex == (int)SpatialQuest.TaskType.ProgressBar)
            {
                Rect progressRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                progressRect = EditorGUI.IndentedRect(progressRect);
                EditorGUI.PropertyField(progressRect, progressStepsProp);
                rect.y += progressRect.height + EditorGUIUtility.standardVerticalSpacing;
            }

            // Task markers
            DrawProperty(ref rect, taskMarkersProp);

            // Events
            DrawProperty(ref rect, onStartedEventProp);
            DrawProperty(ref rect, onCompletedEventProp);
            DrawProperty(ref rect, onPreviouslyCompletedEventProp);

            EditorGUI.EndProperty();

            _heights[property.propertyPath] = rect.y - startY;
        }

        private void DrawProperty(ref Rect rect, SerializedProperty property)
        {
            float height = EditorGUI.GetPropertyHeight(property);
            rect.height = height;
            EditorGUI.PropertyField(rect, property);
            rect.y += height;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (_heights.ContainsKey(property.propertyPath))
                return _heights[property.propertyPath];
            return 50;
        }
    }
}