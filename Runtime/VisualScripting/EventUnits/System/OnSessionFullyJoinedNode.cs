using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial System: On Session Fully Joined")]
    [UnitSurtitle("Spatial System")]
    [UnitShortTitle("On Session Fully Joined")]
    [UnitSubtitle("Event")]
    [UnitCategory("Events\\Spatial\\System")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class OnSessionFullyJoinedNode : EventUnit<EmptyEventArgs>
    {
        public static string eventName = "OnSessionFullyJoined";

        protected override bool register => true;

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(eventName);
        }

        protected override bool ShouldTrigger(Flow flow, EmptyEventArgs args)
        {
            return true;
        }
    }
}
