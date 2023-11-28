using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using SpatialSys.UnitySDK.Internal;

namespace SpatialSys.UnitySDK.Editor
{
    [CustomPropertyDrawer(typeof(SpatialEvent))]
    public class SpatialEventInspector : UnityEditor.PropertyDrawer
    {
        public class PropertyCache
        {
            public float height;
            public SerializedObject serializedObject;
            public SerializedProperty unityEvent;
            public SerializedProperty unityEventCalls;
            public SerializedProperty isSynced;
            public ReorderableList animatorEventsList;
            public ReorderableList questEventsList;

            public int eventCount => animatorEventsList.count + unityEventCalls.arraySize + questEventsList.count;
        }

        private Dictionary<string, PropertyCache> _propCache = new Dictionary<string, PropertyCache>();

        private PropertyCache GetPropertyCache(SerializedProperty property)
        {
            if (!_propCache.ContainsKey(property.propertyPath))
            {
                var propertyCache = new PropertyCache();

                propertyCache.serializedObject = property.serializedObject;

                // Unity Events
                propertyCache.unityEvent = property.FindPropertyRelative(nameof(SpatialEvent.unityEvent));
                propertyCache.unityEventCalls = propertyCache.unityEvent.FindPropertyRelative("m_PersistentCalls.m_Calls");
                propertyCache.isSynced = property.FindPropertyRelative(nameof(SpatialEvent.unityEventIsSynced));

                // Animator Events
                SerializedProperty animatorEvent = property.FindPropertyRelative(nameof(SpatialEvent.animatorEvent));
                SerializedProperty animatorEvents = animatorEvent.FindPropertyRelative(nameof(AnimatorEvent.events));
                propertyCache.animatorEventsList = new ReorderableList(property.serializedObject, animatorEvents, true, true, true, true);
                propertyCache.animatorEventsList.drawHeaderCallback = (rect) => {
                    EditorGUI.LabelField(rect, animatorEvent.displayName);
                };
                propertyCache.animatorEventsList.drawElementCallback = (rect, index, isActive, isFocused) => {
                    EditorGUI.PropertyField(rect, animatorEvents.GetArrayElementAtIndex(index));
                };
                propertyCache.animatorEventsList.elementHeightCallback += idx => {
                    SerializedProperty elementProp = animatorEvents.GetArrayElementAtIndex(idx);
                    return EditorGUI.GetPropertyHeight(elementProp);
                };

                // Quest Events
                SerializedProperty questEvent = property.FindPropertyRelative(nameof(SpatialEvent.questEvent));
                SerializedProperty questEvents = questEvent.FindPropertyRelative(nameof(QuestEvent.events));
                propertyCache.questEventsList = new ReorderableList(property.serializedObject, questEvents, true, true, true, true);
                propertyCache.questEventsList.drawHeaderCallback = (rect) => {
                    EditorGUI.LabelField(rect, questEvent.displayName);
                };
                propertyCache.questEventsList.drawElementCallback = (rect, index, isActive, isFocused) => {
                    EditorGUI.PropertyField(rect, questEvents.GetArrayElementAtIndex(index));
                };
                propertyCache.questEventsList.elementHeightCallback += idx => {
                    SerializedProperty elementProp = questEvents.GetArrayElementAtIndex(idx);
                    return EditorGUI.GetPropertyHeight(elementProp);
                };
                _propCache.Add(property.propertyPath, propertyCache);
            }

            return _propCache[property.propertyPath];
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            PropertyCache propertyCache = GetPropertyCache(property);

            // Popup button
            string title = property.displayName;
            int count = propertyCache.eventCount;
            if (count > 0)
                title += $" ({count})";

            float startY = rect.y;

            // Draw foldout
            rect.height = EditorGUIUtility.singleLineHeight;
            property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(rect, property.isExpanded, title, EditorStyles.toolbarButton);
            {
                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * 2;

                if (property.isExpanded)
                {
                    rect.x += 8;
                    rect.width -= 8;

                    EditorGUI.PropertyField(rect, propertyCache.isSynced);
                    rect.y += EditorGUI.GetPropertyHeight(propertyCache.isSynced);

                    EditorGUI.PropertyField(rect, propertyCache.unityEvent);
                    rect.y += EditorGUI.GetPropertyHeight(propertyCache.unityEvent);

                    propertyCache.animatorEventsList.DoList(rect);
                    rect.y += propertyCache.animatorEventsList.GetHeight();

                    propertyCache.questEventsList.DoList(rect);
                    rect.y += propertyCache.questEventsList.GetHeight();
                }

                propertyCache.height = rect.y - startY;
            }
            EditorGUI.EndFoldoutHeaderGroup();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (_propCache.ContainsKey(property.propertyPath))
            {
                return _propCache[property.propertyPath].height;
            }
            return 50;
        }
    }
}