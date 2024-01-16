using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Local Actor: Set Custom Property")]
    [UnitSurtitle("Local Actor")]
    [UnitShortTitle("Set Custom Property")]
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
        [PortLabel("name")]
        public ValueInput variableName { get; private set; }

        [DoNotSerialize]
        [PortLabel("value")]
        public ValueInput variableValue { get; private set; }

        protected override void Definition()
        {
            variableName = ValueInput<string>(nameof(variableName), "");
            variableValue = ValueInput<object>(nameof(variableValue), null);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                SpatialBridge.actorService.localActor.SetCustomProperty(f.GetValue<string>(variableName), f.GetValue<object>(variableValue));
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitTitle("Local Actor: Get Custom Property")]
    [UnitSurtitle("Local Actor")]
    [UnitShortTitle("Get Custom Property")]
    [UnitCategory("Spatial\\Actor")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetLocalActorCustomVariableNode : Unit
    {
        [DoNotSerialize]
        [PortLabel("propertyName")]
        public ValueInput variableName { get; private set; }

        [DoNotSerialize]
        public ValueOutput value { get; private set; }

        protected override void Definition()
        {
            variableName = ValueInput<string>(nameof(variableName), "");
            value = ValueOutput<object>(nameof(value), (f) => {
                string varName = f.GetValue<string>(variableName);
                if (string.IsNullOrEmpty(varName))
                {
                    SpatialBridge.loggingService.LogError($"GetLocalActorCustomPropertyNode: Property name must be a valid string, but was null or empty");
                    return null;
                }

                if (SpatialBridge.actorService.localActor.customProperties.TryGetValue(varName, out object value))
                    return value;

                // SpatialBridge.loggingService.LogError($"GetLocalActorCustomPropertyNode: Property name '{varName}' does not exist");
                return null;
            });
        }
    }

    [UnitTitle("Actor: Get Custom Property")]
    [UnitSurtitle("Actor")]
    [UnitShortTitle("Get Custom Property")]
    [UnitCategory("Spatial\\Actor")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetActorCustomVariableNode : Unit
    {
        [DoNotSerialize]
        public ValueInput actor { get; private set; }

        [DoNotSerialize]
        [PortLabel("propertyName")]
        public ValueInput variableName { get; private set; }

        [DoNotSerialize]
        public ValueOutput value { get; private set; }

        protected override void Definition()
        {
            actor = ValueInput<int>(nameof(actor), -1);
            variableName = ValueInput<string>(nameof(variableName), "");
            value = ValueOutput<object>(nameof(value), (f) => {
                string varName = f.GetValue<string>(variableName);
                if (string.IsNullOrEmpty(varName))
                {
                    SpatialBridge.loggingService.LogError($"GetActorCustomPropertyNode: Property name must be a valid string, but was null or empty");
                    return null;
                }

                int actorNumber = f.GetValue<int>(actor);
                if (!SpatialBridge.actorService.actors.TryGetValue(actorNumber, out IActor sdkActor))
                {
                    SpatialBridge.loggingService.LogError($"GetActorCustomPropertyNode: Actor with actor number '{actorNumber}' does not exist");
                    return null;
                }

                if (sdkActor.customProperties.TryGetValue(varName, out object value))
                    return value;

                // SpatialBridge.loggingService.LogError($"GetActorCustomPropertyNode: Property name '{varName}' does not exist");
                return null;
            });
        }
    }
}
