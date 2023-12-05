using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial Sync: Get Network Time")]
    [UnitSurtitle("Spatial Sync")]
    [UnitShortTitle("Get Network Time")]
    [UnitCategory("Spatial\\Network")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetNetworkTimeNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput time { get; private set; }

        protected override void Definition()
        {
            time = ValueOutput<double>(nameof(time), (f) => SpatialBridge.networkingService.networkTime);
        }
    }
}
