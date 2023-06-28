using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial Interactable: On Interact")]
    [UnitSurtitle("Spatial Interactable")]
    [UnitShortTitle("On Interact")]
    [UnitSubtitle("Event")]
    [UnitCategory("Events\\Spatial\\Interactable")]
    [TypeIcon(typeof(SpatialInteractable))]
    public class SpatialInteractableOnInteract : EventUnit<SpatialInteractable>
    {
        public static string eventName = "OnSpatialInteractableInteract";

        [NullMeansSelf]
        [PortLabelHidden]
        [DoNotSerialize]
        public ValueInput interactable { get; private set; }
        protected override bool register => true;

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(eventName);
        }

        protected override void Definition()
        {
            base.Definition();
            interactable = ValueInput<SpatialInteractable>(nameof(interactable), null).NullMeansSelf();
        }

        protected override bool ShouldTrigger(Flow flow, SpatialInteractable args)
        {
            if (args == null)
            {
                return false;
            }
            if (flow.GetValue<SpatialInteractable>(interactable) == args)
            {
                return true;
            }
            return false;
        }
    }

    [UnitTitle("Spatial Interactable: On Enter")]
    [UnitSurtitle("Spatial Interactable")]
    [UnitShortTitle("On Enter")]
    [UnitSubtitle("Event")]
    [UnitCategory("Events\\Spatial\\Interactable")]
    [TypeIcon(typeof(SpatialInteractable))]
    public class SpatialInteractableOnEnter : EventUnit<SpatialInteractable>
    {
        public static string eventName = "OnSpatialInteractableEnter";

        [NullMeansSelf]
        [PortLabelHidden]
        [DoNotSerialize]
        public ValueInput interactable { get; private set; }
        protected override bool register => true;

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(eventName);
        }

        protected override void Definition()
        {
            base.Definition();
            interactable = ValueInput<SpatialInteractable>(nameof(interactable), null).NullMeansSelf();
        }

        protected override bool ShouldTrigger(Flow flow, SpatialInteractable args)
        {
            if (args == null)
            {
                return false;
            }
            if (flow.GetValue<SpatialInteractable>(interactable) == args)
            {
                return true;
            }
            return false;
        }
    }

    [UnitTitle("Spatial Interactable: On Exit")]
    [UnitSurtitle("Spatial Interactable")]
    [UnitShortTitle("On Exit")]
    [UnitSubtitle("Event")]
    [UnitCategory("Events\\Spatial\\Interactable")]
    [TypeIcon(typeof(SpatialInteractable))]
    public class SpatialInteractableOnExit : EventUnit<SpatialInteractable>
    {
        public static string eventName = "OnSpatialInteractableExit";

        [NullMeansSelf]
        [PortLabelHidden]
        [DoNotSerialize]
        public ValueInput interactable { get; private set; }
        protected override bool register => true;

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(eventName);
        }

        protected override void Definition()
        {
            base.Definition();
            interactable = ValueInput<SpatialInteractable>(nameof(interactable), null).NullMeansSelf();
        }

        protected override bool ShouldTrigger(Flow flow, SpatialInteractable args)
        {
            if (args == null)
            {
                return false;
            }
            if (flow.GetValue<SpatialInteractable>(interactable) == args)
            {
                return true;
            }
            return false;
        }
    }
}
