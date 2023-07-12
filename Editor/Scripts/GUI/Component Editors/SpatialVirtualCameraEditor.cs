using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SpatialSys.UnitySDK.Editor
{
    [CustomEditor(typeof(SpatialVirtualCamera))]
    public class SpatialVirtualCameraEditor : SpatialComponentEditorBase
    {
        private SerializedProperty _priority;
        private SerializedProperty _fieldOfView;
        private SerializedProperty _nearClipPlane;
        private SerializedProperty _farClipPlane;

        private SpatialVirtualCamera[] _foundCameras;

        private void OnEnable()
        {
            _priority = serializedObject.FindProperty("_priority");
            _fieldOfView = serializedObject.FindProperty("_fieldOfView");
            _nearClipPlane = serializedObject.FindProperty("_nearClipPlane");
            _farClipPlane = serializedObject.FindProperty("_farClipPlane");

            _foundCameras = FindObjectsOfType<SpatialVirtualCamera>();
        }

        public override void DrawFields()
        {
            serializedObject.Update();
            SpatialVirtualCamera targetComponent = target as SpatialVirtualCamera;

            //provide a reactive tooltip that helps explain how the camera works.
            if (!targetComponent.gameObject.activeInHierarchy)
            {
                SpatialGUIUtility.HelpBox("Inactive", "This camera is inactive because the game object is inactive", SpatialGUIUtility.HelpSectionType.Info);
            }
            else if (_foundCameras.Any(c => c.priority > targetComponent.priority))
            {
                SpatialGUIUtility.HelpBox("Inactive", "This camera is inactive because there is another camera with a higher priority", SpatialGUIUtility.HelpSectionType.Warning);
            }
            else if (_foundCameras.Any(c => c.priority == targetComponent.priority && c != targetComponent))
            {
                SpatialGUIUtility.HelpBox("Identical Priority", "This camera has the same priority as another active camera!", SpatialGUIUtility.HelpSectionType.Error);
            }
            else
            {
                SpatialGUIUtility.HelpBox("Active", "This camera is active", SpatialGUIUtility.HelpSectionType.Success);
            }

            EditorGUILayout.PropertyField(_priority, new GUIContent("Priority"));
            EditorGUILayout.Space(20f);
            EditorGUILayout.PropertyField(_fieldOfView, new GUIContent("Field of View"));
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_nearClipPlane, new GUIContent("Near Clip Plane"));
            EditorGUILayout.PropertyField(_farClipPlane, new GUIContent("Far Clip Plane"));

            targetComponent.priority = Mathf.Max(-1000, targetComponent.priority);
            targetComponent.nearClipPlane = Mathf.Max(0.1f, targetComponent.nearClipPlane);
        }
    }
}
