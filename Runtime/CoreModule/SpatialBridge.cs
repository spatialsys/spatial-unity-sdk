using UnityEngine;

namespace SpatialSys.UnitySDK
{
    /// <summary>
    /// The main interface that provides access to Spatial services.
    /// This acts like a bridge between user code and Spatial core functionality.
    /// </summary>
    public static class SpatialBridge
    {
        /// <summary>
        /// Service for interacting with actors and users in the space.
        /// </summary>
        public static IActorService actorService;

        /// <summary>
        /// Service for handling audio and sound effects.
        /// </summary>
        public static IAudioService audioService;

        /// <summary>
        /// Service for handling badges.
        /// </summary>
        public static IBadgeService badgeService;

        /// <summary>
        /// Provides access to all camera related functionality: Main camera state, player camera settings,
        /// camera shake, and target overrides.
        /// </summary>
        public static ICameraService cameraService;

        /// <summary>
        /// Service for handling all UI related functionality.
        /// </summary>
        public static ICoreGUIService coreGUIService;

        /// <summary>
        /// Service to handle inventory and currency.
        /// </summary>
        public static IInventoryService inventoryService;

        /// <summary>
        /// Service for logging errors and messages to the console.
        /// </summary>
        public static ILoggingService loggingService;

        /// <summary>
        /// Service to handle item purchases on the store.
        /// </summary>
        public static IMarketplaceService marketplaceService;

        /// <summary>
        /// This service provides access to all the networking functionality in Spatial: connectivity, server management,
        /// matchmaking, remove events (RPCs/Messaging), etc.
        /// </summary>
        public static INetworkingService networkingService;

        /// <summary>
        /// This service provides access to the <c>Users</c> datastore for the current <c>world</c>. Spaces that belong to
        /// the same <c>world</c> share the same user world datastore.
        /// </summary>
        public static IUserWorldDataStoreService userWorldDataStoreService;

        /// <summary>
        /// A service for handling visual effects.
        /// </summary>
        public static IVFXService vfxService;

        #region Internal services

        /// <summary>
        /// Used to initialize Spatial components in the Unity SDK. This is an internal type and should not be used by developers,
        /// only on spatial components themselves.
        /// </summary>
        public static Internal.ISpatialComponentService spatialComponentService;
        #endregion

        #region ISpaceService

        public delegate int GetSpacePackageVersionDelegate();
        public static GetSpacePackageVersionDelegate GetSpacePackageVersion;

        public delegate bool IsInSandboxDelegate();
        public static IsInSandboxDelegate IsInSandbox;

        public delegate bool HasLocalLovedSpaceDelegate();
        public static HasLocalLovedSpaceDelegate HasLocalLovedSpace;

        public delegate void OpenURLDelegate(string url);
        public static OpenURLDelegate OpenURL;

        public delegate void EnableAvatarToAvatarCollisionsDelegate(bool enabled);
        public static EnableAvatarToAvatarCollisionsDelegate EnableAvatarToAvatarCollisions;

        #endregion

        #region IQuestSystemService
        public delegate void QuestDelegate(SpatialQuest quest);
        public static QuestDelegate StartQuest;
        public static QuestDelegate CompleteQuest;
        public static QuestDelegate ResetQuest;

        public delegate void QuestTaskDelegate(SpatialQuest quest, uint taskID);
        public static QuestTaskDelegate StartQuestTask;
        public static QuestTaskDelegate CompleteQuestTask;
        public delegate void QuestTaskProgressDelegate(SpatialQuest quest, uint taskID, int progress);
        public static QuestTaskProgressDelegate AddQuestTaskProgress;
        public static QuestTaskProgressDelegate SetQuestTaskProgress;

        public delegate int GetQuestTaskProgressDelegate(SpatialQuest quest, uint taskID);
        public static GetQuestTaskProgressDelegate GetQuestTaskProgress;

        public delegate int GetQuestStatusDelegate(SpatialQuest quest);
        public static GetQuestStatusDelegate GetQuestStatus;

