using System;
using UnityEngine;
using Unity.VisualScripting;
using SpatialSys.UnitySDK;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial Network Variables: On Variable Changed")]
    [UnitSurtitle("Spatial Network Variables")]
    [UnitShortTitle("On Network Variable Changed")]
    [UnitCategory("Events\\Spatial\\Network Object")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class SpatialSyncedVariablesOnVariableChanged : EventUnit<(SpatialNetworkVariables, string, object)>
    {
        private const string EVENT_HOOK_ID = "SpatialNetworkVariablesOnVariableChanged";

        [NullMeansSelf]
        [PortLabelHidden]
        [DoNotSerialize]
        public ValueInput syncedVariablesRef { get; private set; }

        [DoNotSerialize]
        public ValueInput variableName { get; private set; }

        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput value { get; private set; }

        protected override bool register => true;

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EVENT_HOOK_ID);
        }

        public static void TriggerEvent(SpatialNetworkVariables networkVariables, string variableName, object value)
        {
            EventBus.Trigger(EVENT_HOOK_ID, (networkVariables, variableName, value));
        }

        protected override void Definition()
        {
            base.Definition();
            syncedVariablesRef = ValueInput<SpatialNetworkVariables>(nameof(syncedVariablesRef), null).NullMeansSelf();
            variableName = ValueInput<string>(nameof(variableName), null);
            value = ValueOutput<object>(nameof(value));
        }

        protected override bool ShouldTrigger(Flow flow, (SpatialNetworkVariables, string, object) args)
        {
            if (flow.GetValue<SpatialNetworkVariables>(syncedVariablesRef) == args.Item1 && flow.GetValue<string>(variableName) == args.Item2)
            {
                return true;
            }
            return false;
        }

        protected override void AssignArguments(Flow flow, (SpatialNetworkVariables, string, object) args)
        {
            flow.SetValue(value, args.Item3);
        }
    }
}