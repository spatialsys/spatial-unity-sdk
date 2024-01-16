using UnityEngine;
using Unity.VisualScripting;
using SpatialSys.UnitySDK;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial Trigger On Enter")]
    [UnitCategory("Events\\Spatial\\Trigger Event")]
    [TypeIcon(typeof(SpatialTriggerEvent))]
    public class SpatialTriggerEventOnEnter : EventUnit<SpatialTriggerEvent>
    {
        private const string EVENT_HOOK_ID = "SpatialTriggerOnEnter";

        [NullMeansSelf]
        [PortLabelHidden]
        [DoNotSerialize]
        public ValueInput triggerRef { get; private set; }
        protected override bool register => true;

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EVENT_HOOK_ID);
        }

        public static void TriggerEvent(SpatialTriggerEvent triggerEvent)
        {
            EventBus.Trigger(EVENT_HOOK_ID, triggerEvent);
        }

        protected override void Definition()
        {
            base.Definition();
            triggerRef = ValueInput<SpatialTriggerEvent>(nameof(triggerRef), null).NullMeansSelf();
        }

        protected override bool ShouldTrigger(Flow flow, SpatialTriggerEvent args)
        {
            if (args == null)
            {
                return false;
            }
            if (flow.GetValue<SpatialTriggerEvent>(triggerRef) == args)
            {
                return true;
            }
            return false;
        }
    }

    [UnitTitle("Spatial Trigger On Exit")]
    [UnitCategory("Events\\Spatial\\Trigger Event")]
    [TypeIcon(typeof(SpatialTriggerEvent))]
    public class SpatialTriggerEventOnExit : EventUnit<SpatialTriggerEvent>
    {
        private const string EVENT_HOOK_ID = "SpatialTriggerOnExit";

        [NullMeansSelf]
        [PortLabelHidden]
        [DoNotSerialize]
        public ValueInput triggerRef { get; private set; }
        protected override bool register => true;

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EVENT_HOOK_ID);
        }

        public static void TriggerEvent(SpatialTriggerEvent triggerEvent)
        {
            EventBus.Trigger(EVENT_HOOK_ID, triggerEvent);
        }

        protected override void Definition()
        {
            base.Definition();
            triggerRef = ValueInput<SpatialTriggerEvent>(nameof(triggerRef), null).NullMeansSelf();
        }

        protected override bool ShouldTrigger(Flow flow, SpatialTriggerEvent args)
        {
            if (args == null)
            {
                return false;
            }
            if (flow.GetValue<SpatialTriggerEvent>(triggerRef) == args)
            {
                return true;
            }
            return false;
        }
    }
}