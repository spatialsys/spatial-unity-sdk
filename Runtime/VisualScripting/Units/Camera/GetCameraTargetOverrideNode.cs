using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial Camera: Get Target Override")]
    [UnitSurtitle("Spatial Camera")]
    [UnitShortTitle("Get Target Override")]
    [UnitCategory("Spatial\\Camera")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetCameraTargetOverrideNode : Unit
    {
        [DoNotSerialize]
        [PortLabel("Target")]
        public ValueOutput target { get; private set; }

        protected override void Definition()
        {
            target = ValueOutput<Transform>(nameof(target), (f) => ClientBridge.GetCameraTargetOverride.Invoke());
        }
    }
}
