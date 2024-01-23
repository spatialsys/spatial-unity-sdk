using System;
using System.Collections.Generic;
using SpatialSys.UnitySDK.Internal;

namespace SpatialSys.UnitySDK.EditorSimulation
{
#pragma warning disable 67 // Disable unused event warning
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

        public void InitializeTriggerEvent(SpatialTriggerEvent spatialTriggerEvent) { }

        public void TriggerEventEnabledChanged(SpatialTriggerEvent spatialTriggerEvent, bool enabled) { }

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
