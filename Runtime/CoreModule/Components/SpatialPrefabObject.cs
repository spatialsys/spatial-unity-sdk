using SpatialSys.UnitySDK.Internal;
using UnityEngine;

namespace SpatialSys.UnitySDK
{
    [DocumentationCategory("Core/Components")]
    public class SpatialPrefabObject : SpatialPackageAsset, ISpatialComponentWithOwner
    {
        public override string prettyName => "Prefab Object";
        public override string tooltip => "This component is used to define a custom prefab object for Spatial";
        public override string documentationURL => "https://toolkit.spatial.io/docs/packages/prefab-object";

        [HideInInspector] public SpatialSeatHotspot[] seats;
        [HideInInspector] public SpatialTriggerEvent[] triggerEvents;
        [HideInInspector] public SpatialSyncedAnimator[] syncedAnimators;
        [HideInInspector] public SpatialInteractable[] interactables;
        [HideInInspector] public SpatialPointOfInterest[] pointsOfInterest;
        [HideInInspector] public SpatialSyncedObject[] syncedObjects;
        [HideInInspector] public Animator[] unsyncedAnimators;
        [HideInInspector] public SpatialEvent[] spatialEvents;
    }
}