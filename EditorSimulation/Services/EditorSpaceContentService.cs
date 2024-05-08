using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using SpatialSys.UnitySDK.Editor;

namespace SpatialSys.UnitySDK.EditorSimulation
{
#pragma warning disable 67 // Disable unused event warning
    public class EditorSpaceContentService : ISpaceContentService
    {
        #region Scene
        public bool isSceneInitialized => true;

        public IReadOnlyDictionary<int, IReadOnlySpaceObject> allObjects => new Dictionary<int, IReadOnlySpaceObject>();
        public IReadOnlyDictionary<int, IReadOnlyAvatar> avatars => new Dictionary<int, IReadOnlyAvatar>();
        public IReadOnlyDictionary<int, IReadOnlyPrefabObject> prefabs => new Dictionary<int, IReadOnlyPrefabObject>();

        public event Action onSceneInitialized
        {
            add { value?.Invoke(); }
            remove { }
        }

        #endregion

        #region Synced Objects

        public event ISpaceContentService.OnSyncedObjectInitializedDelegate onSyncedObjectInitialized;

        public event ISpaceContentService.OnSyncedObjectOwnerChangedDelegate onSyncedObjectOwnerChanged;

        public event ISpaceContentService.OnSyncedObjectVariableChangedDelegate onSyncedObjectVariableChanged;
        public event Action<IReadOnlySpaceObject> onObjectSpawned;
        public event Action<IReadOnlySpaceObject> onObjectDestroyed;

        public SpawnSpaceObjectRequest SpawnSpaceObject()
        {
            SpawnSpaceObjectRequest request = new()
            {
                succeeded = false
            };
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

        public bool TakeoverSyncedObjectOwnership(SpatialSyncedObject syncedObject)
        {
            return syncedObject != null;
        }

        public SpatialSyncedObject GetSyncedObjectByID(int id)
        {
            return GameObject.FindObjectsOfType<SpatialSyncedObject>().FirstOrDefault(x => x.GetInstanceID() == id);
        }

        public bool GetSyncedObjectIsSynced(SpatialSyncedObject syncedObject)
        {
            return syncedObject != null;
        }

        public int GetSyncedObjectID(SpatialSyncedObject syncedObject)
        {
            return syncedObject.GetInstanceID();
        }

        public int GetSyncedObjectOwner(SpatialSyncedObject syncedObject)
        {
            return SpatialBridge.actorService.localActorNumber;
        }

        public bool GetSyncedObjectHasControl(SpatialSyncedObject syncedObject)
        {
            return syncedObject != null;
        }

        public bool GetSyncedObjectIsLocallyOwned(SpatialSyncedObject syncedObject)
        {
            return syncedObject != null;
        }

        #endregion

        #region SyncedAnimator

        public void SetSyncedAnimatorParameter(SpatialSyncedAnimator syncedAnimator, string parameterName, object value)
        {
            if (syncedAnimator == null)
            {
                return;
            }

            if (value is bool)
            {
                syncedAnimator.animator.SetBool(parameterName, (bool)value);
            }
            else if (value is int)
            {
                syncedAnimator.animator.SetInteger(parameterName, (int)value);
            }
            else if (value is float)
            {
                syncedAnimator.animator.SetFloat(parameterName, (float)value);
            }
            else
            {
                SpatialBridge.loggingService.LogError($"SetSyncedAnimatorParameter: Unsupported parameter type {value.GetType()}");
            }
        }

        public void SetSyncedAnimatorTrigger(SpatialSyncedAnimator syncedAnimator, string triggerName)
        {
            if (syncedAnimator == null)
            {
                return;
            }

            syncedAnimator.animator.SetTrigger(triggerName);
        }

        #endregion

        #region Avatar Attachments

        public int GetOwnerActor(SpatialComponentBase component)
        {
            return SpatialBridge.actorService.localActorNumber;
        }

        #endregion

        #region Prefab Objects
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
        #endregion
    }
}