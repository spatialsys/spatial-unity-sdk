using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitCategory("Spatial\\Actions")]
    [UnitTitle("Local Actor: Set Falling Gravity Multiplier")]

    [UnitSurtitle("Local Actor Control Settings")]
    [UnitShortTitle("Set Falling Gravity Multiplier")]

    [TypeIcon(typeof(SpatialComponentBase))]
    public class SetLocalAvatarFallingGravityMultiplierNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        [PortLabel("Gravity Multiplier")]

        public ValueInput multiplier { get; private set; }

        protected override void Definition()
        {
            multiplier = ValueInput<float>(nameof(multiplier), 1.0f); // This default should be matched with AvatarController
            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                ClientBridge.SetLocalAvatarFallingGravityMultiplier.Invoke(f.GetValue<float>(multiplier));
                return outputTrigger;
            });
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitCategory("Spatial\\Actions")]
    [UnitTitle("Local Actor: Get Falling Gravity Multiplier")]

    [UnitSurtitle("Local Actor Control Settings")]
    [UnitShortTitle("Get Falling Gravity Multiplier")]

    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetLocalAvatarFallingGravityMultiplierNode : Unit
    {
        [DoNotSerialize]
        [PortLabel("Gravity Multiplier")]
        public ValueOutput multiplier { get; private set; }

        protected override void Definition()
        {
            multiplier = ValueOutput<float>(nameof(multiplier), (f) => ClientBridge.GetLocalAvatarFallingGravityMultiplier.Invoke());
        }
    }
}
