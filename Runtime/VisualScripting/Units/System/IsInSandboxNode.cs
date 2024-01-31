using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial System: Is In Sandbox")]
    [UnitShortTitle("Is In Sandbox")]
    [UnitSurtitle("Spatial System")]
    [UnitCategory("Spatial\\System")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class IsInSandboxNode : Unit
    {
        [DoNotSerialize]
        public ValueOutput inSandbox { get; private set; }

        protected override void Definition()
        {
            inSandbox = ValueOutput<bool>(nameof(inSandbox), (f) => SpatialBridge.spaceService.isSandbox);
        }
    }
}
