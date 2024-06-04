using System;
using System.Collections.Generic;
using System.Linq;
using SpatialSys.UnitySDK;
using SpatialSys.UnitySDK.Internal;
using Unity.VisualScripting;
using UnityEngine;

namespace SpatialSys.UnitySDK.VisualScripting
{
    /// <summary>
    /// Subscribes to SDK bridge service events and executes Visual Scripting events through the EventBus
    /// </summary>
    [DefaultExecutionOrder(1)] // A bit after everything else, specifically UnitySDKBridgeManager so services are initialized.
    public class UnitySDKVisualScriptingManager : MonoBehaviour
    {
        private static UnitySDKVisualScriptingManager _instance;

        private Dictionary<int, IActor.ActorCustomPropertiesChangedDelegate> _actorCustomPropertyChangedActions = new();
        private Dictionary<string, IInventoryItem.OnItemAmountChangedDelegate> _itemAmountChangedActions = new();
        private Dictionary<string, IInventoryItem.OnItemConsumedDelegate> _itemConsumedActions = new();
        private Dictionary<string, IInventoryItem.OnItemUsedDelegate> _itemUsedActions = new();
        private Dictionary<string, IInventoryItem.OnConsumableItemDurationExpiredDelegate> _itemDurationExpiredActions = new();

        private Dictionary<uint, QuestActions> _questActions = new();

        private void Awake()
        {
            if (_instance != null)
            {
                SpatialBridge.loggingService.LogError($"There should only be one instance of {nameof(UnitySDKVisualScriptingManager)}");
                Destroy(this);
                return;
            }

            _instance = this;

            SpatialBridge.actorService.onActorJoined += HandleOnActorJoined;
            SpatialBridge.actorService.onActorLeft += HandleOnActorLeft;

            SpatialBridge.coreGUIService.onCoreGUIEnabledStateChanged += HandleOnCoreGUIEnabledStateChanged;
            SpatialBridge.coreGUIService.onCoreGUIOpenStateChanged += HandleOnCoreGUIOpenStateChanged;

            SpatialBridge.inputService.onInputCaptureStarted += HandleInputCaptureStarted;
            SpatialBridge.inputService.onInputCaptureStopped += HandleInputCaptureStopped;

            SpatialBridge.inventoryService.onItemOwnedChanged += HandleItemOwnedChanged;
            SpatialBridge.inventoryService.onItemConsumed += OnBackpackAnyItemConsumedNode.TriggerEvent;
            SpatialBridge.inventoryService.onItemUsed += OnBackpackAnyItemUsedNode.TriggerEvent;
            SpatialBridge.inventoryService.onWorldCurrencyBalanceChanged += OnWorldCurrencyBalanceChangedNode.TriggerEvent;

            SpatialBridge.marketplaceService.onItemPurchased += HandleOnItemPurchased;

            SpatialBridge.networkingService.onConnectionStatusChanged += OnConnectedChangedNode.TriggerEvent;
            SpatialBridge.networkingService.onSpaceParticipantCountChanged += OnSpaceParticipantCountChangedNode.TriggerEvent;
            SpatialBridge.networkingService.onServerParticipantCountChanged += OnServerParticipantCountChangedNode.TriggerEvent;
            SpatialBridge.networkingService.remoteEvents.onEvent += IncomingNetworkEventByteNode.TriggerEvent;

            SpatialBridge.questService.onQuestAdded += HandleQuestAdded;
            SpatialBridge.questService.onQuestRemoved += HandleQuestRemoved;

            SpatialBridge.spaceContentService.onSceneInitialized += OnSceneInitializedNode.TriggerEvent;

            SpatialBridge.spaceService.onSpaceLiked += OnSpaceLovedNode.TriggerEvent;

            SpatialBridge.spatialComponentService.onInitializeInteractable += HandleOnInitializeInteractable;
            SpatialBridge.spatialComponentService.onInitializePointOfInterest += HandleOnInitializePointOfInterest;
            SpatialBridge.spatialComponentService.onInitializeTriggerEvent += HandleOnInitializeTriggerEvent;
            SpatialBridge.spatialComponentService.onNetworkObjectOwnerChanged += NetworkObject_OnOwnerChangedNode.TriggerEvent;
            SpatialBridge.spatialComponentService.onNetworkObjectSpawned += NetworkObject_OnSpawnedNode.TriggerEvent;
            SpatialBridge.spatialComponentService.onNetworkObjectDespawned += NetworkObject_OnDespawnedNode.TriggerEvent;
            SpatialBridge.spatialComponentService.onNetworkVariableChanged += SpatialSyncedVariablesOnVariableChanged.TriggerEvent;
            SpatialBridge.spatialComponentService.onSyncedObjectInitialized += SpatialSyncedObjectEventOnObjectInitialized.TriggerEvent;
            SpatialBridge.spatialComponentService.onSyncedObjectOwnerChanged += SpatialSyncedObjectEventOnOwnerChanged.TriggerEvent;
        }

