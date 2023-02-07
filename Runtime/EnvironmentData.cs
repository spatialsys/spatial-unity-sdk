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
        public SpatialCameraPassthrough[] cameraPassthroughs;
        public SpatialThumbnailCamera thumbnailCamera;
        public SpatialProjectorSurface[] projectorSurfaces;

        public Animator[] unsyncedAnimators;

        public Volume[] renderingVolumes;

        // TODO: Have fog setting per variant.
        public bool enableFog;

        public EnvironmentSettings environmentSettings;

        public SpatialEvent[] spatialEvents;
    }
}
