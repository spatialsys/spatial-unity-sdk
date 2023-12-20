using UnityEngine;

namespace SpatialSys.UnitySDK.EditorSimulation
{
    public class EditorCameraService : ICameraService
    {
        public Vector3 position => Camera.main.transform.position;
        public Quaternion rotation => Camera.main.transform.rotation;
        public Vector3 forward => Camera.main.transform.forward;

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

        public void ClearTargetOverride()
        {
            targetOverride = null;
        }

        // Additional properties and methods using UnityEngine.Camera.main
        public Matrix4x4 cameraToWorldMatrix => Camera.main.cameraToWorldMatrix;
        public int pixelHeight => Camera.main.pixelHeight;
        public int pixelWidth => Camera.main.pixelWidth;
        public int scaledPixelHeight => Camera.main.scaledPixelHeight;
        public int scaledPixelWidth => Camera.main.scaledPixelWidth;
        public Vector3 velocity => Camera.main.velocity;
        public Matrix4x4 worldToCameraMatrix => Camera.main.worldToCameraMatrix;

        public void CalculateFrustumCorners(Rect viewport, float z, Camera.MonoOrStereoscopicEye eye, Vector3[] outCorners)
        {
            Camera.main.CalculateFrustumCorners(viewport, z, eye, outCorners);
        }
        public Matrix4x4 CalculateObliqueMatrix(Vector4 clipPlane) => Camera.main.CalculateObliqueMatrix(clipPlane);
        public Ray ScreenPointToRay(Vector3 pos) => Camera.main.ScreenPointToRay(pos);
        public Ray ScreenPointToRay(Vector3 pos, Camera.MonoOrStereoscopicEye eye) => Camera.main.ScreenPointToRay(pos, eye);
        public Vector3 ScreenToViewportPoint(Vector3 pos) => Camera.main.ScreenToViewportPoint(pos);
        public Vector3 ScreenToWorldPoint(Vector3 pos) => Camera.main.ScreenToWorldPoint(pos);
        public Vector3 ScreenToWorldPoint(Vector3 pos, Camera.MonoOrStereoscopicEye eye) => Camera.main.ScreenToWorldPoint(pos, eye);
        public Ray ViewportPointToRay(Vector3 pos) => Camera.main.ViewportPointToRay(pos);
        public Ray ViewportPointToRay(Vector3 pos, Camera.MonoOrStereoscopicEye eye) => Camera.main.ViewportPointToRay(pos, eye);
        public Vector3 ViewportToScreenPoint(Vector3 pos) => Camera.main.ViewportToScreenPoint(pos);
        public Vector3 ViewportToWorldPoint(Vector3 pos) => Camera.main.ViewportToWorldPoint(pos);
        public Vector3 WorldToScreenPoint(Vector3 pos) => Camera.main.WorldToScreenPoint(pos);
        public Vector3 WorldToScreenPoint(Vector3 pos, Camera.MonoOrStereoscopicEye eye) => Camera.main.WorldToScreenPoint(pos, eye);
        public Vector3 WorldToViewportPoint(Vector3 pos) => Camera.main.WorldToViewportPoint(pos);
        public Vector3 WorldToViewportPoint(Vector3 pos, Camera.MonoOrStereoscopicEye eye) => Camera.main.WorldToViewportPoint(pos, eye);
        public Plane[] CalculateFrustumPlanes() => GeometryUtility.CalculateFrustumPlanes(Camera.main);
    }
}
