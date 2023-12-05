using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace SpatialSys.UnitySDK
{
    public static class SpatialBridge
    {
        public static ICameraService cameraService;
        public static ICoreGUIService coreGUIService;
        public static IMarketplaceService marketplaceService;
        public static INetworkingService networkingService;

        public delegate int GetSpacePackageVersionDelegate();
        public static GetSpacePackageVersionDelegate GetSpacePackageVersion;

        public delegate bool IsInSandboxDelegate();
        public static IsInSandboxDelegate IsInSandbox;

        public delegate int GetLocalActorDelegate();
        public static GetLocalActorDelegate GetLocalActor;

        public delegate List<int> GetActorsDelegate();
        public static GetActorsDelegate GetActors;

        public struct ActorUserData
        {
            public bool exists;

            public string userID;
            public string displayName;
            public string username;
            public bool isSignedIn;
            public bool isSpaceAdmin;
            public int platform;
            public bool isTalking;
        }
        public delegate ActorUserData GetLocalActorUserDataDelegate();
        public static GetLocalActorUserDataDelegate GetLocalActorUserData;

        public delegate ActorUserData GetActorUserDataDelegate(int actorNumber);
        public static GetActorUserDataDelegate GetActorUserData;

        public delegate bool GetAvatarExistsDelegate(int actorNumber);
        public static GetAvatarExistsDelegate GetAvatarExists;

        public delegate void MoveLocalAvatarDelegate(Vector2 moveInput, bool sprint);
        public static MoveLocalAvatarDelegate MoveLocalAvatar;

        public delegate Vector3 GetLocalAvatarPositionDelegate();
        public static GetLocalAvatarPositionDelegate GetLocalAvatarPosition;

        public delegate void SetLocalAvatarPositionDelegate(Vector3 position);
        public static SetLocalAvatarPositionDelegate SetLocalAvatarPosition;

        public delegate void SetLocalAvatarPositionRotationDelegate(Vector3 position, Quaternion rotation);
        public static SetLocalAvatarPositionRotationDelegate SetLocalAvatarPositionRotation;

        public delegate void SendLocalAvatarToSeatDelegate(Transform seat);
        public static SendLocalAvatarToSeatDelegate SendLocalAvatarToSeat;

        public delegate void SetLocalAvatarToStandDelegate();
        public static SetLocalAvatarToStandDelegate SetLocalAvatarToStand;

        public delegate Quaternion GetLocalAvatarRotationDelegate();
        public static GetLocalAvatarRotationDelegate GetLocalAvatarRotation;

        public delegate Vector3 GetLocalAvatarVelocityDelegate();
        public static GetLocalAvatarVelocityDelegate GetLocalAvatarVelocity;

        public delegate void SetLocalAvatarVelocityDelegate(Vector3 velocity);
        public static SetLocalAvatarVelocityDelegate SetLocalAvatarVelocity;

        public delegate bool GetLocalAvatarGroundedDelegate();
        public static GetLocalAvatarGroundedDelegate GetLocalAvatarGrounded;

        public delegate Vector3 GetAvatarPositionWithActorDelegate(int actorNumber);
        public static GetAvatarPositionWithActorDelegate GetAvatarPositionWithActor;

        public delegate Quaternion GetAvatarRotationWithActorDelegate(int actorNumber);
        public static GetAvatarRotationWithActorDelegate GetAvatarRotationWithActor;

        public delegate Vector3 GetAvatarVelocityWithActorDelegate(int actorNumber);
        public static GetAvatarVelocityWithActorDelegate GetAvatarVelocityWithActor;

        public delegate bool GetLocalAvatarBodyExistDelegate();
        public static GetLocalAvatarBodyExistDelegate GetLocalAvatarBodyExist;

        public delegate Transform GetLocalAvatarBoneTransformDelegate(HumanBodyBones humanBoneId);
        public static GetLocalAvatarBoneTransformDelegate GetLocalAvatarBoneTransform;

        public delegate Transform GetAvatarBoneTransformDelegate(int actorNumber, HumanBodyBones humanBoneId);
        public static GetAvatarBoneTransformDelegate GetAvatarBoneTransform;

        public delegate Material[] GetLocalAvatarMaterialsDelegate();
        public static GetLocalAvatarMaterialsDelegate GetLocalAvatarMaterials;

        public delegate Material[] GetAvatarMaterialsDelegate(int actorNumber);
        public static GetAvatarMaterialsDelegate GetAvatarMaterials;

        public delegate void GetActorProfilePictureDelegate(int actorNumber, Action<Texture2D> callback);
        public static GetActorProfilePictureDelegate GetActorProfilePicture;

        public delegate int GetRandomActorDelegate();
        public static GetRandomActorDelegate GetRandomActor;

        public delegate void AddForceToLocalAvatarDelegate(Vector3 force);
        public static AddForceToLocalAvatarDelegate AddForceToLocalAvatar;

        public delegate void SetLocalAvatarMovingSpeedDelegate(float speed);
        public static SetLocalAvatarMovingSpeedDelegate SetLocalAvatarMovingSpeed;

        public delegate float GetLocalAvatarMovingSpeedDelegate();
        public static GetLocalAvatarMovingSpeedDelegate GetLocalAvatarMovingSpeed;

        public delegate void SetLocalAvatarRunSpeedDelegate(float speed);
        public static SetLocalAvatarRunSpeedDelegate SetLocalAvatarRunSpeed;

        public delegate float GetLocalAvatarRunSpeedDelegate();
        public static GetLocalAvatarRunSpeedDelegate GetLocalAvatarRunSpeed;

        public delegate void SetLocalAvatarJumpHeightDelegate(float height);
        public static SetLocalAvatarJumpHeightDelegate SetLocalAvatarJumpHeight;

        public delegate float GetLocalAvatarJumpHeightDelegate();
        public static GetLocalAvatarJumpHeightDelegate GetLocalAvatarJumpHeight;

        public delegate void SetLocalAvatarGravityMultiplierDelegate(float multiplier);
        public static SetLocalAvatarGravityMultiplierDelegate SetLocalAvatarGravityMultiplier;

        public delegate float GetLocalAvatarGravityMultiplierDelegate();
        public static GetLocalAvatarGravityMultiplierDelegate GetLocalAvatarGravityMultiplier;

        public delegate void SetLocalAvatarFallingGravityMultiplierDelegate(float multiplier);
        public static SetLocalAvatarFallingGravityMultiplierDelegate SetLocalAvatarFallingGravityMultiplier;

        public delegate float GetLocalAvatarFallingGravityMultiplierDelegate();
        public static GetLocalAvatarFallingGravityMultiplierDelegate GetLocalAvatarFallingGravityMultiplier;

        public delegate void SetLocalAvatarUseVariableHeightJumpDelegate(bool useVariableHeightJump);
        public static SetLocalAvatarUseVariableHeightJumpDelegate SetLocalAvatarUseVariableHeightJump;

        public delegate bool GetLocalAvatarUseVariableHeightJumpDelegate();
        public static GetLocalAvatarUseVariableHeightJumpDelegate GetLocalAvatarUseVariableHeightJump;

        public delegate void SetLocalAvatarMaxJumpCountDelegate(int maxJumpCount);
        public static SetLocalAvatarMaxJumpCountDelegate SetLocalAvatarMaxJumpCount;

        public delegate int GetLocalAvatarMaxJumpCountDelegate();
        public static GetLocalAvatarMaxJumpCountDelegate GetLocalAvatarMaxJumpCount;

        public delegate void SetLocalAvatarGroundFrictionDelegate(float friction);
        public static SetLocalAvatarGroundFrictionDelegate SetLocalAvatarGroundFriction;

        public delegate float GetLocalAvatarGroundFrictionDelegate();
        public static GetLocalAvatarGroundFrictionDelegate GetLocalAvatarGroundFriction;

        public delegate void SetLocalAvatarAirControlDelegate(float airControl);
        public static SetLocalAvatarAirControlDelegate SetLocalAvatarAirControl;

        public delegate float GetLocalAvatarAirControlDelegate();
        public static GetLocalAvatarAirControlDelegate GetLocalAvatarAirControl;

        public delegate void TriggerJumpLocalAvatarDelegate();
        public static TriggerJumpLocalAvatarDelegate TriggerJumpLocalAvatar;

        public delegate void SetLocalAvatarFromPackageDelegate(string packageSKU);
        public static SetLocalAvatarFromPackageDelegate SetLocalAvatarFromPackage;

        public delegate void SetLocalAvatarFromEmbeddedDelegate(string assetID);
        public static SetLocalAvatarFromEmbeddedDelegate SetLocalAvatarFromEmbedded;

        public delegate void ResetLocalAvatarDelegate();
        public static ResetLocalAvatarDelegate ResetLocalAvatar;

        public delegate bool IsLocalOwnerDelegate();
        public static IsLocalOwnerDelegate IsLocalOwner;

        public delegate bool IsLocalHostDelegate();
        public static IsLocalHostDelegate IsLocalHost;

        public delegate bool HasLocalLovedSpaceDelegate();
        public static HasLocalLovedSpaceDelegate HasLocalLovedSpace;

        public delegate void OpenURLDelegate(string url);
        public static OpenURLDelegate OpenURL;

        public delegate void SendToastDelegate(string message, float duration);
        public static SendToastDelegate SendToast;

        public delegate void PlayLocalAvatarPackageEmoteDelegate(string sku, bool immediately = false, bool loop = false);
        public static PlayLocalAvatarPackageEmoteDelegate PlayLocalAvatarPackageEmote;
        public delegate void PlayLocalAvatarEmbeddedEmoteDelegate(string assetID, bool immediately = false, bool loop = false);
        public static PlayLocalAvatarEmbeddedEmoteDelegate PlayLocalAvatarEmbeddedEmote;
        public delegate void StopLocalAvatarEmoteAnimationDelegate();
        public static StopLocalAvatarEmoteAnimationDelegate StopLocalAvatarEmoteAnimation;

        public delegate void SpawnPrefabObjectFromPackageDelegate(string sku, Vector3 position, Quaternion rotation);
        public static SpawnPrefabObjectFromPackageDelegate SpawnPrefabObjectFromPackage;

        public delegate void SpawnPrefabObjectFromEmbeddedDelegate(string assetID, Vector3 position, Quaternion rotation);
        public static SpawnPrefabObjectFromEmbeddedDelegate SpawnPrefabObjectFromEmbedded;

        public delegate void EnableAvatarToAvatarCollisionsDelegate(bool enabled);
        public static EnableAvatarToAvatarCollisionsDelegate EnableAvatarToAvatarCollisions;

        public delegate bool GetLocalAvatarRagdollPhysicsActiveDelegate();
        public static GetLocalAvatarRagdollPhysicsActiveDelegate GetLocalAvatarRagdollPhysicsActive;

        public delegate void SetLocalAvatarRagdollPhysicsActiveDelegate(bool active, Vector3 initialVelocity);
        public static SetLocalAvatarRagdollPhysicsActiveDelegate SetLocalAvatarRagdollPhysicsActive;

        public delegate Vector3 GetLocalAvatarRagdollVelocityDelegate();
        public static GetLocalAvatarRagdollVelocityDelegate GetLocalAvatarRagdollVelocity;

        public delegate void SetLocalAvatarRagdollVelocityDelegate(Vector3 velocity);
        public static SetLocalAvatarRagdollVelocityDelegate SetLocalAvatarRagdollVelocity;

        public delegate void AddForceToLocalAvatarRagdollDelegate(Vector3 force, bool ignoreMass);
        public static AddForceToLocalAvatarRagdollDelegate AddForceToLocalAvatarRagdoll;

        // Avatar Attachments
        public delegate void EquipAvatarAttachmentPackageDelegate(string sku, bool equip, bool clearOccupiedPrimarySlot, string optionalTag, Action<bool> callback);
        public static EquipAvatarAttachmentPackageDelegate EquipAvatarAttachmentPackage;

        public delegate void EquipAvatarAttachmentItemDelegate(string itemID, bool equip, Action<bool> callback);
        public static EquipAvatarAttachmentItemDelegate EquipAvatarAttachmentItem;

        public delegate bool EquipAvatarAttachmentEmbeddedPackageAssetDelegate(string assetID, bool equip, bool clearOccupiedPrimarySlot, string optionalTag);
        public static EquipAvatarAttachmentEmbeddedPackageAssetDelegate EquipAvatarAttachmentEmbedded;

        public delegate bool IsAvatarAttachmentEquippedDelegate(string assetID);
        public static IsAvatarAttachmentEquippedDelegate IsAvatarAttachmentEquipped;

        public delegate void ClearAllAvatarAttachmentsDelegate();
        public static ClearAllAvatarAttachmentsDelegate ClearAllAvatarAttachments;

        public delegate void ClearAvatarAttachmentSlotDelegate(SpatialAvatarAttachment.Slot slot);
        public static ClearAvatarAttachmentSlotDelegate ClearAvatarAttachmentSlot;

        public delegate void ClearAvatarAttachmentsByTagDelegate(string tag);
        public static ClearAvatarAttachmentsByTagDelegate ClearAvatarAttachmentsByTag;

        // Quests
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

        public delegate void RewardBadgeDelegate(string badgeID);
        public static RewardBadgeDelegate RewardBadge;

        // Backpack
        public delegate void AddBackpackItemDelegate(string itemID, ulong quantity, bool showToast, Action<bool> callback);
        public static AddBackpackItemDelegate AddBackpackItem;

        public delegate void DeleteBackpackItemDelegate(string itemID, Action<bool> callback);
        public static DeleteBackpackItemDelegate DeleteBackpackItem;

        public struct GetBackpackItemResponse
        {
            public bool userOwnsItem;
            public ulong amount;
        }
        public delegate void GetBackpackItemDelegate(string itemID, Action<SpatialBridge.GetBackpackItemResponse> callback);
        public static GetBackpackItemDelegate GetBackpackItem;

        public delegate void UseBackpackItemDelegate(string itemID, Action<bool> callback);
        public static UseBackpackItemDelegate UseBackpackItem;

        public delegate void SetBackpackItemEnabledDelegate(string itemID, bool enabled, string disabledMessage);
        public static SetBackpackItemEnabledDelegate SetBackpackItemEnabled;

        public delegate void SetBackpackItemTypeEnabledDelegate(ItemType itemType, bool enabled, string disabledMessage);
        public static SetBackpackItemTypeEnabledDelegate SetBackpackItemTypeEnabled;

        // Consumables
        public struct GetConsumableItemStateResponse
        {
            public bool isActive;
            public float durationRemaining;
            public bool onCooldown;
            public float cooldownRemaining;
        }
        public delegate void GetConsumableItemStateDelegate(string itemID, Action<GetConsumableItemStateResponse> callback);
        public static GetConsumableItemStateDelegate GetConsumableItemState;

        // World Currency
        public delegate ulong GetWorldCurrencyBalanceDelegate();
        public static GetWorldCurrencyBalanceDelegate GetWorldCurrencyBalance;

        public delegate void AwardWorldCurrencyDelegate(ulong amount, Action<bool> callback);
        public static AwardWorldCurrencyDelegate AwardWorldCurrency;

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

        //Component Initialization
        public delegate void InitializeSpatialInteractableDelegate(SpatialInteractable spatialInteractable);
        public static InitializeSpatialInteractableDelegate InitializeSpatialInteractable;

        public delegate void InitializeSpatialPointOfInterestDelegate(SpatialPointOfInterest spatialPointOfInterest);
        public static InitializeSpatialPointOfInterestDelegate InitializeSpatialPointOfInterest;

        public delegate void PointOfInterestEnabledChangedDelegate(SpatialPointOfInterest spatialPointOfInterest, bool enabled);
        public static PointOfInterestEnabledChangedDelegate PointOfInterestEnabledChanged;

        public delegate bool GetIsSceneInitializedDelegate();
        public static GetIsSceneInitializedDelegate GetIsSceneInitialized;

        public delegate void InitializeSpatialSeatHotspotDelegate(SpatialSeatHotspot spatialHotspot);
        public static InitializeSpatialSeatHotspotDelegate InitializeSpatialSeatHotspot;

        public delegate void InitializeSpatialAvatarTeleporterDelegate(SpatialAvatarTeleporter spatialAvatarTeleporter);
        public static InitializeSpatialAvatarTeleporterDelegate InitializeSpatialAvatarTeleporter;

        public delegate void InitializeSpatialTriggerEventDelegate(SpatialTriggerEvent spatialTriggerEvent);
        public static InitializeSpatialTriggerEventDelegate InitializeSpatialTriggerEvent;

        public delegate void TriggerEventEnabledChangedDelegate(SpatialTriggerEvent spatialTriggerEvent, bool enabled);
        public static TriggerEventEnabledChangedDelegate TriggerEventEnabledChanged;

        public delegate void InitializeSpatialClimbableDelegate(SpatialClimbable climbable);
        public static InitializeSpatialClimbableDelegate InitializeSpatialClimbable;

        public delegate void InitializeSpatialCameraPassthroughDelegate(SpatialCameraPassthrough spatialCameraPassthrough);
        public static InitializeSpatialCameraPassthroughDelegate InitializeSpatialCameraPassthrough;

        public delegate void SetLocalActorNametagSubtextDelegate(string subtext);
        public static SetLocalActorNametagSubtextDelegate SetLocalActorNametagSubtext;

        public delegate void SetLocalActorNametagBarVisibleDelegate(bool visible);
        public static SetLocalActorNametagBarVisibleDelegate SetLocalActorNametagBarVisible;

        public delegate void SetLocalActorNametagBarValueDelegate(float value);
        public static SetLocalActorNametagBarValueDelegate SetLocalActorNametagBarValue;

        public delegate string GetActorNametagDisplayNameDelegate(int actor);
        public static GetActorNametagDisplayNameDelegate GetActorNametagDisplayName;

        public delegate string GetActorNametagSubtextDelegate(int actor);
        public static GetActorNametagSubtextDelegate GetActorNametagSubtext;

        public delegate bool GetActorNametagBarVisibleDelegate(int actor);
        public static GetActorNametagBarVisibleDelegate GetActorNametagBarVisible;

        public delegate float GetActorNametagBarValueDelegate(int actor);
        public static GetActorNametagBarValueDelegate GetActorNametagBarValue;

        public delegate void PlaySpatialSFXPositionDelegate(SpatialSFX sfx, Vector3 position, float extraVolume, float extraPitch);
        public static PlaySpatialSFXPositionDelegate PlaySpatialSFXPosition;

        public delegate void PlaySpatialSFXSourceDelegate(SpatialSFX sfx, AudioSource source, float extraVolume, float extraPitch);
        public static PlaySpatialSFXSourceDelegate PlaySpatialSFXSource;

        public delegate void CreateFloatingTextDelegate(string text, FloatingTextAnimStyle style, Vector3 position, Vector3 force, Color color, bool gravity, AnimationCurve scaleCurve, AnimationCurve alphaCurve, float lifetime);
        public static CreateFloatingTextDelegate CreateFloatingText;

        public delegate Action InitializeVirtualCameraDelegate(SpatialVirtualCamera virtualCamera);
        public static InitializeVirtualCameraDelegate InitializeSpatialVirtualCamera;

        // Data Stores
        public struct DataStoreOperationResult
        {
            public bool succeeded;
            public int responseCode;
            public object value; // only used for GetDataStoreVariableValue

            public DataStoreOperationResult(bool succeeded, int responseCode)
            {
                this.succeeded = succeeded;
                this.responseCode = responseCode;
                this.value = null;
            }

            public DataStoreOperationResult(bool succeeded, int responseCode, object value)
            {
                this.succeeded = succeeded;
                this.responseCode = responseCode;
                this.value = value;
            }
        }

        public delegate void GetDataStoreVariableValueDelegate(ClientBridge.DataStoreScope scope, string key, object defaultValue, Action<DataStoreOperationResult> callback);
        public static GetDataStoreVariableValueDelegate GetDataStoreVariableValue;

        public delegate void SetDataStoreVariableValueDelegate(ClientBridge.DataStoreScope scope, string key, object value, Action<DataStoreOperationResult> callback);
        public static SetDataStoreVariableValueDelegate SetDataStoreVariableValue;

        public delegate void DeleteDataStoreVariableDelegate(ClientBridge.DataStoreScope scope, string key, Action<DataStoreOperationResult> callback);
        public static DeleteDataStoreVariableDelegate DeleteDataStoreVariable;

        public delegate void ClearDataStoreDelegate(ClientBridge.DataStoreScope scope, Action<DataStoreOperationResult> callback);
        public static ClearDataStoreDelegate ClearDataStore;

        public delegate void HasDataStoreVariableDelegate(ClientBridge.DataStoreScope scope, string key, Action<DataStoreOperationResult> callback);
        public static HasDataStoreVariableDelegate HasDataStoreVariable;

        public delegate void DataStoreHasAnyVariableDelegate(ClientBridge.DataStoreScope scope, Action<DataStoreOperationResult> callback);
        public static DataStoreHasAnyVariableDelegate DataStoreHasAnyVariable;

        public delegate void DumpDataStoreVariablesDelegate(ClientBridge.DataStoreScope scope, Action<DataStoreOperationResult> callback);
        public static DumpDataStoreVariablesDelegate DumpDataStoreVariables;

        // Actor data
        public delegate object GetLocalActorCustomVariableDelegate(string name);
        public static GetLocalActorCustomVariableDelegate GetLocalActorCustomVariable;

        public delegate void SetLocalActorCustomVariableDelegate(string name, object value);
        public static SetLocalActorCustomVariableDelegate SetLocalActorCustomVariable;

        public delegate object GetActorCustomVariableDelegate(int actorID, string name);
        public static GetActorCustomVariableDelegate GetActorCustomVariable;
    }
}