        public delegate int GetQuestTaskStatusDelegate(SpatialQuest quest, uint taskID);
        public static GetQuestTaskStatusDelegate GetQuestTaskStatus;
        #endregion

        #region ISceneService

        // Avatar Attachments
        public delegate int GetActorFromAvatarAttachmentObjectDelegate(SpatialAvatarAttachment attachment);
        public static GetActorFromAvatarAttachmentObjectDelegate GetActorFromAvatarAttachmentObject;

        // Prefab Objects
        public delegate void SpawnPrefabObjectFromPackageDelegate(string sku, Vector3 position, Quaternion rotation);
        public static SpawnPrefabObjectFromPackageDelegate SpawnPrefabObjectFromPackage;

        public delegate void SpawnPrefabObjectFromEmbeddedDelegate(string assetID, Vector3 position, Quaternion rotation);
        public static SpawnPrefabObjectFromEmbeddedDelegate SpawnPrefabObjectFromEmbedded;

        //Synced objects
        public delegate bool TakeoverSyncedObjectOwnerhipDelegate(SpatialSyncedObject syncedObject);
        public static TakeoverSyncedObjectOwnerhipDelegate TakeoverSyncedObjectOwnership;

        public delegate SpatialSyncedObject GetSyncedObjectByIDDelegate(int id);
        public static GetSyncedObjectByIDDelegate GetSyncedObjectByID;

        public delegate bool GetSyncedObjectIsSyncedDelegate(SpatialSyncedObject syncedObject);
        public static GetSyncedObjectIsSyncedDelegate GetSyncedObjectIsSynced;

        public delegate int GetSyncedObjectIDDelegate(SpatialSyncedObject syncedObject);
        public static GetSyncedObjectIDDelegate GetSyncedObjectID;

        public delegate int GetSyncedObjectOwnerDelegate(SpatialSyncedObject syncedObject);
        public static GetSyncedObjectOwnerDelegate GetSyncedObjectOwner;

        public delegate bool GetSyncedObjectHasControlDelegate(SpatialSyncedObject syncedObject);
        public static GetSyncedObjectHasControlDelegate GetSyncedObjectHasControl;

        public delegate bool GetSyncedObjectIsLocallyOwnedDelegate(SpatialSyncedObject syncedObject);
        public static GetSyncedObjectIsLocallyOwnedDelegate GetSyncedObjectIsLocallyOwned;

        public delegate void SetSyncedAnimatorParameterDelegate(SpatialSyncedAnimator syncedAnimator, string parameterName, object value);
        public static SetSyncedAnimatorParameterDelegate SetSyncedAnimatorParameter;

        public delegate void SetSyncedAnimatorTriggerDelegate(SpatialSyncedAnimator syncedAnimator, string triggerName);
        public static SetSyncedAnimatorTriggerDelegate SetSyncedAnimatorTrigger;

        #endregion

        #region IInputService
        public delegate void SetInputOverridesDelegate(bool movementOverride, bool jumpOverride, bool sprintOverride, bool actionButtonOverride, GameObject target);
        public static SetInputOverridesDelegate SetInputOverrides;

        public delegate void OnInputGraphRootObjectDestroyedDelegate(GameObject target);
        public static OnInputGraphRootObjectDestroyedDelegate OnInputGraphRootObjectDestroyed;

        public delegate void StartVehicleInputCaptureDelegate(VehicleInputFlags flags, Sprite primaryButtonSprite, Sprite secondaryButtonSprite, GameObject target);
        public static StartVehicleInputCaptureDelegate StartVehicleInputCapture;

        public delegate void StartCompleteCustomInputCaptureDelegate(GameObject target);
        public static StartCompleteCustomInputCaptureDelegate StartCompleteCustomInputCapture;

        public delegate void ReleaseInputCaptureDelegate(GameObject target);
        public static ReleaseInputCaptureDelegate ReleaseInputCapture;
        #endregion


        public delegate bool GetIsSceneInitializedDelegate();
        public static GetIsSceneInitializedDelegate GetIsSceneInitialized;
    }
}