        private void OnDestroy()
        {
            if (_instance == this)
                _instance = null;

            if (SpatialBridge.actorService != null)
            {
                SpatialBridge.actorService.onActorJoined -= HandleOnActorJoined;
                SpatialBridge.actorService.onActorLeft -= HandleOnActorLeft;
            }

            if (SpatialBridge.coreGUIService != null)
            {
                SpatialBridge.coreGUIService.onCoreGUIEnabledStateChanged -= HandleOnCoreGUIEnabledStateChanged;
                SpatialBridge.coreGUIService.onCoreGUIOpenStateChanged -= HandleOnCoreGUIOpenStateChanged;
            }

            if (SpatialBridge.inputService != null)
            {
                SpatialBridge.inputService.onInputCaptureStarted -= HandleInputCaptureStarted;
                SpatialBridge.inputService.onInputCaptureStopped -= HandleInputCaptureStopped;
            }

            if (SpatialBridge.inventoryService != null)
            {
                SpatialBridge.inventoryService.onItemOwnedChanged -= HandleItemOwnedChanged;
                SpatialBridge.inventoryService.onItemConsumed -= OnBackpackAnyItemConsumedNode.TriggerEvent;
                SpatialBridge.inventoryService.onItemUsed -= OnBackpackAnyItemUsedNode.TriggerEvent;
                SpatialBridge.inventoryService.onWorldCurrencyBalanceChanged -= OnWorldCurrencyBalanceChangedNode.TriggerEvent;
            }

            if (SpatialBridge.marketplaceService != null)
            {
                SpatialBridge.marketplaceService.onItemPurchased -= HandleOnItemPurchased;
            }

            if (SpatialBridge.networkingService != null)
            {
                SpatialBridge.networkingService.onConnectionStatusChanged -= OnConnectedChangedNode.TriggerEvent;
                SpatialBridge.networkingService.onSpaceParticipantCountChanged -= OnSpaceParticipantCountChangedNode.TriggerEvent;
                SpatialBridge.networkingService.onServerParticipantCountChanged -= OnServerParticipantCountChangedNode.TriggerEvent;
                SpatialBridge.networkingService.remoteEvents.onEvent -= IncomingNetworkEventByteNode.TriggerEvent;
            }

            if (SpatialBridge.questService != null)
            {
                SpatialBridge.questService.onQuestAdded -= HandleQuestAdded;
                SpatialBridge.questService.onQuestRemoved -= HandleQuestRemoved;
            }

            if (SpatialBridge.spaceContentService != null)
            {
                SpatialBridge.spaceContentService.onSceneInitialized -= OnSceneInitializedNode.TriggerEvent;
            }

            if (SpatialBridge.spaceService != null)
            {
                SpatialBridge.spaceService.onSpaceLiked -= OnSpaceLovedNode.TriggerEvent;
            }

            if (SpatialBridge.spatialComponentService != null)
            {
                SpatialBridge.spatialComponentService.onInitializeInteractable -= HandleOnInitializeInteractable;
                SpatialBridge.spatialComponentService.onInitializePointOfInterest -= HandleOnInitializePointOfInterest;
                SpatialBridge.spatialComponentService.onInitializeTriggerEvent -= HandleOnInitializeTriggerEvent;
                SpatialBridge.spatialComponentService.onNetworkObjectOwnerChanged -= NetworkObject_OnOwnerChangedNode.TriggerEvent;
                SpatialBridge.spatialComponentService.onNetworkObjectSpawned -= NetworkObject_OnSpawnedNode.TriggerEvent;
                SpatialBridge.spatialComponentService.onNetworkObjectDespawned -= NetworkObject_OnDespawnedNode.TriggerEvent;
                SpatialBridge.spatialComponentService.onNetworkVariableChanged -= SpatialSyncedVariablesOnVariableChanged.TriggerEvent;
                SpatialBridge.spatialComponentService.onSyncedObjectInitialized -= SpatialSyncedObjectEventOnObjectInitialized.TriggerEvent;
                SpatialBridge.spatialComponentService.onSyncedObjectOwnerChanged -= SpatialSyncedObjectEventOnOwnerChanged.TriggerEvent;
            }

            foreach (var q in _questActions.Values)
            {
                q.Dispose();
            }
            _questActions.Clear();

            _actorCustomPropertyChangedActions.Clear();
            _itemAmountChangedActions.Clear();
            _itemConsumedActions.Clear();
            _itemUsedActions.Clear();
            _itemDurationExpiredActions.Clear();
        }

