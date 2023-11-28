using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial System: Is Connected To Session")]
    [UnitSurtitle("Spatial System")]
    [UnitShortTitle("Is Connected To Session")]
    [UnitCategory("Spatial\\System")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetIsConnectedToSessionNode : Unit
    {
        [DoNotSerialize]
        public ValueOutput isConnected { get; private set; }
        protected override void Definition()
        {
            isConnected = ValueOutput<bool>(nameof(isConnected), (f) => SpatialBridge.GetIsConnectedToSession?.Invoke() ?? true);
        }
    }
}
