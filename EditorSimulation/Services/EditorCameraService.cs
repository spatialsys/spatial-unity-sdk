using SpatialSys.UnitySDK.Services;
using UnityEngine;

namespace SpatialSys.UnitySDK.EditorSimulation
{
    public class EditorCameraService : ICameraService
    {
        public Vector3 position { get; private set; }
        public Quaternion rotation { get; private set; }
        public Vector3 forward { get; private set; } = Vector3.forward;

        // Customization
        public Vector3 thirdPersonOffset { get; set; }
        public float thirdPersonFov { get; set; } = 70f;
        public float firstPersonFov { get; set; } = 70f;
        public bool forceFirstPerson { get; set; }
        public bool lockCameraRotation { get; set; }
        public float zoomDistance { get; set; } = 5f;
        public float minZoomDistance { get; set; } = 0f;
        public float maxZoomDistance { get; set; } = 10f;
        public SpatialCameraRotationMode rotationMode { get; set; } = SpatialCameraRotationMode.AutoRotate;

        // Camera Shake
        public float shakeAmplitude { get; set; }
        public float shakeFrequency { get; set; }
        public float wobbleAmplitude { get; set; }
        public float wobbleFrequency { get; set; }
        public float kickDecay { get; set; }
        public void Kick(Vector2 degrees) { }
        public void Shake(float force) { }
        public void Shake(Vector3 velocity) { }
        public void Wobble(float force) { }
        public void Wobble(Vector3 velocity) { }

        // Target Override
        public Transform targetOverride { get; private set; }

        public void SetTargetOverride(Transform target, SpatialCameraMode cameraMode)
        {
            targetOverride = target;
        }

        public void ClearTargetOverride() { }
    }
}
