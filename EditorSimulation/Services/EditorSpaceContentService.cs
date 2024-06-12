using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using SpatialSys.UnitySDK.Editor;
using SpatialSys.UnitySDK.Internal;

namespace SpatialSys.UnitySDK.EditorSimulation
{
#pragma warning disable 67 // Disable unused event warning
    public class EditorSpaceContentService : ISpaceContentService
    {
        public bool isSceneInitialized => true;

        public IReadOnlyDictionary<int, IReadOnlySpaceObject> allObjects => new Dictionary<int, IReadOnlySpaceObject>();
        public IReadOnlyDictionary<int, IReadOnlyAvatar> avatars => new Dictionary<int, IReadOnlyAvatar>();
        public IReadOnlyDictionary<int, IReadOnlyPrefabObject> prefabs => new Dictionary<int, IReadOnlyPrefabObject>();
        public IReadOnlyDictionary<int, SpatialNetworkObject> networkObjects => new Dictionary<int, SpatialNetworkObject>();

        public event Action onSceneInitialized
        {
            add { value?.Invoke(); }
            remove { }
        }
        public event Action<IReadOnlySpaceObject> onObjectSpawned;
        public event Action<IReadOnlySpaceObject> onObjectDestroyed;

        public SpawnSpaceObjectRequest SpawnSpaceObject()
        {
            SpawnSpaceObjectRequest request = new();
            request.succeeded = false;
            request.InvokeCompletionEvent();
            return request;
        }

        public SpawnSpaceObjectRequest SpawnSpaceObject(Vector3 position, Quaternion rotation)
        {
            SpawnSpaceObjectRequest request = new();
            request.succeeded = false;
            request.InvokeCompletionEvent();
            return request;
        }

        public DestroySpaceObjectRequest DestroySpaceObject(int objectID)
        {
            DestroySpaceObjectRequest request = new();
            request.succeeded = false;
            request.InvokeCompletionEvent();
            return request;
        }

        public SpaceObjectOwnershipTransferRequest TakeOwnership(int objectID)
        {
            SpaceObjectOwnershipTransferRequest request = new();
            request.succeeded = false;
            request.InvokeCompletionEvent();
            return request;
        }

        public SpaceObjectOwnershipTransferRequest TransferOwnership(int objectID, int actorNumber)
        {
            SpaceObjectOwnershipTransferRequest request = new();
            request.succeeded = false;
            request.InvokeCompletionEvent();
            return request;
        }

        public bool ReleaseOwnership(int objectID)
        {
            return false;
        }

        public bool TryFindNetworkObject(int objectID, out SpatialNetworkObject networkObject)
        {
            networkObject = null;
            return false;
        }

        public SpawnNetworkObjectRequest SpawnNetworkObject(SpatialNetworkObject prefab, Vector3? position = null, Quaternion? rotation = null)
        {
            SpawnNetworkObjectRequest request = new();
            request.succeeded = false;
            request.InvokeCompletionEvent();
            return request;
        }

        public SpawnAvatarRequest SpawnAvatar(AssetType assetType, string assetID, Vector3 position, Quaternion rotation, string displayName)
        {
            SpawnAvatarRequest request = new();
            request.succeeded = false;
            request.InvokeCompletionEvent();
            return request;
        }

        public bool TryGetSpaceObjectID(GameObject gameObject, out int objectID)
        {
            objectID = 0;
            return false;
        }

        public int GetOwner(ISpatialComponentWithOwner component)
        {
            return SpatialBridge.actorService.localActorNumber;
        }

