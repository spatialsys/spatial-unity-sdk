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
    [UnitCategory("Events\\Spatial")]
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

    [UnitTitle("Spatial Point of Interest: Get Title")]
    [UnitSurtitle("Spatial Point of Interest")]
    [UnitShortTitle("Get Title")]
    [UnitCategory("Spatial\\Get Actions")]
    [TypeIcon(typeof(SpatialPointOfInterest))]
    public class GetPointOfInterestTitle : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        [NullMeansSelf]
        [PortLabelHidden]
        public ValueInput poi { get; private set; }

        [DoNotSerialize]
        public ValueOutput title { get; private set; }

        private string result = "";

        protected override void Definition()
        {
            poi = ValueInput<SpatialPointOfInterest>(nameof(poi), null).NullMeansSelf();

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                SpatialPointOfInterest p = f.GetValue<SpatialPointOfInterest>(poi);
                if (p != null)
                {
                    result = p.title;
                }
                else
                {
                    result = "";
                }
                return outputTrigger;
            });
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);

            title = ValueOutput<string>(nameof(title), (f) => result);
        }
    }

    [UnitTitle("Spatial Point of Interest: Get Description")]
    [UnitSurtitle("Spatial Point of Interest")]
    [UnitShortTitle("Get Description")]
    [UnitCategory("Spatial\\Get Actions")]
    [TypeIcon(typeof(SpatialPointOfInterest))]
    public class GetPointOfInterestDescription : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        [NullMeansSelf]
        [PortLabelHidden]
        public ValueInput poi { get; private set; }

        [DoNotSerialize]
        public ValueOutput description { get; private set; }

        private string result = "";

        protected override void Definition()
        {
            poi = ValueInput<SpatialPointOfInterest>(nameof(poi), null).NullMeansSelf();

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                SpatialPointOfInterest p = f.GetValue<SpatialPointOfInterest>(poi);
                if (p != null)
                {
                    result = p.description;
                }
                else
                {
                    result = "";
                }
                return outputTrigger;
            });
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);

            description = ValueOutput<string>(nameof(description), (f) => result);
        }
    }
}