        //--------------------------------------------------------------------------------------------------------------
        // ACTOR SERVICE
        //--------------------------------------------------------------------------------------------------------------

        private void HandleOnActorJoined(ActorJoinedEventArgs args)
        {
            int actorNumber = args.actorNumber;

            OnActorJoinedNode.TriggerEvent(actorNumber);

            if (SpatialBridge.actorService.actors.TryGetValue(actorNumber, out IActor actor))
            {
                void HandleActorPropertiesChanged(ActorCustomPropertiesChangedEventArgs a) => HandleOnActorCustomPropertiesChanged(actorNumber, a);
                _actorCustomPropertyChangedActions.Add(actorNumber, HandleActorPropertiesChanged);
                actor.onCustomPropertiesChanged += HandleActorPropertiesChanged;

                // For visual scripting backwards compatibility to hydrate the initial state
                HandleActorPropertiesChanged(new ActorCustomPropertiesChangedEventArgs {
                    changedProperties = actor.customProperties,
                    removedProperties = new List<string>()
                });
            }
        }

        private void HandleOnActorLeft(ActorLeftEventArgs args)
        {
            int actorNumber = args.actorNumber;

            OnActorLeftNode.TriggerEvent(actorNumber);

            if (SpatialBridge.actorService.actors.TryGetValue(actorNumber, out IActor actor)
                && _actorCustomPropertyChangedActions.TryGetValue(actorNumber, out IActor.ActorCustomPropertiesChangedDelegate action))
            {
                actor.onCustomPropertiesChanged -= action;
            }
            _actorCustomPropertyChangedActions.Remove(actorNumber);
        }

        private void HandleOnActorCustomPropertiesChanged(int actorNumber, ActorCustomPropertiesChangedEventArgs args)
        {
            foreach (KeyValuePair<string, object> prop in args.changedProperties)
                OnActorCustomVariableChanged.TriggerEvent(actorNumber, prop.Key, prop.Value);
        }

        //--------------------------------------------------------------------------------------------------------------
        // CORE GUI SERVICE
        //--------------------------------------------------------------------------------------------------------------

        private void HandleOnCoreGUIEnabledStateChanged(SpatialCoreGUIType guiType, bool enabled)
        {
            // OnCoreGUIEnabledStateChangedNode.TriggerEvent(guiType, enabled) // TODO: There's no scripting node for this yet.
        }