        public SpawnPrefabObjectRequest SpawnPrefabObject(AssetType assetType, string assetID, Vector3 position, Quaternion rotation)
        {
            SpawnPrefabObjectRequest request = new() {
                succeeded = false
            };
            switch (assetType)
            {
                case AssetType.Package:
                    SpatialBridge.loggingService.LogError($"{nameof(SpawnPrefabObject)}: Package loading not implemented");
                    break;
                case AssetType.EmbeddedAsset:
                    // We would need this to be an editor script
                    EmbeddedPackageAsset embeddedAsset = (ProjectConfig.activePackageConfig as SpaceConfig).embeddedPackageAssets.FirstOrDefault(package => package.id == assetID);
                    if (embeddedAsset != null)
                    {
                        if (embeddedAsset.asset is SpatialPrefabObject prefabObject)
                        {
                            var go = GameObject.Instantiate(prefabObject, position, rotation);
                            go.name = prefabObject.name;
                            request.succeeded = true;
                        }
                        else
                        {
                            SpatialBridge.loggingService.LogError($"{nameof(SpawnPrefabObject)}: Embedded asset not {assetID} is not a {nameof(SpatialPrefabObject)}");
                        }
                    }
                    else
                    {
                        SpatialBridge.loggingService.LogError($"{nameof(SpawnPrefabObject)}: Embedded asset not found");
                    }

                    SpatialBridge.loggingService.LogError($"{nameof(SpawnPrefabObject)}: Embedded asset loading not implemented");
                    break;
                default:
                    SpatialBridge.loggingService.LogError($"{nameof(SpawnPrefabObject)}: Unsupported asset type {assetType}");
                    break;
            }
            request.InvokeCompletionEvent();
            return request;
        }

        //--------------------------------------------------------------------------------------------------------------
        // Public API - OBSOLETE
        //--------------------------------------------------------------------------------------------------------------

        #pragma warning disable 0618 // Disable obsolete warning
        public event ISpaceContentService.OnSyncedObjectInitializedDelegate onSyncedObjectInitialized
        {
            add { }
            remove { }
        }
        public event ISpaceContentService.OnSyncedObjectOwnerChangedDelegate onSyncedObjectOwnerChanged
        {
            add { }
            remove { }
        }
        public event ISpaceContentService.OnSyncedObjectVariableChangedDelegate onSyncedObjectVariableChanged
        {
            add { }
            remove { }
        }
        #pragma warning restore 0618 // Restore obsolete warning
        public bool TakeoverSyncedObjectOwnership(SpatialSyncedObject syncedObject) => SpatialBridge.spatialComponentService.TakeoverSyncedObjectOwnership(syncedObject);
        public SpatialSyncedObject GetSyncedObjectByID(int id) => SpatialBridge.spatialComponentService.GetSyncedObjectByID(id);
        public bool GetSyncedObjectIsSynced(SpatialSyncedObject syncedObject) => SpatialBridge.spatialComponentService.GetSyncedObjectIsSynced(syncedObject);
        public int GetSyncedObjectID(SpatialSyncedObject syncedObject) => SpatialBridge.spatialComponentService.GetSyncedObjectID(syncedObject);
        public int GetSyncedObjectOwner(SpatialSyncedObject syncedObject) => SpatialBridge.spatialComponentService.GetSyncedObjectOwner(syncedObject);
        public bool GetSyncedObjectHasControl(SpatialSyncedObject syncedObject) => SpatialBridge.spatialComponentService.GetSyncedObjectHasControl(syncedObject);
        public bool GetSyncedObjectIsLocallyOwned(SpatialSyncedObject syncedObject) => SpatialBridge.spatialComponentService.GetSyncedObjectIsLocallyOwned(syncedObject);

        public int GetOwnerActor(SpatialComponentBase component) => SpatialBridge.actorService.localActorNumber;

        public void SetSyncedAnimatorParameter(SpatialSyncedAnimator syncedAnimator, string parameterName, object value) => SpatialBridge.spatialComponentService.SetSyncedAnimatorParameter(syncedAnimator, parameterName, value);
        public void SetSyncedAnimatorTrigger(SpatialSyncedAnimator syncedAnimator, string triggerName) => SpatialBridge.spatialComponentService.SetSyncedAnimatorTrigger(syncedAnimator, triggerName);
    }
}