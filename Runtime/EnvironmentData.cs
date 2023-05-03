using UnityEngine;
using UnityEngine.Rendering;

namespace SpatialSys.UnitySDK
{
    // Don't rename class name otherwise component reference will break.
    public class EnvironmentData : MonoBehaviour
    {
        public SpatialSeatHotspot[] seats;
        public SpatialEntrancePoint[] entrancePoints;
        public SpatialEmptyFrame[] emptyFrames;
        public SpatialSyncedAnimator[] syncedAnimators;
        public SpatialCameraPassthrough[] cameraPassthroughs;
        public SpatialThumbnailCamera thumbnailCamera;
        public SpatialProjectorSurface[] projectorSurfaces;
        public SpatialQuest[] quests;
        public SpatialSyncedObject[] syncedObjects;

        public Animator[] unsyncedAnimators;

        public Volume[] renderingVolumes;

        // TODO: Have fog setting per variant.
        public bool enableFog;

        public EnvironmentSettings environmentSettings;
        public SavedProjectSettings savedProjectSettings;

        public SpatialEvent[] spatialEvents;
    }
}