        private void HandleOnCoreGUIOpenStateChanged(SpatialCoreGUIType guiType, bool open)
        {
            switch (guiType)
            {
                case SpatialCoreGUIType.Backpack:
                    OnBackpackMenuOpenChangedNode.TriggerEvent(open);
                    break;
                case SpatialCoreGUIType.Shop:
                    OnShopMenuOpenChangedNode.TriggerEvent(open);
                    break;
            }
        }

        private void HandleInputCaptureStarted(IInputActionsListener listener, InputCaptureType type)
        {
            if (listener is SpatialInputActionsListenerComponent component)
            {
                component.onInputCaptureStoppedEvent += SpatialOnInputCaptureStopped.TriggerEvent;

                // Avatar events
                component.onAvatarMoveInputEvent += SpatialOnOverriddenMoveInput.TriggerEvent;
                component.onAvatarJumpInputEvent += SpatialOnOverriddenJumpInput.TriggerEvent;
                component.onAvatarSprintInputEvent += SpatialOnOverriddenSprintInput.TriggerEvent;
                component.onAvatarActionInputEvent += SpatialOnOverriddenActionButtonInput.TriggerEvent;
                component.onAvatarAutoSprintToggledEvent += SpatialOnAutoSprintToggledOn.TriggerEvent;

                // Vehicle events
                void HandleVehicleSteerEvent(InputPhase phase, Vector2 steer) => SpatialOnVehicleSteerInput.TriggerEvent(component.gameObject, phase, steer);
                component.onVehicleSteerInputEvent += HandleVehicleSteerEvent;

                void HandleVehicleThrottleEvent(InputPhase phase, float throttle) => SpatialOnVehicleThrottleInput.TriggerEvent(component.gameObject, phase, throttle);
                component.onVehicleThrottleInputEvent += HandleVehicleThrottleEvent;

                void HandleVehicleReverseEvent(InputPhase phase, float reverse) => SpatialOnVehicleReverseInput.TriggerEvent(component.gameObject, phase, reverse);
                component.onVehicleReverseInputEvent += HandleVehicleReverseEvent;

                void HandleVehiclePrimaryActionEvent(InputPhase phase) => SpatialOnVehiclePrimaryActionInput.TriggerEvent(component.gameObject, phase);
                component.onVehiclePrimaryActionInputEvent += HandleVehiclePrimaryActionEvent;

                void HandleVehicleSecondaryActionEvent(InputPhase phase) => SpatialOnVehicleSecondaryActionInput.TriggerEvent(component.gameObject, phase);
                component.onVehicleSecondaryActionInputEvent += HandleVehicleSecondaryActionEvent;

                void HandleVehicleExitEvent() => SpatialOnExitVehicleInput.TriggerEvent(component.gameObject);
                component.onVehicleExitInputEvent += HandleVehicleExitEvent;
            }
        }


        private void HandleInputCaptureStopped(IInputActionsListener listener, InputCaptureType type)
        {
            if (listener is SpatialInputActionsListenerComponent component)
            {
                component.ClearListeners();
            }
        }

        //--------------------------------------------------------------------------------------------------------------
        // INVENTORY SERVICE
        //--------------------------------------------------------------------------------------------------------------

