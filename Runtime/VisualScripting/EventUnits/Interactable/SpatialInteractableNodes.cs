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
        private const string EVENT_HOOK_ID = "OnSpatialInteractableInteract";

        [NullMeansSelf]
        [PortLabelHidden]
        [DoNotSerialize]
        public ValueInput interactable { get; private set; }
        protected override bool register => true;

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EVENT_HOOK_ID);
        }

        public static void TriggerEvent(SpatialInteractable interactable)
        {
            EventBus.Trigger(EVENT_HOOK_ID, interactable);
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
        private const string EVENT_HOOK_ID = "OnSpatialInteractableEnter";

        [NullMeansSelf]
        [PortLabelHidden]
        [DoNotSerialize]
        public ValueInput interactable { get; private set; }
        protected override bool register => true;

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EVENT_HOOK_ID);
        }

        public static void TriggerEvent(SpatialInteractable interactable)
        {
            EventBus.Trigger(EVENT_HOOK_ID, interactable);
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
        private const string EVENT_HOOK_ID = "OnSpatialInteractableExit";

        [NullMeansSelf]
        [PortLabelHidden]
        [DoNotSerialize]
        public ValueInput interactable { get; private set; }
        protected override bool register => true;

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EVENT_HOOK_ID);
        }

        public static void TriggerEvent(SpatialInteractable interactable)
        {
            EventBus.Trigger(EVENT_HOOK_ID, interactable);
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
