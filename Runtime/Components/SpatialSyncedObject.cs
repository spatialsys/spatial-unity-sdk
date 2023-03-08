using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SpatialSys.UnitySDK
{
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    public class SpatialSyncedObject : SpatialComponentBase
    {
        public override string prettyName => "Synced Object";
        public override string tooltip => "Used to create objects at runtime that are synced across clients.";
        public override string documentationURL => "https://docs.spatial.io/1149f36778494c8581988c4be47efab5";
        public override bool isExperimental => true;

        public bool syncTransform = true;
        public bool saveWithSpace = false;
        public bool destroyOnCreatorDisconnect = false;

        [HideInInspector] public string assetID; // unity prefab asset ID
        [HideInInspector] public string instanceID;

        // Delete any hidden SpatialSyncedVariables components when this component is deleted in the editor
        private void OnDestroy()
        {
            if (Application.isPlaying)
                return;

            if (TryGetComponent(out SpatialSyncedVariables variables))
            {
                DestroyImmediate(variables);
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Remove Synced Variables")]
        private void RemoveSyncedVariables()
        {
            if (TryGetComponent(out SpatialSyncedVariables variables))
            {
                DestroyImmediate(variables);
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            if (Application.isPlaying)
                return;

            PrefabInstanceStatus prefabInstanceStatus = PrefabUtility.GetPrefabInstanceStatus(this);
            PrefabAssetType prefabAssetType = PrefabUtility.GetPrefabAssetType(this);

            bool isPrefab = prefabAssetType != PrefabAssetType.NotAPrefab;
            bool isPrefabInstance = isPrefab && prefabInstanceStatus == PrefabInstanceStatus.Connected;
            bool isPrefabAsset = isPrefab && prefabInstanceStatus == PrefabInstanceStatus.NotAPrefab;

            // set assetID if it's a prefab or instance of prefab
            if (isPrefabAsset || isPrefabInstance)
            {
                string assetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObject);
                string assetGUID = AssetDatabase.AssetPathToGUID(assetPath);
                if (assetID != assetGUID)
                {
                    assetID = assetGUID;
                }
            }
            else
            {
                assetID = null;
            }

            // set instance ID if it's an instance in the scene
            if (isPrefabAsset)
            {
                instanceID = null;
            }
            else
            {
                HashSet<string> allInstanceIDs = new HashSet<string>();
                SpatialSyncedObject[] allInstanceSyncedObjects = GameObject.FindObjectsOfType<SpatialSyncedObject>();
                foreach (SpatialSyncedObject syncedObject in allInstanceSyncedObjects)
                {
                    if (syncedObject == this)
                        continue;

                    allInstanceIDs.Add(syncedObject.instanceID);
                }

                while (string.IsNullOrEmpty(instanceID) || allInstanceIDs.Contains(instanceID))
                {
                    instanceID = System.Guid.NewGuid().ToString();
                }
            }
        }
#endif
    }
}
