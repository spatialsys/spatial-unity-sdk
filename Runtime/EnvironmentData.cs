using System.Collections.Generic;
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

        public EmbeddedPackageAsset[] embeddedPackageAssets;

        private Dictionary<string, SpatialPackageAsset> _embeddedPackageAssetsLookup;
        public bool TryGetEmbeddedPackageAsset(string id, out SpatialPackageAsset asset)
        {
            if (_embeddedPackageAssetsLookup == null)
            {
                _embeddedPackageAssetsLookup = new Dictionary<string, SpatialPackageAsset>();
                foreach (EmbeddedPackageAsset em in embeddedPackageAssets)
                    _embeddedPackageAssetsLookup.Add(em.id, em.asset);
            }

            return _embeddedPackageAssetsLookup.TryGetValue(id, out asset);
        }
    }

    [System.Serializable]
    public class EmbeddedPackageAsset
    {
        public string id;
        public SpatialPackageAsset asset;
    }
}
