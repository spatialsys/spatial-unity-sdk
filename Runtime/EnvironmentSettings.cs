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

        public const int VERSION = 1;
        public bool disableTeleport;
        public bool useSeatsAsSpawnPoints;

        public AvatarControlSettings avatarControlSettings = AvatarControlSettings.Default;
        [MinAttribute(0.1f), Tooltip("m/s"), HideInInspector] public float localAvatarMovingSpeed; // in m/s

        // Set default values
        public EnvironmentSettings()
        {
            disableTeleport = false;
            useSeatsAsSpawnPoints = false;
            avatarControlSettings = AvatarControlSettings.Default;
            localAvatarMovingSpeed = 3.0f; // This default should be matched with AvatarController movingSpeed.
        }
    }
}
