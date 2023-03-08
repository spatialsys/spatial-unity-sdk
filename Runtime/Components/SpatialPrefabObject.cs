using UnityEngine;

namespace SpatialSys.UnitySDK
{
    public class SpatialPrefabObject : SpatialPackageAsset
    {
        public override string prettyName => "Prefab Object";
        public override string tooltip => "This component is used to define a custom prefab object for Spatial";
        public override string documentationURL => "https://docs.spatial.io/components/custom-prefab-objects";
    }
}