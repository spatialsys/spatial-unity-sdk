using System;
using UnityEngine;
using Unity.VisualScripting;
using SpatialSys.UnitySDK;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial Synced Variables: On Variable Changed")]
    [UnitSurtitle("Spatial Synced Variables")]
    [UnitShortTitle("On Synced Variable Changed")]
    [UnitCategory("Events\\Spatial\\Synced Object")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class SpatialSyncedVariablesOnVariableChanged : EventUnit<(SpatialSyncedVariables, string)>
    {
        public static string eventName = "SpatialSyncedVariablesOnVariableChanged";

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
            return new EventHook(eventName);
        }

        protected override void Definition()
        {
            base.Definition();
            syncedVariablesRef = ValueInput<SpatialSyncedVariables>(nameof(syncedVariablesRef), null).NullMeansSelf();
            variableName = ValueInput<string>(nameof(variableName), null);
            value = ValueOutput<object>(nameof(value));
        }

        protected override bool ShouldTrigger(Flow flow, (SpatialSyncedVariables, string) args)
        {
            if (flow.GetValue<SpatialSyncedVariables>(syncedVariablesRef) == args.Item1 && flow.GetValue<string>(variableName) == args.Item2)
            {
                return true;
            }
            return false;
        }

        protected override void AssignArguments(Flow flow, (SpatialSyncedVariables, string) args)
        {
            foreach (SpatialSyncedVariables.Data data in args.Item1.variableSettings)
            {
                if (data.name == args.Item2)
                {
                    flow.SetValue(value, data.declaration.value);
                    break;
                }
            }
        }
    }
}