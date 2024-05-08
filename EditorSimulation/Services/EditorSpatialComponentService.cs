using System;
using System.Collections.Generic;
using SpatialSys.UnitySDK.Internal;
using UnityEngine;

namespace SpatialSys.UnitySDK.EditorSimulation
{
    public class EditorSpatialComponentService : ISpatialComponentService
    {
#pragma warning disable 0067 // Disable unused event warning
        public event Action<SpatialInteractable> onInitializeInteractable;
        public event Action<SpatialPointOfInterest> onInitializePointOfInterest;
#pragma warning restore 0067
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
            void UpdateProperties()
            {
            }
            return UpdateProperties;
        }
    }
}