        private void HandleItemOwnedChanged(string itemID, bool owned)
        {
            OnBackpackItemOwnedChangedNode.TriggerEvent(itemID, owned);
            OnBackpackAnyItemOwnedChangedNode.TriggerEvent(itemID, owned);

            if (owned)
            {
                if (SpatialBridge.inventoryService.items.TryGetValue(itemID, out IInventoryItem item))
                {
                    // Subscribe to item events
                    void HandleItemAmountChanged(ulong amount) => OnBackpackItemAmountChangedNode.TriggerEvent(itemID, amount);
                    _itemAmountChangedActions.Add(itemID, HandleItemAmountChanged);
                    item.onItemAmountChanged += HandleItemAmountChanged;

                    void HandleItemConsumed() => OnBackpackItemConsumedNode.TriggerEvent(itemID);
                    _itemConsumedActions.Add(itemID, HandleItemConsumed);
                    item.onItemConsumed += HandleItemConsumed;

                    void HandleItemUsed() => OnBackpackItemUsedNode.TriggerEvent(itemID);
                    _itemUsedActions.Add(itemID, HandleItemUsed);
                    item.onItemUsed += HandleItemUsed;

                    void HandleItemDurationExpired() => OnConsumableItemDurationExpiredNode.TriggerEvent(itemID);
                    _itemDurationExpiredActions.Add(itemID, HandleItemDurationExpired);
                    item.onConsumableItemDurationExpired += HandleItemDurationExpired;
                }
            }
            else
            {
                if (SpatialBridge.inventoryService.items.TryGetValue(itemID, out IInventoryItem item))
                {
                    // Unscubscribe from item events
                    if (_itemAmountChangedActions.TryGetValue(itemID, out IInventoryItem.OnItemAmountChangedDelegate action1))
                    {
                        item.onItemAmountChanged -= action1;
                    }
                    if (_itemConsumedActions.TryGetValue(itemID, out IInventoryItem.OnItemConsumedDelegate action2))
                    {
                        item.onItemConsumed -= action2;
                    }
                    if (_itemUsedActions.TryGetValue(itemID, out IInventoryItem.OnItemUsedDelegate action3))
                    {
                        item.onItemUsed -= action3;
                    }
                    if (_itemDurationExpiredActions.TryGetValue(itemID, out IInventoryItem.OnConsumableItemDurationExpiredDelegate action4))
                    {
                        item.onConsumableItemDurationExpired -= action4;
                    }
                }
                _itemAmountChangedActions.Remove(itemID);
                _itemConsumedActions.Remove(itemID);
                _itemUsedActions.Remove(itemID);
                _itemDurationExpiredActions.Remove(itemID);
            }
        }

        // private void RemoveFromActionCache<K, T>(K key, Dictionary<K, T> actionCache, Action<T> removeCallback) where T : Delegate
        // {
        //     if (actionCache.TryGetValue(key, out T action))
        //     {
        //         removeCallback.Invoke(action);
        //     }
        //     actionCache.Remove(key);
        // }

        //--------------------------------------------------------------------------------------------------------------
        // MARKETPLACE SERVICE
        //--------------------------------------------------------------------------------------------------------------

        private void HandleOnItemPurchased(ItemPurchasedEventArgs args)
        {
            OnShopItemPurchasedNode.TriggerEvent(args);
            OnShopSpecificItemPurchasedNode.TriggerEvent(args);
        }

        //--------------------------------------------------------------------------------------------------------------
        // QUEST SERVICE
        //--------------------------------------------------------------------------------------------------------------

        private void HandleQuestAdded(IQuest quest)
        {
            SpatialQuest spatialQuest = FindObjectsOfType<SpatialQuest>().FirstOrDefault(spatialQuest => spatialQuest.id == quest.id);

            if (spatialQuest != null)
            {
                _questActions.Add(quest.id, new QuestActions(spatialQuest, quest));
            }
        }

        private void HandleQuestRemoved(IQuest quest)
        {
            if (_questActions.TryGetValue(quest.id, out QuestActions actions))
            {
                actions.Dispose();
                _questActions.Remove(quest.id);
            }
        }

        //--------------------------------------------------------------------------------------------------------------
        // SPATIAL COMPONENT SERVICE
        //--------------------------------------------------------------------------------------------------------------

        private void HandleOnInitializeInteractable(SpatialInteractable interactable)
        {
            interactable.onEnterEvent += () => SpatialInteractableOnEnter.TriggerEvent(interactable);
            interactable.onExitEvent += () => SpatialInteractableOnExit.TriggerEvent(interactable);
            interactable.onInteractEvent += () => SpatialInteractableOnInteract.TriggerEvent(interactable);
        }

        private void HandleOnInitializePointOfInterest(SpatialPointOfInterest poi)
        {
            poi.onTextDisplayedEvent += () => SpatialPointOfInterestOnEnter.TriggerEvent(poi);
        }

