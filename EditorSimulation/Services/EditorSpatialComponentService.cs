using System;
using System.Collections.Generic;
using System.Linq;
using SpatialSys.UnitySDK.Internal;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace SpatialSys.UnitySDK.EditorSimulation
{
#pragma warning disable 0067 // Disable unused event warning
    public class EditorSpatialComponentService : ISpatialComponentService
    {
        public event Action<SpatialInteractable> onInitializeInteractable;
        public event Action<SpatialPointOfInterest> onInitializePointOfInterest;
        public event Action<SpatialTriggerEvent> onInitializeTriggerEvent;

        public void InitializeInteractable(SpatialInteractable spatialInteractable) { }

        public void InitializePointOfInterest(SpatialPointOfInterest spatialPointOfInterest) { }

        public void PointOfInterestEnabledChanged(SpatialPointOfInterest spatialPointOfInterest, bool enabled) { }

        public void InitializeSeatHotspot(SpatialSeatHotspot spatialHotspot) { }

        public void InitializeAvatarTeleporter(SpatialAvatarTeleporter spatialAvatarTeleporter) { }

        public void InitializeTriggerEvent(SpatialTriggerEvent spatialTriggerEvent)
        {
            var triggerEventComponent = spatialTriggerEvent.gameObject.AddComponent<TriggerEventComponent>();
            triggerEventComponent.triggerEvent = spatialTriggerEvent;
            triggerEventComponent.enabled = spatialTriggerEvent.enabled;

            onInitializeTriggerEvent?.Invoke(spatialTriggerEvent);
        }

        public void TriggerEventEnabledChanged(SpatialTriggerEvent spatialTriggerEvent, bool enabled)
        {
            var triggerEventComponent = spatialTriggerEvent.gameObject.GetComponent<TriggerEventComponent>();
            if (triggerEventComponent)
                triggerEventComponent.enabled = enabled;
        }

        public void InitializeClimbable(SpatialClimbable climbable) { }

        public void InitializeCameraPassthrough(SpatialCameraPassthrough spatialCameraPassthrough) { }

        public Action InitializeVirtualCamera(SpatialVirtualCamera virtualCamera)
        {
            var camera = virtualCamera.gameObject.AddComponent<Camera>();

            camera.depth = virtualCamera.priority;
            camera.fieldOfView = virtualCamera.fieldOfView;
            camera.nearClipPlane = virtualCamera.nearClipPlane;
            camera.farClipPlane = virtualCamera.farClipPlane;

            void UpdateProperties()
            {
                camera.depth = virtualCamera.priority;
                camera.fieldOfView = virtualCamera.fieldOfView;
                camera.nearClipPlane = virtualCamera.nearClipPlane;
                camera.farClipPlane = virtualCamera.farClipPlane;
            }
            return UpdateProperties;
        }

        public event ISpatialComponentService.OnNetworkObjectOwnerChangedDelegate onNetworkObjectOwnerChanged;
        public event ISpatialComponentService.OnNetworkObjectSpawnedDelegate onNetworkObjectSpawned;
        public event ISpatialComponentService.OnNetworkObjectDestroyedDelegate onNetworkObjectDespawned;
        public event ISpatialComponentService.OnNetworkVariableChangedDelegate onNetworkVariableChanged;

        public event ISpatialComponentService.OnSyncedObjectInitializedDelegate onSyncedObjectInitialized;
        public event ISpatialComponentService.OnSyncedObjectOwnerChangedDelegate onSyncedObjectOwnerChanged;

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

        public void SetSyncedAnimatorParameter(SpatialSyncedAnimator syncedAnimator, string parameterName, object value)
        {
            if (syncedAnimator == null)
                return;

            if (value is bool boolValue)
            {
                syncedAnimator.animator.SetBool(parameterName, boolValue);
            }
            else if (value is int intValue)
            {
                syncedAnimator.animator.SetInteger(parameterName, intValue);
            }
            else if (value is float floatValue)
            {
                syncedAnimator.animator.SetFloat(parameterName, floatValue);
            }
            else
            {
                SpatialBridge.loggingService.LogError($"SetSyncedAnimatorParameter: Unsupported parameter type {value.GetType()}");
            }
        }

        public void SetSyncedAnimatorTrigger(SpatialSyncedAnimator syncedAnimator, string triggerName)
        {
            if (syncedAnimator == null)
                return;

            syncedAnimator.animator.SetTrigger(triggerName);
        }
    }
#pragma warning restore 0067
}
