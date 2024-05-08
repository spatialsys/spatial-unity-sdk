using UnityEngine;

namespace SpatialSys.UnitySDK
{
    /// <summary>
    /// This service provides access to all camera related functionality: Main camera state, player camera settings,
    /// camera shake, and target overrides.
    /// </summary>
    /// <remarks>
    /// <para>
    /// In spatial there is a single internally managed <c>Main Camera</c>, which is driven by various
    /// <c>VirtualCameras</c>. The default <c>Actor Camera</c> is a virtual camera that follows the player actor.
    /// This can be overridden using the <see cref="SpatialVirtualCamera"/> component.
    /// </para>
    /// <para>
    /// Spatial limits access to the internal <c>Main Camera</c> component. This service provides a limited
    /// subset of useful camera properties and functions.
    /// Most properties and methods implement the corresponding member from Unity's <c>Camera</c> class.
    /// </para>
    /// </remarks>
    [DocumentationCategory("Services/Camera Service")]
    public interface ICameraService
    {
        /// <summary>
        /// The world position of the main camera
        /// </summary>
        Vector3 position { get; }

        /// <summary>
        /// The world rotation of the main camera
        /// </summary>
        Quaternion rotation { get; }

        /// <summary>
        /// The world space forward vector of the main camera
        /// </summary>
        Vector3 forward { get; }

        /// <summary>
        /// A translation offset applied to the actor camera in relation to the player actor. This value is rotated with
        /// the look direction, meaning a value of (1,0,0) will always keep the player on the left side of the screen.
        /// </summary>
        Vector3 thirdPersonOffset { get; set; }

        /// <summary>
        /// The field of view while the actor camera is in third person mode.
        /// </summary>
        float thirdPersonFov { get; set; }

        /// <summary>
        /// The field of view while the actor camera is in first person mode.
        /// </summary>
        float firstPersonFov { get; set; }

        /// <summary>
        /// When true the actor camera will be forced into first person mode.
        /// </summary>
        bool forceFirstPerson { get; set; }

        /// <summary>
        /// When true the user will be unable to rotate the actor camera.
        /// </summary>
        bool lockCameraRotation { get; set; }

        /// <summary>
        /// The current 3rd person camera distance from the player actor.
        /// </summary>
        float zoomDistance { get; set; }

        /// <summary>
        /// The minimum 3rd person camera zoom distance for the actor camera.
        /// </summary>
        float minZoomDistance { get; set; }

        /// <summary>
        /// The maximum 3rd person camera zoom distance for the actor camera.
        /// </summary>
        float maxZoomDistance { get; set; }

        /// <summary>
        /// The method of rotation for the actor camera with respect to player inputs.
        /// How should the camera be controlled?
        /// </summary>
        SpatialCameraRotationMode rotationMode { get; set; }

        /// <summary>
        /// The camera mode for players using an XR headset
        /// Note that this changing this will not have effect while a virtual camera is currently rendering
        /// </summary>
        XRCameraMode xrCameraMode { get; set; }

        /// <summary>
        /// Allows a player in XR to switch between first person and third person modes
        /// </summary>
        bool allowPlayerToSwitchXRCameraMode { get; set; }

        /// <summary>
        /// The intensity of camera shakes.
        /// </summary>
        float shakeAmplitude { get; set; }

        /// <summary>
        /// The speed at which camera shakes vibrate
        /// </summary>
        float shakeFrequency { get; set; }

        /// <summary>
        /// The intensity of camera wobbles. Wobbles are low frequency shakes.
        /// </summary>
        float wobbleAmplitude { get; set; }

        /// <summary>
        /// The speed at which camera wobbles vibrate. Wobbles are low frequency shakes.
        /// </summary>
        float wobbleFrequency { get; set; }

        /// <summary>
        /// The speed at which camera kicks return to zero.
        /// </summary>
        float kickDecay { get; set; }

        /// <summary>
        /// The blend time in seconds for virtual camera transitions. Default is 0.6 seconds.
        /// </summary>
        float virtualCameraBlendTime { get; set; }

        /// <summary>
        /// Apply a high frequency shake to the player camera and all virtual cameras. Good for short, low to high intensity impacts.
        /// </summary>
        /// <remarks>
        /// Equivalent to <c>Shake(Vector3.down * force)</c>
        /// </remarks>
        void Shake(float force);

