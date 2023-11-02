using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{

    [UnitTitle("Actor: On Custom Variable Changed")]
    [UnitSurtitle("Actor")]
    [UnitShortTitle("On Custom Variable Changed")]
    [UnitSubtitle("Event")]
    [UnitCategory("Events\\Spatial\\Actor")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class OnActorCustomVariableChanged : EventUnit<(int, string, object)>
    {
        public static string eventName = "OnActorCustomVariableChanged";
        protected override bool register => true;

        [DoNotSerialize]
        public ValueOutput actor { get; private set; }
        [DoNotSerialize]
        public ValueOutput variableName { get; private set; }
        [DoNotSerialize]
        public ValueOutput variableValue { get; private set; }

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(eventName);
        }

        protected override void Definition()
        {
            base.Definition();
            actor = ValueOutput<int>(nameof(actor));
            variableName = ValueOutput<string>(nameof(variableName));
            variableValue = ValueOutput<object>(nameof(variableValue));
        }

        protected override bool ShouldTrigger(Flow flow, (int, string, object) args)
        {
            return true;
        }

        protected override void AssignArguments(Flow flow, (int, string, object) args)
        {
            flow.SetValue(actor, args.Item1);
            flow.SetValue(variableName, args.Item2);
            flow.SetValue(variableValue, args.Item3);
        }
    }
}
