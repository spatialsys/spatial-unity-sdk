using System;
using UnityEngine;
using Unity.VisualScripting;
using SpatialSys.UnitySDK.Internal;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial Input: On Overridden Avatar Move")]
    [UnitSurtitle("Spatial Input")]
    [UnitShortTitle("On Overridden Avatar Move")]
    [UnitCategory("Events\\Spatial\\Input")]
    [TypeIcon(typeof(InputIcon))]
    public class SpatialOnOverriddenMoveInput : EventUnit<(InputPhase, Vector2)>
    {
        private const string EVENT_HOOK_ID = "SpatialOnOverriddenMoveInput";
        protected override bool register => true;

        [Serialize, Inspectable, UnitHeaderInspectable]
        public InputPhase inputPhase;

        [DoNotSerialize]
        public ValueOutput movement { get; private set; }

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EVENT_HOOK_ID);
        }

        public static void TriggerEvent(InputPhase phase, Vector2 movement)
        {
            EventBus.Trigger(EVENT_HOOK_ID, (phase, movement));
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

    [UnitTitle("Spatial Input: On Overridden Avatar Jump")]
    [UnitSurtitle("Spatial Input")]
    [UnitShortTitle("On Overridden Avatar Jump")]
    [UnitCategory("Events\\Spatial\\Input")]
    [TypeIcon(typeof(InputIcon))]
    public class SpatialOnOverriddenJumpInput : EventUnit<InputPhase>
    {
        private const string EVENT_HOOK_ID = "SpatialOnOverriddenJumpInput";
        protected override bool register => true;

        [Serialize, Inspectable, UnitHeaderInspectable]
        public InputPhase inputPhase;

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EVENT_HOOK_ID);
        }

        public static void TriggerEvent(InputPhase phase)
        {
            EventBus.Trigger(EVENT_HOOK_ID, phase);
        }

        protected override bool ShouldTrigger(Flow flow, InputPhase args)
        {
            return inputPhase == args;
        }
    }

    [UnitTitle("Spatial Input: On Overridden Avatar Sprint")]
    [UnitSurtitle("Spatial Input")]
    [UnitShortTitle("On Overridden Avatar Sprint")]
    [UnitCategory("Events\\Spatial\\Input")]
    [TypeIcon(typeof(InputIcon))]
    public class SpatialOnOverriddenSprintInput : EventUnit<InputPhase>
    {
        private const string EVENT_HOOK_ID = "SpatialOnOverriddenSprintInput";
        protected override bool register => true;

        [Serialize, Inspectable, UnitHeaderInspectable]
        public InputPhase inputPhase;

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EVENT_HOOK_ID);
        }

        public static void TriggerEvent(InputPhase phase)
        {
            EventBus.Trigger(EVENT_HOOK_ID, phase);
        }

        protected override bool ShouldTrigger(Flow flow, InputPhase args)
        {
            return inputPhase == args;
        }
    }

    [UnitTitle("Spatial Input: On Overridden Avatar Action Button")]
    [UnitSurtitle("Spatial Input")]
    [UnitShortTitle("On Overridden Action Button")]
    [UnitCategory("Events\\Spatial\\Input")]
    [TypeIcon(typeof(InputIcon))]
    public class SpatialOnOverriddenActionButtonInput : EventUnit<InputPhase>
    {
        private const string EVENT_HOOK_ID = "SpatialOnOverriddenActionButtonInput";
        protected override bool register => true;

        [Serialize, Inspectable, UnitHeaderInspectable]
        public InputPhase inputPhase;

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EVENT_HOOK_ID);
        }

        public static void TriggerEvent(InputPhase phase)
        {
            EventBus.Trigger(EVENT_HOOK_ID, phase);
        }

        protected override bool ShouldTrigger(Flow flow, InputPhase args)
        {
            return inputPhase == args;
        }
    }

    [UnitTitle("Spatial Input: On Auto Sprint Toggled On")]
    [UnitSurtitle("Spatial Input")]
    [UnitShortTitle("On Auto Sprint Toggled On")]
    [UnitCategory("Events\\Spatial\\Input")]
    [TypeIcon(typeof(InputIcon))]
    public class SpatialOnAutoSprintToggledOn : EventUnit<EmptyEventArgs>
    {
        private const string EVENT_HOOK_ID = "SpatialOnAutoSprintToggledOn";
        protected override bool register => true;

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EVENT_HOOK_ID);
        }

        public static void TriggerEvent(bool on)
        {
            if (on)
                EventBus.Trigger(EVENT_HOOK_ID);
        }

        protected override bool ShouldTrigger(Flow flow, EmptyEventArgs args)
        {
            return true;
        }
    }

    [UnitTitle("Spatial Input: On Input Capture Stopped")]
    [UnitSurtitle("Spatial Input")]
    [UnitShortTitle("On Input Capture Stopped")]
    [UnitCategory("Events\\Spatial\\Input")]
    [TypeIcon(typeof(InputIcon))]
    public class SpatialOnInputCaptureStopped : GameObjectEventUnit<EmptyEventArgs>
    {
        private const string EVENT_HOOK_ID = "SpatialOnInputCaptureStopped";
        protected override string hookName => EVENT_HOOK_ID;
        public override Type MessageListenerType => null;

        public static void TriggerEvent(InputCaptureType type)
        {
            EventBus.Trigger(EVENT_HOOK_ID);
        }

        protected override bool ShouldTrigger(Flow flow, EmptyEventArgs args)
        {
            return true;
        }
    }
}