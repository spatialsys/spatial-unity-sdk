using System;
using UnityEngine;
using Unity.VisualScripting;
using SpatialSys.UnitySDK.Internal;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial Input: On Vehicle Steer")]
    [UnitSurtitle("Spatial Input")]
    [UnitShortTitle("On Vehicle Steer")]
    [UnitCategory("Events\\Spatial\\System")]
    [TypeIcon(typeof(InputIcon))]
    public class SpatialOnVehicleSteerInput : GameObjectEventUnit<(InputPhase, Vector2)>
    {
        private const string EVENT_HOOK_ID = "SpatialOnVehicleSteerInput";
        protected override string hookName => EVENT_HOOK_ID;
        public override Type MessageListenerType => null;

        [Serialize, Inspectable, UnitHeaderInspectable]
        public InputPhase inputPhase;

        [DoNotSerialize]
        public ValueOutput steer { get; private set; }

        public static void TriggerEvent(GameObject gameObject, InputPhase phase, Vector2 steer)
        {
            EventBus.Trigger(new EventHook(EVENT_HOOK_ID, gameObject), (phase, steer));
        }

        protected override void Definition()
        {
            steer = ValueOutput<Vector2>(nameof(steer));
            base.Definition();
        }

        protected override bool ShouldTrigger(Flow flow, (InputPhase, Vector2) args)
        {
            return inputPhase == args.Item1;
        }

        protected override void AssignArguments(Flow flow, (InputPhase, Vector2) args)
        {
            flow.SetValue(steer, args.Item2);
        }
    }

    [UnitTitle("Spatial Input: On Vehicle Throttle")]
    [UnitSurtitle("Spatial Input")]
    [UnitShortTitle("On Vehicle Throttle")]
    [UnitCategory("Events\\Spatial\\System")]
    [TypeIcon(typeof(InputIcon))]
    public class SpatialOnVehicleThrottleInput : GameObjectEventUnit<(InputPhase, float)>
    {
        private const string EVENT_HOOK_ID = "SpatialOnVehicleThrottleInput";
        protected override string hookName => EVENT_HOOK_ID;
        public override Type MessageListenerType => null;

        [Serialize, Inspectable, UnitHeaderInspectable]
        public InputPhase inputPhase;

        [DoNotSerialize]
        public ValueOutput throttle { get; private set; }

        public static void TriggerEvent(GameObject gameObject, InputPhase phase, float throttle)
        {
            EventBus.Trigger(new EventHook(EVENT_HOOK_ID, gameObject), (phase, throttle));
        }

        protected override void Definition()
        {
            base.Definition();
            throttle = ValueOutput<float>(nameof(throttle));
        }

        protected override bool ShouldTrigger(Flow flow, (InputPhase, float) args)
        {
            return inputPhase == args.Item1;
        }

        protected override void AssignArguments(Flow flow, (InputPhase, float) args)
        {
            flow.SetValue(throttle, args.Item2);
        }
    }

    [UnitTitle("Spatial Input: On Vehicle Reverse")]
    [UnitSurtitle("Spatial Input")]
    [UnitShortTitle("On Vehicle Reverse")]
    [UnitCategory("Events\\Spatial\\System")]
    [TypeIcon(typeof(InputIcon))]
    public class SpatialOnVehicleReverseInput : GameObjectEventUnit<(InputPhase, float)>
    {
        private const string EVENT_HOOK_ID = "SpatialOnVehicleReverseInput";
        protected override string hookName => EVENT_HOOK_ID;
        public override Type MessageListenerType => null;

        [Serialize, Inspectable, UnitHeaderInspectable]
        public InputPhase inputPhase;

        [DoNotSerialize]
        public ValueOutput reverse { get; private set; }

        public static void TriggerEvent(GameObject gameObject, InputPhase phase, float reverse)
        {
            EventBus.Trigger(new EventHook(EVENT_HOOK_ID, gameObject), (phase, reverse));
        }

        protected override void Definition()
        {
            base.Definition();
            reverse = ValueOutput<float>(nameof(reverse));
        }

        protected override bool ShouldTrigger(Flow flow, (InputPhase, float) args)
        {
            return inputPhase == args.Item1;
        }

        protected override void AssignArguments(Flow flow, (InputPhase, float) args)
        {
            flow.SetValue(reverse, args.Item2);
        }
    }

    [UnitTitle("Spatial Input: On Vehicle Primary Action")]
    [UnitSurtitle("Spatial Input")]
    [UnitShortTitle("On Vehicle Primary Action")]
    [UnitCategory("Events\\Spatial\\System")]
    [TypeIcon(typeof(InputIcon))]
    public class SpatialOnVehiclePrimaryActionInput : GameObjectEventUnit<InputPhase>
    {
        private const string EVENT_HOOK_ID = "SpatialOnVehiclePrimaryActionInput";
        protected override string hookName => EVENT_HOOK_ID;
        public override Type MessageListenerType => null;

        [Serialize, Inspectable, UnitHeaderInspectable]
        public InputPhase inputPhase;

        public static void TriggerEvent(GameObject gameObject, InputPhase phase)
        {
            EventBus.Trigger(new EventHook(EVENT_HOOK_ID, gameObject), phase);
        }

        protected override void Definition()
        {
            base.Definition();
        }

        protected override bool ShouldTrigger(Flow flow, InputPhase args)
        {
            return inputPhase == args;
        }
    }

    [UnitTitle("Spatial Input: On Vehicle Secondary Action")]
    [UnitSurtitle("Spatial Input")]
    [UnitShortTitle("On Vehicle Secondary Action")]
    [UnitCategory("Events\\Spatial\\System")]
    [TypeIcon(typeof(InputIcon))]
    public class SpatialOnVehicleSecondaryActionInput : GameObjectEventUnit<InputPhase>
    {
        private const string EVENT_HOOK_ID = "SpatialOnVehicleSecondaryActionInput";
        protected override string hookName => EVENT_HOOK_ID;
        public override Type MessageListenerType => null;

        [Serialize, Inspectable, UnitHeaderInspectable]
        public InputPhase inputPhase;

        public static void TriggerEvent(GameObject gameObject, InputPhase phase)
        {
            EventBus.Trigger(new EventHook(EVENT_HOOK_ID, gameObject), phase);
        }

        protected override void Definition()
        {
            base.Definition();
        }

        protected override bool ShouldTrigger(Flow flow, InputPhase args)
        {
            return inputPhase == args;
        }
    }

    [UnitTitle("Spatial Input: On Exit Vehicle")]
    [UnitSurtitle("Spatial Input")]
    [UnitShortTitle("On Exit Vehicle")]
    [UnitCategory("Events\\Spatial\\System")]
    [TypeIcon(typeof(InputIcon))]
    public class SpatialOnExitVehicleInput : GameObjectEventUnit<EmptyEventArgs>
    {
        private const string EVENT_HOOK_ID = "SpatialOnExitVehicleInput";
        protected override string hookName => EVENT_HOOK_ID;
        public override Type MessageListenerType => null;

        public static void TriggerEvent(GameObject gameObject)
        {
            EventBus.Trigger(new EventHook(EVENT_HOOK_ID, gameObject));
        }

        protected override bool ShouldTrigger(Flow flow, EmptyEventArgs args)
        {
            return true;
        }
    }
}