        /// <summary>
        /// Apply a high frequency shake to the player camera and all virtual cameras. Good for short, low to high intensity impacts.
        /// </summary>
        void Shake(Vector3 velocity);

        /// <summary>
        /// Apply a low frequency shake to the player camera and all virtual cameras. Good for longer high intensity impacts.
        /// Makes the camera feel <c>dazed</c> or off balance.
        /// </summary>
        /// <remarks>
        /// Equivalent to <c>Wobble(Vector3.down * force)</c>
        /// </remarks>
        void Wobble(float force);

        /// <summary>
        /// Apply a low frequency shake to the player camera and all virtual cameras. Good for longer high intensity impacts.
        /// Makes the camera feel <c>dazed</c> or off balance.
        /// </summary>
        void Wobble(Vector3 velocity);

        /// <summary>
        /// Apply a temporary angular offset to the player camera, think <c>recoil</c> for a first person shooter.
        /// 
        /// <para>Given a zero-ed out initial rotation, the camera will rotate to the given degrees offset, then return
        /// to zero over time. The rotation is applied with a parabolic function over time using <c>kickDecay</c> to
        /// drive the slope. A larger <c>kickDecay</c> creates a shorter/faster kick.</para>
        /// 
        /// <para>Kicks are additive, but there can only be one active kick. This means two kicks in rapid succession
        /// can combine to create a larger kick, but two kicks in instant succession (during the same frame) will override
        /// each other, and will not produce a larger kick.</para>
        /// </summary>
        void Kick(Vector2 degrees);

        /// <summary>
        /// The current transform overriding the actor camera.
        /// </summary>
        Transform targetOverride { get; }

        /// <summary>
        /// Override the actor camera to follow a given transform. 
        /// </summary>
        void SetTargetOverride(Transform target, SpatialCameraMode cameraMode);

        /// <summary>
        /// Clear the current actor camera target override.
        /// </summary>
        void ClearTargetOverride();

        #region Unity Main Camera Functionality

        // Spatial Limits access to the internal <c>Main Camera</c> component. This interface provides a limited subset
        // of useful camera properties and functions.

        /// <summary>
        /// Returns a copy of the main camera. This is useful for creating a new camera with the same properties and transform as the main camera.
        /// <para><see href="https://docs.unity3d.com/ScriptReference/Camera.CopyFrom.html">Unity docs</see></para>
        /// </summary>
        void CopyFromMainCamera(Camera camera);

        /// <summary>
        /// Matrix that transforms from camera space to world space (Read Only).
        /// <para><see href="https://docs.unity3d.com/ScriptReference/Camera-cameraToWorldMatrix.html">Unity docs</see></para>
        /// </summary>
        Matrix4x4 cameraToWorldMatrix { get; }

        /// <summary>
        /// How tall is the camera in pixels (not accounting for dynamic resolution scaling) (Read Only).
        /// <para><see href="https://docs.unity3d.com/ScriptReference/Camera-pixelHeight.html">Unity docs</see></para>
        /// </summary>
        int pixelHeight { get; }

        /// <summary>
        /// How wide is the camera in pixels (not accounting for dynamic resolution scaling) (Read Only).
        /// <para><see href="https://docs.unity3d.com/ScriptReference/Camera-pixelWidth.html">Unity docs</see></para>
        /// </summary>
        int pixelWidth { get; }

        /// <summary>
        /// How tall is the camera in pixels (accounting for dynamic resolution scaling) (Read Only).
        /// <para><see href="https://docs.unity3d.com/ScriptReference/Camera-scaledPixelHeight.html">Unity docs</see></para>
        /// </summary>
        int scaledPixelHeight { get; }

        /// <summary>
        /// How wide is the camera in pixels (accounting for dynamic resolution scaling) (Read Only).
        /// <para><see href="https://docs.unity3d.com/ScriptReference/Camera-scaledPixelWidth.html">Unity docs</see></para>
        /// </summary>
        int scaledPixelWidth { get; }

        /// <summary>
        /// Main camera rect (Read Only).
        /// </summary>
        Rect rect { get; }

        /// <summary>
        /// Get the world-space speed of the camera (Read Only).
        /// <para><see href="https://docs.unity3d.com/ScriptReference/Camera-velocity.html">Unity docs</see></para>
        /// </summary>
        Vector3 velocity { get; }

