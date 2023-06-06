using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialSys.UnitySDK
{
    public class ClientBridge
    {
        public delegate int GetLocalActorDelegate();
        public static GetLocalActorDelegate GetLocalActor;

        public delegate List<int> GetActorsDelegate();
        public static GetActorsDelegate GetActors;

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

        public delegate Vector3 GetAvatarPositionWithActorDelegate(int actorNumber);
        public static GetAvatarPositionWithActorDelegate GetAvatarPositionWithActor;

        public delegate Quaternion GetAvatarRotationWithActorDelegate(int actorNumber);
        public static GetAvatarRotationWithActorDelegate GetAvatarRotationWithActor;

        public delegate bool GetLocalAvatarBodyExistDelegate();
        public static GetLocalAvatarBodyExistDelegate GetLocalAvatarBodyExist;

        public delegate Transform GetLocalAvatarBoneTransformDelegate(HumanBodyBones humanBoneId);
        public static GetLocalAvatarBoneTransformDelegate GetLocalAvatarBoneTransform;

        public delegate Transform GetAvatarBoneTransformDelegate(int actorNumber, HumanBodyBones humanBoneId);
        public static GetAvatarBoneTransformDelegate GetAvatarBoneTransform;

        public delegate int GetLocalActorPlatformDelegate();
        public static GetLocalActorPlatformDelegate GetLocalActorPlatform;

        public delegate void GetActorProfilePictureDelegate(int actorNumber, Action<Texture2D> callback);
        public static GetActorProfilePictureDelegate GetActorProfilePicture;

        public delegate string GetActorNameDelegate(int actorNumber);
        public static GetActorNameDelegate GetActorName;

        // bool = actorNumber is valid
        public delegate Tuple<bool, int> GetActorPlatformDelegate(int actorNumber);
        public static GetActorPlatformDelegate GetActorPlatform;

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

        public delegate void TriggerJumpLocalAvatarDelegate();
        public static TriggerJumpLocalAvatarDelegate TriggerJumpLocalAvatar;

        public delegate void SetLocalAvatarDelegate(string sku);
        public static SetLocalAvatarDelegate SetLocalAvatar;

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

        public delegate Vector3 GetCameraPositionDelegate();
        public static GetCameraPositionDelegate GetCameraPosition;

        public delegate Quaternion GetCameraRotationDelegate();
        public static GetCameraRotationDelegate GetCameraRotation;

        public delegate Vector3 GetCameraForwardDelegate();
        public static GetCameraForwardDelegate GetCameraForward;

        public delegate bool IsCameraRoomModeDelegate();
        public static IsCameraRoomModeDelegate IsCameraRoomMode;

        public delegate void SendToastDelegate(string message, float duration);
        public static SendToastDelegate SendToast;

        public delegate void PlayLocalAvatarEmoteAnimationDelegate(string sku, bool immediately = false);
        public static PlayLocalAvatarEmoteAnimationDelegate PlayLocalAvatarEmoteAnimation;

        public delegate void SpawnPrefabObjectDelegate(string sku, Vector3 position, Quaternion rotation);
        public static SpawnPrefabObjectDelegate SpawnPrefabObject;

        // Avatar Attachments
        public delegate void EquipAvatarAttachmentPackageDelegate(string sku, bool equip, Action<bool> callback);
        public static EquipAvatarAttachmentPackageDelegate EquipAvatarAttachmentPackage;

        public delegate void EquipAvatarAttachmentItemDelegate(string itemID, bool equip, Action<bool> callback);
        public static EquipAvatarAttachmentItemDelegate EquipAvatarAttachmentItem;

        public delegate bool IsAvatarAttachmentEquippedDelegate(string itemOrPackageID);
        public static IsAvatarAttachmentEquippedDelegate IsAvatarAttachmentEquipped;

        // Quests
        public delegate void QuestDelegate(SpatialQuest quest);
        public static QuestDelegate StartQuest;
        public static QuestDelegate CompleteQuest;
        public static QuestDelegate ResetQuest;

        public delegate void QuestTaskDelegate(SpatialQuest quest, uint taskID);
        public static QuestTaskDelegate StartQuestTask;
        public static QuestTaskDelegate AddQuestTaskProgress;
        public static QuestTaskDelegate CompleteQuestTask;

        public delegate int GetQuestTaskProgressDelegate(SpatialQuest quest, uint taskID);
        public static GetQuestTaskProgressDelegate GetQuestTaskProgress;

        public delegate int GetQuestStatusDelegate(SpatialQuest quest);
        public static GetQuestStatusDelegate GetQuestStatus;

        public delegate int GetQuestTaskStatusDelegate(SpatialQuest quest, uint taskID);
        public static GetQuestTaskStatusDelegate GetQuestTaskStatus;

        public delegate void RewardBadgeDelegate(string badgeID);
        public static RewardBadgeDelegate RewardBadge;

        // Backpack
        public delegate void SetBackpackMenuOpenDelegate(bool open);
        public static SetBackpackMenuOpenDelegate SetBackpackMenuOpen;

        public delegate void AddBackpackItemDelegate(string itemID, ulong quantity, Action<bool> callback);
        public static AddBackpackItemDelegate AddBackpackItem;

        public struct GetBackpackItemResponse
        {
            public bool userOwnsItem;
            public ulong amount;
        }
        public delegate void GetBackpackItemDelegate(string itemID, Action<ClientBridge.GetBackpackItemResponse> callback);
        public static GetBackpackItemDelegate GetBackpackItem;

        public delegate void UseBackpackItemDelegate(string itemID, Action<bool> callback);
        public static UseBackpackItemDelegate UseBackpackItem;

        public delegate void SetBackpackItemEnabledDelegate(string itemID, bool enabled, string disabledMessage);
        public static SetBackpackItemEnabledDelegate SetBackpackItemEnabled;

        public delegate void SetBackpackItemTypeEnabledDelegate(ItemType itemType, bool enabled, string disabledMessage);
        public static SetBackpackItemTypeEnabledDelegate SetBackpackItemTypeEnabled;

        // World Currency
        public delegate ulong GetWorldCurrencyBalanceDelegate();
        public static GetWorldCurrencyBalanceDelegate GetWorldCurrencyBalance;

        public delegate void AwardWorldCurrencyDelegate(ulong amount, Action<bool> callback);
        public static AwardWorldCurrencyDelegate AwardWorldCurrency;

        // Shop
        public delegate void SetShopMenuOpenDelegate(bool open);
        public static SetShopMenuOpenDelegate SetShopMenuOpen;

        public delegate void SelectShopMenuItemDelegate(string itemID);
        public static SelectShopMenuItemDelegate SelectShopMenuItem;

        public delegate void SetShopItemEnabledDelegate(string itemID, bool enabled, string disabledMessage);
        public static SetShopItemEnabledDelegate SetShopItemEnabled;

        public delegate void SetShopItemVisibilityDelegate(string itemID, bool visible);
        public static SetShopItemVisibilityDelegate SetShopItemVisibility;

        public delegate void PurchaseShopItemDelegate(string itemID, ulong amount, Action<bool> callback);
        public static PurchaseShopItemDelegate PurchaseShopItem;

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

        public delegate void SetInputOverridesDelegate(bool movementOverride, bool jumpOverride, bool sprintOverride, bool actionButtonOverride, UnityEngine.Object rootObject = null);
        public static SetInputOverridesDelegate SetInputOverrides;

        public delegate void OnInputGraphRootObjectDestroyedDelegate(UnityEngine.Object rootObject);
        public static OnInputGraphRootObjectDestroyedDelegate OnInputGraphRootObjectDestroyed;

        //Component Initialization
        public delegate void InitializeSpatialInteractableDelegate(SpatialInteractable spatialInteractable);
        public static InitializeSpatialInteractableDelegate InitializeSpatialInteractable;

        public delegate void InitializeSpatialPointOfInterestDelegate(SpatialPointOfInterest spatialPointOfInterest);
        public static InitializeSpatialPointOfInterestDelegate InitializeSpatialPointOfInterest;

        public delegate void PointOfInterestEnabledChangedDelegate(SpatialPointOfInterest spatialPointOfInterest, bool enabled);
        public static PointOfInterestEnabledChangedDelegate PointOfInterestEnabledChanged;

        public delegate bool GetIsSceneInitializedDelegate();
        public static GetIsSceneInitializedDelegate GetIsSceneInitialized;

        public delegate Transform GetCameraTargetOverrideDelegate();
        public static GetCameraTargetOverrideDelegate GetCameraTargetOverride;

        public delegate void SetCameraTargetOverrideDelegate(Transform target, SpatialCameraMode mode);
        public static SetCameraTargetOverrideDelegate SetCameraTargetOverride;

        public delegate void ClearCameraTargetOverrideDelegate();
        public static ClearCameraTargetOverrideDelegate ClearCameraTargetOverride;

        public delegate void SetCameraRotationModeDelegate(SpatialCameraRotationMode mode);
        public static SetCameraRotationModeDelegate SetCameraRotationMode;

        public delegate SpatialCameraRotationMode GetCameraRotationModeDelegate();
        public static GetCameraRotationModeDelegate GetCameraRotationMode;

        public delegate void InitializeSpatialSeatHotspotDelegate(SpatialSeatHotspot spatialHotspot);
        public static InitializeSpatialSeatHotspotDelegate InitializeSpatialSeatHotspot;

        public delegate void InitializeSpatialAvatarTeleporterDelegate(SpatialAvatarTeleporter spatialAvatarTeleporter);
        public static InitializeSpatialAvatarTeleporterDelegate InitializeSpatialAvatarTeleporter;

        public delegate void InitializeSpatialTriggerEventDelegate(SpatialTriggerEvent spatialTriggerEvent);
        public static InitializeSpatialTriggerEventDelegate InitializeSpatialTriggerEvent;

        public delegate void InitializeSpatialClimbableDelegate(SpatialClimbable climbable);
        public static InitializeSpatialClimbableDelegate InitializeSpatialClimbable;

        public delegate void TriggerEventEnabledChangedDelegate(SpatialTriggerEvent spatialTriggerEvent, bool enabled);
        public static TriggerEventEnabledChangedDelegate TriggerEventEnabledChanged;

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

        //Network events (RPC)
        public delegate void SendSDKNetworkEventByteDelegate(bool everyone, byte eventID, object[] args);
        public static SendSDKNetworkEventByteDelegate SendSDKNetworkEventByte;

        public delegate void SendSDKNetworkEventToActorByteDelegate(int targetActor, byte eventID, object[] args);
        public static SendSDKNetworkEventToActorByteDelegate SendSDKNetworkEventToActorByte;
    }
}
