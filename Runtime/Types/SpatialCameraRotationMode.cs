namespace SpatialSys.UnitySDK
{
    // This should match SpatialUnity's enum
    public enum SpatialCameraRotationMode
    {
        /// <summary>Automatic rotation as player move</summary>
        AutoRotate = 0,
        /// <summary>Locked to the cursor</summary>
        PointerLock_Locked = 1,
        /// <summary>Left mouse button rotation</summary>
        DragToRotate = 2,
        /// <summary>Cursor is unlocked, but next non-ui click returns to PointerLock</summary>
        PointerLock_Unlocked = 3,
    }
}
