using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial: On Local Actor Emote")]
    [UnitSurtitle("Local Actor")]
    [UnitShortTitle("On Emote")]
    [UnitSubtitle("Event")]
    [UnitCategory("Events\\Spatial")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class OnLocalAvatarEmoteNode : EventUnit<EmptyEventArgs>
    {
        public static string eventName = "OnLocalAvatarEmote";

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
