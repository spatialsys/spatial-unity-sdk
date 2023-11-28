using UnityEngine;

namespace SpatialSys.UnitySDK
{
    // We cannot move these into the `.Services` namespace without making breaking changes

    public enum SpatialCameraMode
    {
        Actor = 0,
        Vehicle = 1
    }

    public enum SpatialCameraRotationMode
    {
        /// <summary>Automatic rotation as player moves</summary>
        AutoRotate = 0,
        /// <summary>Locked to the cursor</summary>
        PointerLock_Locked = 1,
        /// <summary>Left mouse button or touch rotation</summary>
        DragToRotate = 2,
        /// <summary>Cursor is unlocked, but next non-ui click returns to PointerLock</summary>
        PointerLock_Unlocked = 3,
    }
}

namespace SpatialSys.UnitySDK.Services
{
    public interface ICameraService
    {
        Vector3 position { get; }
        Quaternion rotation { get; }
        Vector3 forward { get; }

        // Customization
        Vector3 thirdPersonOffset { get; set; }
        float thirdPersonFov { get; set; }
        float firstPersonFov { get; set; }
        bool forceFirstPerson { get; set; }
        bool lockCameraRotation { get; set; }
        float zoomDistance { get; set; }
        float minZoomDistance { get; set; }
        float maxZoomDistance { get; set; }
        SpatialCameraRotationMode rotationMode { get; set; }

        // Camera Shake
        float shakeAmplitude { get; set; }
        float shakeFrequency { get; set; }
        float wobbleAmplitude { get; set; }
        float wobbleFrequency { get; set; }
        float kickDecay { get; set; }
        void Shake(float force);
        void Shake(Vector3 velocity);
        void Wobble(float force);
        void Wobble(Vector3 velocity);
        void Kick(Vector2 degrees);

        // Target Override
        Transform targetOverride { get; }
        void SetTargetOverride(Transform target, SpatialCameraMode cameraMode);
        void ClearTargetOverride();
    }
}
