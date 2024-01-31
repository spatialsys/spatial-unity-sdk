using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial System: Get Space Package Version")]
    [UnitSurtitle("Spatial System")]
    [UnitShortTitle("Get Space Package Version")]
    [UnitCategory("Spatial\\System")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetSpacePackageVersionNode : Unit
    {
        [DoNotSerialize]
        public ValueOutput version { get; private set; }

        protected override void Definition()
        {
            version = ValueOutput<int>(nameof(version), (f) => SpatialBridge.spaceService.spacePackageVersion);
        }
    }
}
