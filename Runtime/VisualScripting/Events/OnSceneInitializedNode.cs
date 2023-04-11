using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial System: On Scene Initialized")]
    [UnitSurtitle("Spatial System")]
    [UnitShortTitle("On Scene Initialized")]
    [UnitSubtitle("Event")]
    [UnitCategory("Events\\Spatial")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class OnSceneInitializedNode : EventUnit<EmptyEventArgs>
    {
        public static string eventName = "OnSceneInitialized";

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
