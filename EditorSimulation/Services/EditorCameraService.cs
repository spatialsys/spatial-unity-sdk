using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace SpatialSys.UnitySDK.EditorSimulation
{
    // Helper class used to run coroutines in the editor. By placing this class on a shared file, Unity
    // doesn't detect this file as an 'Editor' file and lets us attach it to a GameObject.
    public class EditorCameraServiceHelper : MonoBehaviour
    {
        public EditorCameraService service;

        private void Update()
        {
            service.Update();
        }
    }

    public class EditorCameraService : ICameraService
    {
        private CameraFollow _cameraFollow;
        private Camera _activeCamera;

        public Camera activeCamera => _activeCamera ?? Camera.main;

        public EditorCameraService()
        {
            // Delete all cameras that are not render textures
            ProcessCameras(GameObject.FindObjectsOfType<Camera>(true));

            // Create a new main camera
            GameObject g = new GameObject();
            g.name = "[Spatial SDK] Main Camera";
            g.tag = "MainCamera";
            var camera = g.AddComponent<Camera>();
            g.AddComponent<EditorCameraServiceHelper>().service = this;

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

        // This update loop keeps track of the camera that's currently rendering. Other alternatives do not work:
        // - Camera.main just returns the first enabled game object that has the MainCamera tag
        // - Camera.current does not give us the proper camera and is sometimes null
        // Ideally, the EditorCameraService uses the cinemachine library like Spatial does, so there's really only one camera
        // and can be safely accessed via Camera.main
        public void Update()
        {
            float priority = -1;
            if (_activeCamera != null && _activeCamera.gameObject.activeInHierarchy && _activeCamera.enabled)
            {
                priority = _activeCamera.depth;
            }
            else
            {
                _activeCamera = null;
            }

            foreach (Camera c in Camera.allCameras)
            {
                if (c.gameObject.activeInHierarchy && c.enabled && c.depth > priority)
                {
                    _activeCamera = c;
                    priority = c.depth;
                }
            }
        }
        //----------------------------------------------------------------------------------------------------
        // ICameraService implementation
        //----------------------------------------------------------------------------------------------------

        public Vector3 position => activeCamera.transform.position;
        public Quaternion rotation => activeCamera.transform.rotation;
        public Vector3 forward => activeCamera.transform.forward;

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
        public Matrix4x4 cameraToWorldMatrix => activeCamera.cameraToWorldMatrix;
        public int pixelHeight => activeCamera.pixelHeight;
        public int pixelWidth => activeCamera.pixelWidth;
        public int scaledPixelHeight => activeCamera.scaledPixelHeight;
        public int scaledPixelWidth => activeCamera.scaledPixelWidth;
        public Rect rect => activeCamera.rect;
        public Vector3 velocity => activeCamera.velocity;
        public Matrix4x4 worldToCameraMatrix => activeCamera.worldToCameraMatrix;
        public Matrix4x4 projectionMatrix => activeCamera.projectionMatrix;
        public Matrix4x4 GetStereoViewMatrix(Camera.StereoscopicEye eye) => activeCamera.GetStereoViewMatrix(eye);
        public Matrix4x4 GetStereoProjectionMatrix(Camera.StereoscopicEye eye) => activeCamera.GetStereoProjectionMatrix(eye);

        public void CalculateFrustumCorners(Rect viewport, float z, Camera.MonoOrStereoscopicEye eye, Vector3[] outCorners)
        {
            activeCamera.CalculateFrustumCorners(viewport, z, eye, outCorners);
        }
        public Matrix4x4 CalculateObliqueMatrix(Vector4 clipPlane) => activeCamera.CalculateObliqueMatrix(clipPlane);
        public Ray ScreenPointToRay(Vector3 pos) => activeCamera.ScreenPointToRay(pos);
        public Ray ScreenPointToRay(Vector3 pos, Camera.MonoOrStereoscopicEye eye) => activeCamera.ScreenPointToRay(pos, eye);
        public Vector3 ScreenToViewportPoint(Vector3 pos) => activeCamera.ScreenToViewportPoint(pos);
        public Vector3 ScreenToWorldPoint(Vector3 pos) => activeCamera.ScreenToWorldPoint(pos);
        public Vector3 ScreenToWorldPoint(Vector3 pos, Camera.MonoOrStereoscopicEye eye) => activeCamera.ScreenToWorldPoint(pos, eye);
        public Ray ViewportPointToRay(Vector3 pos) => activeCamera.ViewportPointToRay(pos);
        public Ray ViewportPointToRay(Vector3 pos, Camera.MonoOrStereoscopicEye eye) => activeCamera.ViewportPointToRay(pos, eye);
        public Vector3 ViewportToScreenPoint(Vector3 pos) => activeCamera.ViewportToScreenPoint(pos);
        public Vector3 ViewportToWorldPoint(Vector3 pos) => activeCamera.ViewportToWorldPoint(pos);
        public Vector3 WorldToScreenPoint(Vector3 pos) => activeCamera.WorldToScreenPoint(pos);
        public Vector3 WorldToScreenPoint(Vector3 pos, Camera.MonoOrStereoscopicEye eye) => activeCamera.WorldToScreenPoint(pos, eye);
        public Vector3 WorldToViewportPoint(Vector3 pos) => activeCamera.WorldToViewportPoint(pos);
        public Vector3 WorldToViewportPoint(Vector3 pos, Camera.MonoOrStereoscopicEye eye) => activeCamera.WorldToViewportPoint(pos, eye);
        public Plane[] CalculateFrustumPlanes() => GeometryUtility.CalculateFrustumPlanes(activeCamera);
        public void SetXRCameraMode(XRCameraMode cameraMode) { }
    }
}
