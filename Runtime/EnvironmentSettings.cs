using UnityEngine;

namespace SpatialSys.UnitySDK
{
    [System.Serializable]
    public class EnvironmentSettings
    {
        public const int VERSION = 1;
        public bool disableTeleport;
        public bool useSeatsAsSpawnPoints;

        // Set default values
        public EnvironmentSettings()
        {
            disableTeleport = false;
            useSeatsAsSpawnPoints = false;
        }
    }
}
