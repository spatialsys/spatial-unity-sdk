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
        public static string eventName = "SpatialOnVehicleSteerInput";
        protected override string hookName => eventName;
        public override Type MessageListenerType => null;

        [Serialize, Inspectable, UnitHeaderInspectable]
        public InputPhase inputPhase;

        [DoNotSerialize]
        public ValueOutput steer { get; private set; }

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
        public static string eventName = "SpatialOnVehicleThrottleInput";
        protected override string hookName => eventName;
        public override Type MessageListenerType => null;

        [Serialize, Inspectable, UnitHeaderInspectable]
        public InputPhase inputPhase;

        [DoNotSerialize]
        public ValueOutput throttle { get; private set; }

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
        public static string eventName = "SpatialOnVehicleReverseInput";
        protected override string hookName => eventName;
        public override Type MessageListenerType => null;

        [Serialize, Inspectable, UnitHeaderInspectable]
        public InputPhase inputPhase;

        [DoNotSerialize]
        public ValueOutput reverse { get; private set; }

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
        public static string eventName = "SpatialOnVehiclePrimaryActionInput";
        protected override string hookName => eventName;
        public override Type MessageListenerType => null;

        [Serialize, Inspectable, UnitHeaderInspectable]
        public InputPhase inputPhase;

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
        public static string eventName = "SpatialOnVehicleSecondaryActionInput";
        protected override string hookName => eventName;
        public override Type MessageListenerType => null;

        [Serialize, Inspectable, UnitHeaderInspectable]
        public InputPhase inputPhase;

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
        public static string eventName = "SpatialOnExitVehicleInput";
        protected override string hookName => eventName;
        public override Type MessageListenerType => null;
        protected override bool ShouldTrigger(Flow flow, EmptyEventArgs args)
        {
            return true;
        }
    }
}