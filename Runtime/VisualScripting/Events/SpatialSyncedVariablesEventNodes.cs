using System;
using UnityEngine;
using Unity.VisualScripting;
using SpatialSys.UnitySDK;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial Synced Variables: On Variable Changed")]
    [UnitSurtitle("Spatial Synced Variables")]
    [UnitShortTitle("On Synced Variable Changed")]
    [UnitCategory("Events\\Spatial")]
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
        }

        protected override bool ShouldTrigger(Flow flow, (SpatialSyncedVariables, string) args)
        {
            if (flow.GetValue<SpatialSyncedVariables>(syncedVariablesRef) == args.Item1 && flow.GetValue<string>(variableName) == args.Item2)
            {
                return true;
            }
            return false;
        }
    }
}