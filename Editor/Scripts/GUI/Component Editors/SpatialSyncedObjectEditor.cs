using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SpatialSys.UnitySDK.Editor
{
    [CustomEditor(typeof(SpatialSyncedObject))]
    public class SpatialSyncedObjectEditor : SpatialComponentEditorBase
    {
        private UnityEditor.Editor _variablesEditor;
        private SerializedProperty _syncTransformProp;
        private SerializedProperty _syncRigidbodyProp;
        private SerializedProperty _saveWithSpaceProp;
        private SerializedProperty _destroyOnCreatorDisconnectProp;
        private SerializedProperty _destroyOnOwnerDisconnectProp;
        private SerializedProperty _isMasterClientObjectProp;
        private GameObject _targetGameObject;

        private void InitializePropertiesIfNecessary()
        {
            if (_syncTransformProp != null)
            {
                return;
            }

            _syncTransformProp = serializedObject.FindProperty(nameof(SpatialSyncedObject.syncTransform));
            _syncRigidbodyProp = serializedObject.FindProperty(nameof(SpatialSyncedObject.syncRigidbody));
            _saveWithSpaceProp = serializedObject.FindProperty(nameof(SpatialSyncedObject.saveWithSpace));
            _destroyOnCreatorDisconnectProp = serializedObject.FindProperty(nameof(SpatialSyncedObject.destroyOnCreatorDisconnect));
            _destroyOnOwnerDisconnectProp = serializedObject.FindProperty(nameof(SpatialSyncedObject.destroyOnOwnerDisconnect));
            _isMasterClientObjectProp = serializedObject.FindProperty(nameof(SpatialSyncedObject.isMasterClientObject));
        }

        private void OnEnable()
        {
            if (target != null)
            {
                _targetGameObject = (target as SpatialSyncedObject).gameObject;
            }
            else
            {
                _targetGameObject = null;
            }
        }

        private void OnDisable()
        {
            // when target is null here, it's been destroyed
            if (target == null)
            {
                if (_targetGameObject != null)
                {
                    // Check target object really doesn't have a SpatialSyncedObject
                    if (_targetGameObject.TryGetComponent<SpatialSyncedObject>(out SpatialSyncedObject obj))
                    {
                        return;
                    }
                    // Delete any hidden SpatialSyncedVariables components when synced object component is deleted in the editor
                    if (_targetGameObject.TryGetComponent<SpatialSyncedVariables>(out SpatialSyncedVariables variables))
                    {
                        DestroyImmediate(variables);
                    }
                }
            }
        }

        public override void DrawFields()
        {
            InitializePropertiesIfNecessary();
            SpatialSyncedObject syncedObject = target as SpatialSyncedObject;

            if (syncedObject.gameObject.GetComponentsInParent<SpatialSyncedObject>().Length > 1)
            {
                SpatialGUIUtility.HelpBox("A Synced Object can not be the child of another Synced Object", SpatialGUIUtility.HelpSectionType.Error);
            }

            EditorGUILayout.PropertyField(_syncTransformProp);
            
            if (syncedObject.GetComponent<Rigidbody>() == null)
            {
                GUI.enabled = false;
                EditorGUILayout.PropertyField(_syncRigidbodyProp);
                syncedObject.syncRigidbody = false;
                GUI.enabled = true;
            }
            else
            {
                EditorGUILayout.PropertyField(_syncRigidbodyProp);
            }

            EditorGUILayout.PropertyField(_saveWithSpaceProp);

            bool isOnPrefab = (syncedObject.gameObject.scene.name == null || !syncedObject.gameObject.scene.name.Equals(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name));

            // These can only be enabled in specific cases: if it's a prefab or if saveWithSpace is false
            bool destroyOnDisconnectAllowed = !Application.isPlaying && isOnPrefab && !syncedObject.saveWithSpace;
            if (!destroyOnDisconnectAllowed)
            {
                GUI.enabled = destroyOnDisconnectAllowed;
                syncedObject.destroyOnCreatorDisconnect = false;
                syncedObject.destroyOnOwnerDisconnect = false;
            }

            //we are a prefab. Show the destroy option
            EditorGUILayout.PropertyField(_destroyOnCreatorDisconnectProp);
            EditorGUILayout.PropertyField(_destroyOnOwnerDisconnectProp);
            GUI.enabled = true;
            EditorGUILayout.PropertyField(_isMasterClientObjectProp);

            GUILayout.Space(8);

            //Embed the synced variables inspector
            if (syncedObject.TryGetComponent(out SpatialSyncedVariables syncedVariables))
            {
                if (_variablesEditor == null || _variablesEditor.target != syncedVariables)
                {
                    _variablesEditor = UnityEditor.Editor.CreateEditor(syncedVariables);
                }
                _variablesEditor.OnInspectorGUI();
            }
            else
            {
                //No synced Variables
                if (GUILayout.Button("Add Synced Variables", new GUILayoutOption[] { GUILayout.Height(32) }))
                {
                    syncedObject.gameObject.AddComponent<SpatialSyncedVariables>();
                    UnityEditor.EditorUtility.SetDirty(syncedObject.gameObject);
                }
            }
        }
    }
}
