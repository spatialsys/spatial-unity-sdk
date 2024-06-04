using System;

namespace SpatialSys.UnitySDK.Internal
{
    /// <summary>
    /// This interface is used to initialize Spatial components in the Unity SDK. This is an internal type and should not
    /// be used by developers, only on spatial components themselves.
    /// </summary>
    [InternalType]
    public interface ISpatialComponentService
    {
        event Action<SpatialInteractable> onInitializeInteractable;
        event Action<SpatialPointOfInterest> onInitializePointOfInterest;
        event Action<SpatialTriggerEvent> onInitializeTriggerEvent;

        void InitializeInteractable(SpatialInteractable spatialInteractable);

        void InitializePointOfInterest(SpatialPointOfInterest spatialPointOfInterest);

        void PointOfInterestEnabledChanged(SpatialPointOfInterest spatialPointOfInterest, bool enabled);

        void InitializeSeatHotspot(SpatialSeatHotspot spatialHotspot);

        void InitializeAvatarTeleporter(SpatialAvatarTeleporter spatialAvatarTeleporter);

        void InitializeTriggerEvent(SpatialTriggerEvent spatialTriggerEvent);

        void TriggerEventEnabledChanged(SpatialTriggerEvent spatialTriggerEvent, bool enabled);

        void InitializeClimbable(SpatialClimbable climbable);

        void InitializeCameraPassthrough(SpatialCameraPassthrough spatialCameraPassthrough);

        Action InitializeVirtualCamera(SpatialVirtualCamera virtualCamera);

        // Network Object
        event OnNetworkObjectOwnerChangedDelegate onNetworkObjectOwnerChanged;
        public delegate void OnNetworkObjectOwnerChangedDelegate(SpatialNetworkObject networkObject, int newOwnerActor);
        event OnNetworkObjectSpawnedDelegate onNetworkObjectSpawned;
        public delegate void OnNetworkObjectSpawnedDelegate(SpatialNetworkObject networkObject);
        event OnNetworkObjectDestroyedDelegate onNetworkObjectDespawned;
        public delegate void OnNetworkObjectDestroyedDelegate(SpatialNetworkObject networkObject);
        event OnNetworkVariableChangedDelegate onNetworkVariableChanged;
        public delegate void OnNetworkVariableChangedDelegate(SpatialNetworkVariables networkVariables, string variableName, object newValue);

        // Synced Objects
        event OnSyncedObjectInitializedDelegate onSyncedObjectInitialized;
        public delegate void OnSyncedObjectInitializedDelegate(SpatialSyncedObject syncedObject);
        event OnSyncedObjectOwnerChangedDelegate onSyncedObjectOwnerChanged;
        public delegate void OnSyncedObjectOwnerChangedDelegate(SpatialSyncedObject syncedObject, int newOwnerActor);
        bool TakeoverSyncedObjectOwnership(SpatialSyncedObject syncedObject);
        SpatialSyncedObject GetSyncedObjectByID(int id);
        bool GetSyncedObjectIsSynced(SpatialSyncedObject syncedObject);
        int GetSyncedObjectID(SpatialSyncedObject syncedObject);
        int GetSyncedObjectOwner(SpatialSyncedObject syncedObject);
        bool GetSyncedObjectHasControl(SpatialSyncedObject syncedObject);
        bool GetSyncedObjectIsLocallyOwned(SpatialSyncedObject syncedObject);

        // SyncedAnimator
        void SetSyncedAnimatorParameter(SpatialSyncedAnimator syncedAnimator, string parameterName, object value);
        void SetSyncedAnimatorTrigger(SpatialSyncedAnimator syncedAnimator, string triggerName);
    }
}
