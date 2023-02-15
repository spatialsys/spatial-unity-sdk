using UnityEngine;
using Unity.VisualScripting;
using SpatialSys.UnitySDK;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial Trigger On Enter")]
    [TypeIcon(typeof(SpatialTriggerEvent))]
    public class SpatialTriggerEventOnEnter : EventUnit<SpatialTriggerEvent>
    {
        public static string eventName = "SpatialTriggerOnEnter";

        [NullMeansSelf]
        [PortLabelHidden]
        [DoNotSerialize]
        public ValueInput triggerRef { get; private set; }
        protected override bool register => true;

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(eventName);
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
    [TypeIcon(typeof(SpatialTriggerEvent))]
    public class SpatialTriggerEventOnExit : EventUnit<SpatialTriggerEvent>
    {
        public static string eventName = "SpatialTriggerOnExit";

        [NullMeansSelf]
        [PortLabelHidden]
        [DoNotSerialize]
        public ValueInput triggerRef { get; private set; }
        protected override bool register => true;

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(eventName);
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