        /// <summary>
        /// Matrix that transforms from world to camera space. (Read Only).
        /// <para><see href="https://docs.unity3d.com/ScriptReference/Camera-worldToCameraMatrix.html">Unity docs</see></para>
        /// </summary>
        Matrix4x4 worldToCameraMatrix { get; }

        /// <summary>
        /// Gets the projection matrix of the camera. (Read Only).
        /// <para><see href="https://docs.unity3d.com/ScriptReference/Camera-projectionMatrix.html">Unity docs</see></para>
        /// </summary>
        Matrix4x4 projectionMatrix { get; }

        /// <summary>
        /// Gets the left or right view matrix of a specific stereoscopic eye. (Read Only).
        /// <para><see href="https://docs.unity3d.com/ScriptReference/Camera.GetStereoViewMatrix.html">Unity docs</see></para>
        /// </summary>
        Matrix4x4 GetStereoViewMatrix(Camera.StereoscopicEye eye);

        /// <summary>
        /// Gets the projection matrix of a specific left or right stereoscopic eye. (Read Only).
        /// <para><see href="https://docs.unity3d.com/ScriptReference/Camera.GetStereoProjectionMatrix.html">Unity docs</see></para>
        /// </summary>
        Matrix4x4 GetStereoProjectionMatrix(Camera.StereoscopicEye eye);

        /// <summary>
        /// Given viewport coordinates, calculates the view space vectors pointing to the four frustum corners at the specified camera depth.
        /// <para><see href="https://docs.unity3d.com/ScriptReference/Camera.CalculateFrustumCorners.html">Unity docs</see></para>
        /// </summary>
        void CalculateFrustumCorners(Rect viewport, float z, Camera.MonoOrStereoscopicEye eye, Vector3[] outCorners);

        /// <summary>
        /// Calculates and returns oblique near-plane projection matrix.
        /// <para><see href="https://docs.unity3d.com/ScriptReference/Camera.CalculateObliqueMatrix.html">Unity docs</see></para>
        /// </summary>
        Matrix4x4 CalculateObliqueMatrix(Vector4 clipPlane);

        /// <summary>
        /// Returns a ray going from camera through a screen point.
        /// <para><see href="https://docs.unity3d.com/ScriptReference/Camera.ScreePointToRay.html">Unity docs</see></para>
        /// </summary>
        Ray ScreenPointToRay(Vector3 pos);

        /// <summary>
        /// Returns a ray going from camera through a screen point.
        /// <para><see href="https://docs.unity3d.com/ScriptReference/Camera.ScreePointToRay.html">Unity docs</see></para>
        /// </summary>
        Ray ScreenPointToRay(Vector3 pos, Camera.MonoOrStereoscopicEye eye);

        /// <summary>
        /// Transforms position from screen space into viewport space.
        /// <para><see href="https://docs.unity3d.com/ScriptReference/Camera.ScreenToViewportPoint.html">Unity docs</see></para>
        /// </summary>
        Vector3 ScreenToViewportPoint(Vector3 pos);

        /// <summary>
        /// Transforms a point from screen space into world space, where world space is defined as the coordinate system at the very top of your game's hierarchy.
        /// <para><see href="https://docs.unity3d.com/ScriptReference/Camera.ScreenToWorldPoint.html">Unity docs</see></para>
        /// </summary>
        Vector3 ScreenToWorldPoint(Vector3 pos);

        /// <summary>
        /// Transforms a point from screen space into world space, where world space is defined as the coordinate system at the very top of your game's hierarchy.
        /// <para><see href="https://docs.unity3d.com/ScriptReference/Camera.ScreenToWorldPoint.html">Unity docs</see></para>
        /// </summary>
        Vector3 ScreenToWorldPoint(Vector3 pos, Camera.MonoOrStereoscopicEye eye);

        /// <summary>
        /// Returns a ray going from camera through a viewport point.
        /// <para><see href="https://docs.unity3d.com/ScriptReference/Camera.ViewportPointToRay.html">Unity docs</see></para>
        /// </summary>
        Ray ViewportPointToRay(Vector3 pos);

        /// <summary>
        /// Returns a ray going from camera through a viewport point.
        /// <para><see href="https://docs.unity3d.com/ScriptReference/Camera.ViewportPointToRay.html">Unity docs</see></para>
        /// </summary>
        Ray ViewportPointToRay(Vector3 pos, Camera.MonoOrStereoscopicEye eye);

