using UnityEngine;
using UnityEngine.Rendering;

namespace SpatialSys.UnitySDK
{
    // TODO: Figure out how to rename to SceneData without breaking component reference from old bundles after client load
    public class EnvironmentData : MonoBehaviour
    {
        public const int VERSION = 1;

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
