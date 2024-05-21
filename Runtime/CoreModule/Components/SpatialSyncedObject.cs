using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SpatialSys.UnitySDK
{
    [DocumentationCategory("Core/Components")]
    [DisallowMultipleComponent]
    public class SpatialSyncedObject : SpatialComponentBase
    {
        public override string prettyName => "Synced Object";
        public override string tooltip => "Used to create objects at runtime that are synced across clients.";
        public override string documentationURL => "https://docs.spatial.io/components/synced-object";
        public override bool isExperimental => true;

        public bool syncTransform = true;
        public bool syncRigidbody = false;
        public bool saveWithSpace = false;

        [Tooltip("If checked, the object will be destroyed when the creator or the object disconnects.")]
        public bool destroyOnCreatorDisconnect = false;
        [Tooltip("If checked, the object will be destroyed when the current owner disconnects.")]
        public bool destroyOnOwnerDisconnect = false;
        [Tooltip("If checked, the owner will be fixed to the master client.")]
        public bool isMasterClientObject = false;

        [SerializeField]
        private string assetID; // unity prefab asset ID

        [SerializeField]
        private string instanceID;

        public string AssetID => assetID;
        public string InstanceID => instanceID;

        public delegate void OnOwnerChangedDelegate(int newOwner);
        public delegate void OnVariableChangedDelegate(string variableName, object newValue);

        private Action _onInitialized;
        private OnOwnerChangedDelegate _onOwnerChanged;
        private OnVariableChangedDelegate _onVariableChanged;
        private bool _handlingInit;
        private bool _handlingOwnerChange;
        private bool _handlingVariableChange;

        private void OnEnable()
        {
            if (_onInitialized != null && _onInitialized.GetInvocationList().Length > 0)
            {
                SpatialBridge.spaceContentService.onSyncedObjectInitialized += HandleSyncedObjectInitialized;
                _handlingInit = true;
            }
            if (_onOwnerChanged != null && _onOwnerChanged.GetInvocationList().Length > 0)
            {
                SpatialBridge.spaceContentService.onSyncedObjectOwnerChanged += HandleSyncedObjectOwnerChanged;
                _handlingOwnerChange = true;
            }
            if (_onVariableChanged != null && _onVariableChanged.GetInvocationList().Length > 0)
            {
                SpatialBridge.spaceContentService.onSyncedObjectVariableChanged += HandleSyncedObjectVariableChanged;
                _handlingVariableChange = true;
            }
        }

        private void OnDisable()
        {
            SpatialBridge.spaceContentService.onSyncedObjectInitialized -= HandleSyncedObjectInitialized;
            SpatialBridge.spaceContentService.onSyncedObjectOwnerChanged -= HandleSyncedObjectOwnerChanged;
            SpatialBridge.spaceContentService.onSyncedObjectVariableChanged -= HandleSyncedObjectVariableChanged;
        }

        //------------------------------------------------------------------------------------------------------------
        // Observers
        //------------------------------------------------------------------------------------------------------------

        private void HandleSyncedObjectInitialized(SpatialSyncedObject syncedObject)
        {
            if (syncedObject == this)
            {
                _onInitialized?.Invoke();
            }
        }

        private void HandleSyncedObjectOwnerChanged(SpatialSyncedObject syncedObject, int newOwner)
        {
            if (syncedObject == this)
            {
                _onOwnerChanged?.Invoke(newOwner);
            }
        }

        private void HandleSyncedObjectVariableChanged(SpatialSyncedVariables syncedVariables, string variableName, object newValue)
        {
            if (syncedVariables.gameObject == gameObject)
            {
                _onVariableChanged?.Invoke(variableName, newValue);
            }
        }

        //------------------------------------------------------------------------------------------------------------
        // Spatial Bridge shortcuts
        //------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Does this synced object have control?
        /// </summary>
        public bool hasControl => SpatialBridge.spaceContentService.GetSyncedObjectHasControl(this);

        /// <summary>
        /// Is this synced object locally owned?
        /// </summary>
        public bool isLocallyOwned => SpatialBridge.spaceContentService.GetSyncedObjectIsLocallyOwned(this);

        /// <summary>
        /// Is this synced object synced across clients?
        /// </summary>
        public bool isSynced => SpatialBridge.spaceContentService.GetSyncedObjectIsSynced(this);

        /// <summary>
        /// The ID of this synced object.
        /// </summary>
        public int syncedObjectID => SpatialBridge.spaceContentService.GetSyncedObjectID(this);

        /// <summary>
        /// The actor ID of the owner of this synced object.
        /// </summary>
        public int ownerActorID => SpatialBridge.spaceContentService.GetSyncedObjectOwner(this);

        /// <summary>
        /// Called when this synced object is initialized.
        /// </summary>
        public event Action onObjectInitialized
        {
            add
            {
                _onInitialized += value;
                if (!_handlingInit)
                {
                    SpatialBridge.spaceContentService.onSyncedObjectInitialized += HandleSyncedObjectInitialized;
                    _handlingInit = true;
                }
            }
            remove
            {
                _onInitialized -= value;
                if (_onInitialized.GetInvocationList().Length == 0)
                {
                    SpatialBridge.spaceContentService.onSyncedObjectInitialized -= HandleSyncedObjectInitialized;
                    _handlingInit = false;
                }
            }
        }

        /// <summary>
        /// Called when the owner of this synced object changes.
        /// </summary>
        public event OnOwnerChangedDelegate onOwnerChanged
        {
            add
            {
                _onOwnerChanged += value;
                if (!_handlingOwnerChange)
                {
                    SpatialBridge.spaceContentService.onSyncedObjectOwnerChanged += HandleSyncedObjectOwnerChanged;
                    _handlingOwnerChange = true;
                }
            }
            remove
            {
                _onOwnerChanged -= value;
                if (_onOwnerChanged.GetInvocationList().Length == 0)
                {
                    SpatialBridge.spaceContentService.onSyncedObjectOwnerChanged -= HandleSyncedObjectOwnerChanged;
                    _handlingOwnerChange = false;
                }
            }
        }

        /// <summary>
        /// Called when a synced variable on this synced object changes.
        /// </summary>
        public event OnVariableChangedDelegate onVariableChanged
        {
            add
            {
                _onVariableChanged += value;
                if (!_handlingVariableChange)
                {
                    SpatialBridge.spaceContentService.onSyncedObjectVariableChanged += HandleSyncedObjectVariableChanged;
                    _handlingVariableChange = true;
                }
            }
            remove
            {
                _onVariableChanged -= value;
                if (_onVariableChanged.GetInvocationList().Length == 0)
                {
                    SpatialBridge.spaceContentService.onSyncedObjectVariableChanged -= HandleSyncedObjectVariableChanged;
                    _handlingVariableChange = false;
                }
            }
        }

        /// <summary>
        /// Takeover ownership of this synced object.
        /// </summary>
        /// <returns>True if takeover was successful.</returns>
        public bool TakeoverOwnership()
        {
            return SpatialBridge.spaceContentService.TakeoverSyncedObjectOwnership(this);
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
            bool isChildOfPrefabObject = transform.GetComponentsInParent<SpatialPrefabObject>(true).Length > 0;

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
            // or embedded within a prefab object
            if (isPrefabAsset && !isChildOfPrefabObject)
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
