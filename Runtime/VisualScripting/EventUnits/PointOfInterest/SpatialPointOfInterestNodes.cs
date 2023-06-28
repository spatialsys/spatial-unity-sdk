using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial Point of Interest: On Enter")]
    [UnitSurtitle("Spatial Point of Interest")]
    [UnitShortTitle("On Enter")]
    [UnitSubtitle("Event")]
    [UnitCategory("Events\\Spatial\\Point Of Interest")]
    [TypeIcon(typeof(SpatialPointOfInterest))]
    public class SpatialPointOfInterestOnEnter : EventUnit<SpatialPointOfInterest>
    {
        public static string eventName = "OnSpatialPointOfInterestEnter";

        [NullMeansSelf]
        [PortLabelHidden]
        [DoNotSerialize]
        public ValueInput poi { get; private set; }

        protected override bool register => true;

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(eventName);
        }

        protected override void Definition()
        {
            base.Definition();
            poi = ValueInput<SpatialPointOfInterest>(nameof(poi), null).NullMeansSelf();
        }

        protected override bool ShouldTrigger(Flow flow, SpatialPointOfInterest args)
        {
            if (args == null)
            {
                return false;
            }
            if (flow.GetValue<SpatialPointOfInterest>(poi) == args)
            {
                return true;
            }
            return false;
        }
    }
}
