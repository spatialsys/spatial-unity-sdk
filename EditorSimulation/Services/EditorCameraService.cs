using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace SpatialSys.UnitySDK.EditorSimulation
{
    public class EditorCameraService : ICameraService
    {
        private CameraFollow _cameraFollow;

        public EditorCameraService()
        {
            // Delete all cameras that are not render textures
            ProcessCameras(GameObject.FindObjectsOfType<Camera>(true));

            // Create a new main camera
            GameObject g = new GameObject();
            g.name = "[Spatial SDK] Main Camera";
            g.tag = "MainCamera";
            var camera = g.AddComponent<Camera>();

            _cameraFollow = g.AddComponent<CameraFollow>();
            _cameraFollow.camera = camera;
            _cameraFollow.target = (SpatialBridge.actorService.localActor.avatar as EditorLocalAvatar).transform;
            _cameraFollow.offset = new Vector3(0, 1.6f, -4.5f);
            _cameraFollow.lookAtOffset = new Vector3(0, 1.5f, 0);
        }


        private static void ProcessCameras(Camera[] cameras)
        {
            // Delete non render texture cameras
            foreach (Camera camera in cameras)
            {
                if (camera.targetTexture == null)
                {
                    if (camera.TryGetComponent(out UniversalAdditionalCameraData extraData))
                    {
                        GameObject.DestroyImmediate(extraData);
                    }
                    GameObject.DestroyImmediate(camera);
                }
            }
        }
        //----------------------------------------------------------------------------------------------------
        // ICameraService implementation
        //----------------------------------------------------------------------------------------------------

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

        public XRCameraMode xrCameraMode { get; set; } = XRCameraMode.FirstPerson;
        public bool allowPlayerToSwitchXRCameraMode { get; set; } = true;

        // Camera Shake
        public float shakeAmplitude { get; set; }
        public float shakeFrequency { get; set; }
        public float wobbleAmplitude { get; set; }
        public float wobbleFrequency { get; set; }
        public float kickDecay { get; set; }
        public float virtualCameraBlendTime { get; set; }
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
            _cameraFollow.target = targetOverride;
        }

        public void ClearTargetOverride()
        {
            targetOverride = null;
            _cameraFollow.target = (SpatialBridge.actorService.localActor.avatar as EditorLocalAvatar).transform;
        }

        // Additional properties and methods using UnityEngine.Camera.main
        public void CopyFromMainCamera(Camera camera)
        {
            if (Camera.main != null)
            {
                camera.CopyFrom(Camera.main);
            }
            else if (UnityEditor.SceneView.currentDrawingSceneView != null)
            {
                camera.CopyFrom(UnityEditor.SceneView.currentDrawingSceneView.camera);
            }
        }
        public Matrix4x4 cameraToWorldMatrix => Camera.main.cameraToWorldMatrix;
        public int pixelHeight => Camera.main.pixelHeight;
        public int pixelWidth => Camera.main.pixelWidth;
        public int scaledPixelHeight => Camera.main.scaledPixelHeight;
        public int scaledPixelWidth => Camera.main.scaledPixelWidth;
        public Rect rect => Camera.main.rect;
        public Vector3 velocity => Camera.main.velocity;
        public Matrix4x4 worldToCameraMatrix => Camera.main.worldToCameraMatrix;
        public Matrix4x4 projectionMatrix => Camera.main.projectionMatrix;
        public Matrix4x4 GetStereoViewMatrix(Camera.StereoscopicEye eye) => Camera.main.GetStereoViewMatrix(eye);
        public Matrix4x4 GetStereoProjectionMatrix(Camera.StereoscopicEye eye) => Camera.main.GetStereoProjectionMatrix(eye);

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
        public void SetXRCameraMode(XRCameraMode cameraMode) { }
    }
}
