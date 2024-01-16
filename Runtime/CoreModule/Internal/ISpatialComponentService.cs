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
    }
}
