using UnityEngine;
using Unity.VisualScripting;
using SpatialSys.UnitySDK;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial Input: On Overridden Move")]
    [UnitSurtitle("Spatial Input")]
    [UnitShortTitle("On Overridden Move")]
    [UnitCategory("Events\\Spatial")]
    [TypeIcon(typeof(InputIcon))]
    public class SpatialOnOverriddenMoveInput : EventUnit<(InputPhase, Vector2)>
    {
        public static string eventName = "SpatialOnOverriddenMoveInput";
        protected override bool register => true;

        [Serialize, Inspectable, UnitHeaderInspectable]
        public InputPhase inputPhase;

        [DoNotSerialize]
        public ValueOutput movement { get; private set; }

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(eventName);
        }

        protected override void Definition()
        {
            base.Definition();
            movement = ValueOutput<Vector2>(nameof(movement));
        }

        protected override bool ShouldTrigger(Flow flow, (InputPhase, Vector2) args)
        {
            return inputPhase == args.Item1;
        }

        protected override void AssignArguments(Flow flow, (InputPhase, Vector2) args)
        {
            flow.SetValue(movement, args.Item2);
        }
    }

    [UnitTitle("Spatial Input: On Overridden Jump")]
    [UnitSurtitle("Spatial Input")]
    [UnitShortTitle("On Overridden Jump")]
    [UnitCategory("Events\\Spatial")]
    [TypeIcon(typeof(InputIcon))]
    public class SpatialOnOverriddenJumpInput : EventUnit<InputPhase>
    {
        public static string eventName = "SpatialOnOverriddenJumpInput";

        protected override bool register => true;

        [Serialize, Inspectable, UnitHeaderInspectable]
        public InputPhase inputPhase;

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(eventName);
        }

        protected override bool ShouldTrigger(Flow flow, InputPhase args)
        {
            return inputPhase == args;
        }
    }

    [UnitTitle("Spatial Input: On Overridden Sprint")]
    [UnitSurtitle("Spatial Input")]
    [UnitShortTitle("On Overridden Sprint")]
    [UnitCategory("Events\\Spatial")]
    [TypeIcon(typeof(InputIcon))]
    public class SpatialOnOverriddenSprintInput : EventUnit<InputPhase>
    {
        public static string eventName = "SpatialOnOverriddenSprintInput";

        protected override bool register => true;

        [Serialize, Inspectable, UnitHeaderInspectable]
        public InputPhase inputPhase;

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(eventName);
        }

        protected override bool ShouldTrigger(Flow flow, InputPhase args)
        {
            return inputPhase == args;
        }
    }

    [UnitTitle("Spatial Input: On Overridden Action Button")]
    [UnitSurtitle("Spatial Input")]
    [UnitShortTitle("On Overridden Action Button")]
    [UnitCategory("Events\\Spatial")]
    [TypeIcon(typeof(InputIcon))]
    public class SpatialOnOverriddenActionButtonInput : EventUnit<InputPhase>
    {
        public static string eventName = "SpatialOnOverriddenActionButtonInput";

        protected override bool register => true;

        [Serialize, Inspectable, UnitHeaderInspectable]
        public InputPhase inputPhase;

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(eventName);
        }

        protected override bool ShouldTrigger(Flow flow, InputPhase args)
        {
            return inputPhase == args;
        }
    }

    [UnitTitle("Spatial Input: On Auto Sprint Toggled On")]
    [UnitSurtitle("Spatial Input")]
    [UnitShortTitle("On Auto Sprint Toggled On")]
    [UnitCategory("Events\\Spatial")]
    [TypeIcon(typeof(InputIcon))]
    public class SpatialOnAutoSprintToggledOn : EventUnit<EmptyEventArgs>
    {
        public static string eventName = "SpatialOnAutoSprintToggledOn";

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