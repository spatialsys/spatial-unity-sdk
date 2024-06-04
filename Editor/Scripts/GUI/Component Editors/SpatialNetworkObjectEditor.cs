using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SpatialSys.UnitySDK.Editor
{
    [CustomEditor(typeof(SpatialNetworkObject))]
    public class SpatialNetworkObjectEditor : SpatialComponentEditorBase
    {
        private UnityEditor.Editor _variablesEditor;
        private SerializedProperty _objectFlagsProp;
        private SerializedProperty _syncFlagsProp;
        private GameObject _targetGameObject;

        private void InitializePropertiesIfNecessary()
        {
            if (_objectFlagsProp != null)
            {
                return;
            }

            _objectFlagsProp = serializedObject.FindProperty(nameof(SpatialNetworkObject.objectFlags));
            _syncFlagsProp = serializedObject.FindProperty(nameof(SpatialNetworkObject.syncFlags));
        }

        private void OnEnable()
        {
            if (target != null)
            {
                _targetGameObject = (target as SpatialNetworkObject).gameObject;
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
                    // Check target object really doesn't have a SpatialNetworkObject
                    if (_targetGameObject.TryGetComponent<SpatialNetworkObject>(out SpatialNetworkObject obj))
                    {
                        return;
                    }
                    // Delete any hidden SpatialNetworkVariables components when network object component is deleted in the editor
                    if (_targetGameObject.TryGetComponent<SpatialNetworkVariables>(out SpatialNetworkVariables variables))
                    {
                        DestroyImmediate(variables);
                    }
                }
            }
        }

        public override void DrawFields()
        {
            InitializePropertiesIfNecessary();
            SpatialNetworkObject networkObject = target as SpatialNetworkObject;

            EditorGUILayout.PropertyField(_objectFlagsProp);
            EditorGUILayout.PropertyField(_syncFlagsProp);

            // Create a rigidbody if the sync flags include Rigidbody and the object doesn't have one
            if (GUI.changed)
            {
                NetworkObjectSyncFlags flags = (NetworkObjectSyncFlags)_syncFlagsProp.intValue;
                if (flags.HasFlag(NetworkObjectSyncFlags.Rigidbody) && !networkObject.TryGetComponent(out Rigidbody rb))
                    networkObject.gameObject.AddComponent<Rigidbody>();
            }

            // These can only be enabled in specific cases: if it's a prefab
            bool isOnPrefab = (networkObject.gameObject.scene.name == null || !networkObject.gameObject.scene.name.Equals(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name));
            SpaceObjectFlags objectFlags = (SpaceObjectFlags)_objectFlagsProp.intValue;
            bool destroyOnDisconnectAllowed = isOnPrefab;
            if (!Application.isPlaying && !destroyOnDisconnectAllowed && 
                (objectFlags.HasFlag(SpaceObjectFlags.DestroyWhenCreatorLeaves) || objectFlags.HasFlag(SpaceObjectFlags.DestroyWhenOwnerLeaves)))
            {
                networkObject.objectFlags &= ~SpaceObjectFlags.DestroyWhenCreatorLeaves;
                networkObject.objectFlags &= ~SpaceObjectFlags.DestroyWhenOwnerLeaves;
                _objectFlagsProp.serializedObject.Update();
                UnityEditor.EditorUtility.SetDirty(networkObject);
            }

            GUILayout.Space(8);

            // Embed the network variables inspector
            if (networkObject.TryGetComponent(out SpatialNetworkVariables networkVariables))
            {
                if (_variablesEditor == null || _variablesEditor.target != networkVariables)
                    _variablesEditor = UnityEditor.Editor.CreateEditor(networkVariables);

                _variablesEditor.OnInspectorGUI();
            }
            else
            {
                // No network Variables
                if (GUILayout.Button("Add Visual Scripting Network Variables", new GUILayoutOption[] { GUILayout.Height(32) }))
                {
                    networkObject.gameObject.AddComponent<SpatialNetworkVariables>();
                    UnityEditor.EditorUtility.SetDirty(networkObject.gameObject);
                }
            }
        }
    }
}