        private void HandleOnInitializeTriggerEvent(SpatialTriggerEvent triggerEvent)
        {
            triggerEvent.onEnterEvent += () => SpatialTriggerEventOnEnter.TriggerEvent(triggerEvent);
            triggerEvent.onExitEvent += () => SpatialTriggerEventOnExit.TriggerEvent(triggerEvent);
        }
    }

    public class QuestActions : IDisposable
    {
        private IQuest _quest;
        private SpatialQuest _spatialQuest;
        private List<QuestTaskActions> _taskActions = new();

        private void HandleCompleted()
        {
            SpatialQuestEventOnCompleted.TriggerEvent(_spatialQuest);
        }
        private void HandleReset()
        {
            SpatialQuestEventOnReset.TriggerEvent(_spatialQuest);
        }
        private void HandlePreviouslyCompleted()
        {
            SpatialQuestEventOnPreviouslyCompleted.TriggerEvent(_spatialQuest);
        }
        private void HandleStarted()
        {
            SpatialQuestEventOnStarted.TriggerEvent(_spatialQuest);
        }
        private void HandleQuestTaskAdded(IQuestTask task)
        {
            _taskActions.Add(new QuestTaskActions(_spatialQuest, task));
        }

        public QuestActions(SpatialQuest spatialQuest, IQuest quest)
        {
            _spatialQuest = spatialQuest;
            _quest = quest;
            _quest.onCompleted += HandleCompleted;
            _quest.onReset += HandleReset;
            _quest.onPreviouslyCompleted += HandlePreviouslyCompleted;
            _quest.onStarted += HandleStarted;

            quest.onTaskAdded += HandleQuestTaskAdded;

            foreach (IQuestTask task in quest.tasks)
            {
                HandleQuestTaskAdded(task);
            }

            switch (_quest.status)
            {
                case QuestStatus.Completed:
                    HandlePreviouslyCompleted();
                    break;
                case QuestStatus.InProgress:
                    HandleStarted();
                    break;
            }
        }

        public void Dispose()
        {
            foreach (var t in _taskActions)
            {
                t.Dispose();
            }
            _taskActions.Clear();
            _quest.onCompleted -= HandleCompleted;
            _quest.onReset -= HandleReset;
            _quest.onPreviouslyCompleted -= HandlePreviouslyCompleted;
            _quest.onStarted -= HandleStarted;
            _quest = null;
        }
    }

    public class QuestTaskActions : IDisposable
    {
        private IQuestTask _task;
        private SpatialQuest _spatialQuest;

        private void HandleCompleted()
        {
            SpatialQuestEventOnTaskCompleted.TriggerEvent(_spatialQuest, _task.id);
            SpatialQuestEventOnAnyTaskCompleted.TriggerEvent(_spatialQuest);
        }
        private void HandlePreviouslyCompleted()
        {
            SpatialQuestEventOnTaskPreviouslyCompleted.TriggerEvent(_spatialQuest, _task.id);
        }
        private void HandleStarted()
        {
            SpatialQuestEventOnTaskStarted.TriggerEvent(_spatialQuest, _task.id);
            SpatialQuestEventOnAnyTaskStarted.TriggerEvent(_spatialQuest);
        }

        public QuestTaskActions(SpatialQuest spatialQuest, IQuestTask task)
        {
            _spatialQuest = spatialQuest;
            _task = task;
            _task.onCompleted += HandleCompleted;
            _task.onPreviouslyCompleted += HandlePreviouslyCompleted;
            _task.onStarted += HandleStarted;

            switch (_task.status)
            {
                case QuestStatus.Completed:
                    HandlePreviouslyCompleted();
                    break;
                case QuestStatus.InProgress:
                    HandleStarted();
                    break;
            }
        }

        public void Dispose()
        {
            _task.onCompleted -= HandleCompleted;
            _task.onPreviouslyCompleted -= HandlePreviouslyCompleted;
            _task.onStarted -= HandleStarted;
            _task = null;
        }
    }
}
