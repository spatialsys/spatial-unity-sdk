using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Local Actor: Set Custom Variable")]
    [UnitSurtitle("Local Actor")]
    [UnitShortTitle("Set Custom Variable")]
    [UnitCategory("Spatial\\Actor")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class SetLocalActorCustomVariableNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput variableName { get; private set; }

        [DoNotSerialize]
        public ValueInput variableValue { get; private set; }

        protected override void Definition()
        {
            variableName = ValueInput<string>(nameof(variableName), "");
            variableValue = ValueInput<object>(nameof(variableValue), null);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                SpatialBridge.SetLocalActorCustomVariable?.Invoke(f.GetValue<string>(variableName), f.GetValue<object>(variableValue));
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitTitle("Local Actor: Get Custom Variable")]
    [UnitSurtitle("Local Actor")]
    [UnitShortTitle("Get Custom Variable")]
    [UnitCategory("Spatial\\Actor")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetLocalActorCustomVariableNode : Unit
    {
        [DoNotSerialize]
        public ValueInput variableName { get; private set; }

        [DoNotSerialize]
        public ValueOutput value { get; private set; }

        protected override void Definition()
        {
            variableName = ValueInput<string>(nameof(variableName), "");
            value = ValueOutput<object>(nameof(value), (f) => SpatialBridge.GetLocalActorCustomVariable?.Invoke(f.GetValue<string>(variableName)) ?? null);
        }
    }

    [UnitTitle("Actor: Get Custom Variable")]
    [UnitSurtitle("Actor")]
    [UnitShortTitle("Get Custom Variable")]
    [UnitCategory("Spatial\\Actor")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetActorCustomVariableNode : Unit
    {
        [DoNotSerialize]
        public ValueInput actor { get; private set; }

        [DoNotSerialize]
        public ValueInput variableName { get; private set; }

        [DoNotSerialize]
        public ValueOutput value { get; private set; }

        protected override void Definition()
        {
            actor = ValueInput<int>(nameof(actor), -1);
            variableName = ValueInput<string>(nameof(variableName), "");
            value = ValueOutput<object>(nameof(value), (f) => SpatialBridge.GetActorCustomVariable?.Invoke(f.GetValue<int>(actor), f.GetValue<string>(variableName)) ?? null);
        }
    }
}
