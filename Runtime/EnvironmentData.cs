using UnityEngine;
using UnityEngine.Rendering;

namespace SpatialSys.UnitySDK
{
    public class EnvironmentData : MonoBehaviour
    {
        public const int VERSION = 1;

        public SpatialSeatHotspot[] seats;
        public SpatialEntrancePoint[] entrancePoints;
        public SpatialTriggerEvent[] triggerEvents;
        public SpatialEmptyFrame[] emptyFrames;
        public SpatialAvatarTeleporter[] avatarTeleporters;
        public SpatialSyncedAnimator[] syncedAnimators;
        public Animator[] unsyncedAnimators;

        public Volume[] renderingVolumes;

        // TODO: Add other config data.
        // TODO: Have config data per variant.
        public bool enableFog;

        public SpatialEvent[] spatialEvents;
    }
}
