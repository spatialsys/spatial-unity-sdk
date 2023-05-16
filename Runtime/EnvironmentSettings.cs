using UnityEngine;

namespace SpatialSys.UnitySDK
{
    [System.Serializable]
    public class EnvironmentSettings
    {
        public enum AvatarControlSettings
        {
            Default,
            Override,
        }

        public bool disableTeleport = false;
        public bool useSeatsAsSpawnPoints = false;

        [Space(5), Tooltip("Web Only")]
        public SpatialCameraRotationMode cameraRotationMode = SpatialCameraRotationMode.AutoRotate;

        public AvatarControlSettings avatarControlSettings = AvatarControlSettings.Default;

        // Default values set below should be set to be backwards compatible: these values are loaded in packages using old SDK version that don't have these settings available.
        [HideInInspector, MinAttribute(0.0f), Tooltip("The normal movement speed (m/s)")]
        public float localAvatarMovingSpeed = 3.0f;

        [HideInInspector, MinAttribute(0.0f), Tooltip("Movement speed when running (m/s)")]
        public float localAvatarRunSpeed = 5.5f;

        [HideInInspector, MinAttribute(0.1f), Tooltip("Jump height in meters")]
        public float localAvatarJumpHeight = 1.2f;

        [HideInInspector, MinAttribute(0.1f), Tooltip("Gravity multiplier - based on the physics gravity Y value")]
        public float localAvatarGravityMultiplier = 1.5f;

        [HideInInspector, MinAttribute(0.1f), Tooltip("Additional gravity multiplier used when falling - Stacks with the default gravity multiplier defined above")]
        public float localAvatarFallingGravityMultiplier = 1.0f;

        [HideInInspector, Tooltip("Jump higher depending on how long jump button is held")]
        public bool localAvatarUseVariableHeightJump = false;

        [HideInInspector, Tooltip("Maximum jump count that can be performed")]
        public int localAvatarMaxJumpCount = 1;
    }
}
