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
                base.DrawFields();
            }
        }

        private void UpgradeToLatest()
        {
            if (version.intValue == SpatialTriggerEvent.LATEST_VERSION)
                return;

            // Upgrade from previous versions
            if (version.intValue == 0)
            {
#pragma warning disable 0618
                SerializedProperty onEnter = serializedObject.FindProperty(nameof(SpatialTriggerEvent.onEnterEvent));
                var onEnterUnityEvent = onEnter.FindPropertyRelative(nameof(SpatialTriggerEvent.onEnterEvent.unityEvent));
                
                var onEnterOld = serializedObject.FindProperty(nameof(SpatialTriggerEvent.deprecated_onEnter));
                CopySerializedProperty(onEnterOld, onEnterOld.propertyPath, serializedObject, onEnterUnityEvent.propertyPath);
                serializedObject.FindProperty($"{nameof(SpatialTriggerEvent.deprecated_onEnter)}.m_PersistentCalls.m_Calls").ClearArray();
                serializedObject.ApplyModifiedProperties();

                SerializedProperty onExit = serializedObject.FindProperty(nameof(SpatialTriggerEvent.onExitEvent));
                var onExitUnityEvent = onExit.FindPropertyRelative(nameof(SpatialTriggerEvent.onExitEvent.unityEvent));
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
                    case SerializedPropertyType.Integer:
                        dest.intValue = source.intValue;
                        break;
                    case SerializedPropertyType.String:
                        dest.stringValue = source.stringValue;
                        break;
                    case SerializedPropertyType.Boolean:
                        dest.boolValue = source.boolValue;
                        break;
                    case SerializedPropertyType.Float:
                        dest.floatValue = source.floatValue;
                        break;
                    case SerializedPropertyType.Enum:
                        dest.enumValueIndex = source.enumValueIndex;
                        break;
                }
            }
        }
    }
}
