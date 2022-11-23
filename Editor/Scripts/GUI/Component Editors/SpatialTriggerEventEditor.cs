using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace SpatialSys.UnitySDK.Editor
{
    [CustomEditor(typeof(SpatialTriggerEvent))]
    public class SpatialTriggerEventEditor : SpatialComponentEditorBase
    {
        private bool initialized;

        private SerializedProperty version;
        private SerializedProperty listenFor;

        private SerializedProperty onEnterUnityEvent;
        private SerializedProperty onEnterIsSynced;
        private ReorderableList onEnterAnimatorEventsList;

        private SerializedProperty onExitUnityEvent;
        private SerializedProperty onExitIsSynced;
        private ReorderableList onExitAnimatorEventsList;

        public void OnSceneGUI()
        {
            var t = target as SpatialTriggerEvent;

            SpatialHandles.UnityEventHandle(t.transform, t.onEnterEvent.unityEvent);
            SpatialHandles.UnityEventHandle(t.transform, t.onExitEvent.unityEvent);
        }

        private void InitializeIfNecessary()
        {
            if (initialized && listenFor != null)
            {
                return;
            }

            initialized = true;
            serializedObject.Update();
            version = serializedObject.FindProperty(nameof(SpatialTriggerEvent.version));
            listenFor = serializedObject.FindProperty(nameof(SpatialTriggerEvent.listenFor));

            SerializedProperty onEnter = serializedObject.FindProperty(nameof(SpatialTriggerEvent.onEnterEvent));
            onEnterUnityEvent = onEnter.FindPropertyRelative(nameof(SpatialTriggerEvent.onEnterEvent.unityEvent));
            onEnterIsSynced = onEnter.FindPropertyRelative(nameof(SpatialTriggerEvent.onEnterEvent.unityEventIsSynced));
            SerializedProperty onEnterAnimatorEvent = onEnter.FindPropertyRelative(nameof(SpatialTriggerEvent.onEnterEvent.animatorEvent));
            SerializedProperty onEnterAnimatorEvents = onEnterAnimatorEvent.FindPropertyRelative(nameof(AnimatorEvent.events));
            onEnterAnimatorEventsList = new ReorderableList(serializedObject, onEnterAnimatorEvents, true, true, true, true);
            onEnterAnimatorEventsList.elementHeight = EditorGUIUtility.singleLineHeight * 2 + 3;
            onEnterAnimatorEventsList.drawHeaderCallback = (rect) => {
                EditorGUI.LabelField(rect, "Animator Events");
            };
            onEnterAnimatorEventsList.drawElementCallback = (rect, index, isActive, isFocused) => {
                EditorGUI.PropertyField(rect, onEnterAnimatorEvents.GetArrayElementAtIndex(index));
            };

            SerializedProperty onExit = serializedObject.FindProperty(nameof(SpatialTriggerEvent.onExitEvent));
            onExitUnityEvent = onExit.FindPropertyRelative(nameof(SpatialTriggerEvent.onExitEvent.unityEvent));
            onExitIsSynced = onExit.FindPropertyRelative(nameof(SpatialTriggerEvent.onExitEvent.unityEventIsSynced));
            SerializedProperty onExitAnimatorEvent = onExit.FindPropertyRelative(nameof(SpatialTriggerEvent.onExitEvent.animatorEvent));
            SerializedProperty onExitAnimatorEvents = onExitAnimatorEvent.FindPropertyRelative(nameof(AnimatorEvent.events));
            onExitAnimatorEventsList = new ReorderableList(serializedObject, onExitAnimatorEvents, true, true, true, true);
            onExitAnimatorEventsList.elementHeight = onEnterAnimatorEventsList.elementHeight;
            onExitAnimatorEventsList.drawHeaderCallback = (rect) => {
                EditorGUI.LabelField(rect, "Animator Events");
            };
            onExitAnimatorEventsList.drawElementCallback = (rect, index, isActive, isFocused) => {
                EditorGUI.PropertyField(rect, onExitAnimatorEvents.GetArrayElementAtIndex(index));
            };
        }

        public override void DrawFields()
        {
            InitializeIfNecessary();

            if (version.intValue < SpatialTriggerEvent.LATEST_VERSION)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("This component must be upgraded in order to function properly.", _warningStyle);
                    if (GUILayout.Button("Upgrade"))
                        UpgradeToLatest();
                }
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(10);
            }
            else
            {
                SpatialGUIUtility.AutoFoldout("TRIGGER_ON_ENTER", "On Enter", () => {
                    DrawEventGroup(onEnterIsSynced, onEnterUnityEvent, onEnterAnimatorEventsList);
                }, savePref: true, openByDefault: false);

                GUILayout.Space(10);

                SpatialGUIUtility.AutoFoldout("TRIGGER_ON_EXIT", "On Exit", () => {
                    DrawEventGroup(onExitIsSynced, onExitUnityEvent, onExitAnimatorEventsList);
                }, savePref: true, openByDefault: false);
            }
        }

        private void DrawEventGroup(SerializedProperty syncedField, SerializedProperty unityEvent, ReorderableList animatorEventsList)
        {
            // Unity Event
            GUILayout.Label("Unity Events", EditorStyles.whiteLabel);
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.BeginVertical();
            {
                EditorGUILayout.PropertyField(syncedField);
                EditorGUILayout.PropertyField(unityEvent);
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            // Animator Event
            GUILayout.Label("Animator Events", EditorStyles.whiteLabel);
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.BeginVertical();
            {
                animatorEventsList.DoLayoutList();
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private void UpgradeToLatest()
        {
            if (version.intValue == SpatialTriggerEvent.LATEST_VERSION)
                return;

            // Upgrade from previous versions
            if (version.intValue == 0)
            {
#pragma warning disable 0618
                var onEnterOld = serializedObject.FindProperty(nameof(SpatialTriggerEvent.deprecated_onEnter));
                CopySerializedProperty(onEnterOld, onEnterOld.propertyPath, serializedObject, onEnterUnityEvent.propertyPath);
                serializedObject.FindProperty($"{nameof(SpatialTriggerEvent.deprecated_onEnter)}.m_PersistentCalls.m_Calls").ClearArray();
                serializedObject.ApplyModifiedProperties();

                var onExitOld = serializedObject.FindProperty(nameof(SpatialTriggerEvent.deprecated_onExit));
                CopySerializedProperty(onExitOld, onExitOld.propertyPath, serializedObject, onExitUnityEvent.propertyPath);
                serializedObject.FindProperty($"{nameof(SpatialTriggerEvent.deprecated_onExit)}.m_PersistentCalls.m_Calls").ClearArray();
#pragma warning restore 0618
                version.intValue = 1;
            }

            version.intValue = SpatialTriggerEvent.LATEST_VERSION;
        }

        private static void CopySerializedProperty(SerializedProperty source, string sourceBasePath, SerializedObject destinationObj, string destinationBasePath)
        {
            SerializedProperty dest = destinationObj.FindProperty($"{destinationBasePath}{source.propertyPath.Substring(sourceBasePath.Length)}");
            if (source.hasChildren)
            {
                if (source.isArray)
                {
                    dest.ClearArray();

                    for (int i = 0; i < source.arraySize; i++)
                    {
                        dest.InsertArrayElementAtIndex(i);
                    }

                    for (int i = 0; i < source.arraySize; i++)
                    {
                        CopySerializedProperty(source.GetArrayElementAtIndex(i), sourceBasePath, destinationObj, destinationBasePath);
                    }
                }
                else
                {
                    SerializedProperty sourceChild = source.Copy();
                    sourceChild.Next(true);
                    do
                    {
                        CopySerializedProperty(sourceChild, sourceBasePath, destinationObj, destinationBasePath);
                    }
                    while (sourceChild.Next(false) && sourceChild.propertyPath.StartsWith(sourceBasePath));
                }
            }
            else
            {
                switch (source.propertyType)
                {
                    case SerializedPropertyType.Integer: dest.intValue = source.intValue; break;
                    case SerializedPropertyType.String: dest.stringValue = source.stringValue; break;
                    case SerializedPropertyType.Boolean: dest.boolValue = source.boolValue; break;
                    case SerializedPropertyType.Float: dest.floatValue = source.floatValue; break;
                    case SerializedPropertyType.Enum: dest.enumValueIndex = source.enumValueIndex; break;
                }
            }
        }
    }
}