        /// <summary>
        /// Transforms position from viewport space into screen space.
        /// <para><see href="https://docs.unity3d.com/ScriptReference/Camera.ViewportToScreenPoint.html">Unity docs</see></para>
        /// </summary>
        Vector3 ViewportToScreenPoint(Vector3 pos);

        /// <summary>
        /// Transforms position from viewport space into world space.
        /// <para><see href="https://docs.unity3d.com/ScriptReference/Camera.ViewportToWorldPoint.html">Unity docs</see></para>
        /// </summary>
        Vector3 ViewportToWorldPoint(Vector3 pos);

        /// <summary>
        /// Transforms position from world space into screen space.
        /// <para><see href="https://docs.unity3d.com/ScriptReference/Camera.WorldToScreenPoint.html">Unity docs</see></para>
        /// </summary>
        Vector3 WorldToScreenPoint(Vector3 pos);

        /// <summary>
        /// Transforms position from world space into screen space.
        /// <para><see href="https://docs.unity3d.com/ScriptReference/Camera.WorldToScreenPoint.html">Unity docs</see></para>
        /// </summary>
        Vector3 WorldToScreenPoint(Vector3 pos, Camera.MonoOrStereoscopicEye eye);

        /// <summary>
        /// Transforms position from world space into viewport space.
        /// <para><see href="https://docs.unity3d.com/ScriptReference/Camera.WorldToViewportPoint.html">Unity docs</see></para>
        /// </summary>
        Vector3 WorldToViewportPoint(Vector3 pos);

        /// <summary>
        /// Transforms position from world space into viewport space.
        /// <para><see href="https://docs.unity3d.com/ScriptReference/Camera.WorldToViewportPoint.html">Unity docs</see></para>
        /// </summary>
        Vector3 WorldToViewportPoint(Vector3 pos, Camera.MonoOrStereoscopicEye eye);

        //* Other useful methods that normally require a Camera component input:

        /// <summary>
        /// Calculates frustum planes using the worldToProjection matrix from the main camera. Equivalent to
        /// <c>GeometryUtility.CalculateFrustumPlanes(Camera.main)</c>.
        /// <para><see href="https://docs.unity3d.com/ScriptReference/GeometryUtility.CalculateFrustumPlanes.html">Unity docs</see></para>
        /// </summary>
        Plane[] CalculateFrustumPlanes();

        #endregion
    }

    [DocumentationCategory("Services/Camera Service")]
    public enum SpatialCameraMode
    {
        /// <summary>
        /// The default player / local actor camera.
        /// </summary>
        Actor = 0,

        /// <summary>
        /// The special camera mode for vehicles. The camera will rotate with the vehicles movement.
        /// </summary>
        Vehicle = 1
    }

    /// <summary>
    /// The method of rotation for the actor camera with respect to player inputs.
    /// How should the camera be controlled?
    /// </summary>
    [DocumentationCategory("Services/Camera Service")]
    public enum SpatialCameraRotationMode
    {
        /// <summary>
        /// Automatic rotation as player moves.
        /// </summary>
        AutoRotate = 0,

        /// <summary>
        /// Left mouse button or touch rotation.
        /// </summary>
        DragToRotate = 2,

        /// <summary>
        /// Locked to the cursor.
        /// Note that this mode is meant to be used for desktop/mouse-based devices (such as Web).
        /// Browsers don't allow you to enter PointerLock mode without a user interaction, so this mode will
        /// show a UI element to the user to click on to enter PointerLock mode.
        /// </summary>
        PointerLock_Locked = 1,

        /// <summary>
        /// Cursor is unlocked, but next non-ui click returns to PointerLock.
        /// </summary>
        PointerLock_Unlocked = 3,
    }

    /// <summary>
    /// The camera's point of view for a player in an XR headset
    /// </summary>
    [DocumentationCategory("Services/Camera Service")]
    public enum XRCameraMode
    {
        /// <summary>
        /// This will maintain the currently set mode if player controlled switching modes is allowed
        /// Otherwise, defaults to first person
        /// </summary>
        Default = 0,

        /// <summary>
        /// Player embodies their avatar
        /// The avatar's head and hand movements map to the player's own physical movements
        /// </summary>
        FirstPerson = 1,

        /// <summary>
        /// Player's point of view is placed behind their avatar
        /// The avatar's head and hand movements do not map to the player's own physical movements
        /// </summary>
        ThirdPerson = 2
    };
}
