using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitCategory("Spatial\\Actor")]
    [UnitTitle("Local Actor: Set Gravity Multiplier")]
    [UnitSurtitle("Local Actor Control Settings")]
    [UnitShortTitle("Set Gravity Multiplier")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class SetLocalAvatarGravityMultiplierNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        [PortLabel("Multiplier")]
        public ValueInput gravityMultiplier { get; private set; }

        protected override void Definition()
        {
            gravityMultiplier = ValueInput<float>(nameof(gravityMultiplier), 1.5f); // This default should be matched with AvatarController
            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                SpatialBridge.actorService.localActor.avatar.gravityMultiplier = f.GetValue<float>(gravityMultiplier);
                return outputTrigger;
            });
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitCategory("Spatial\\Actor")]
    [UnitTitle("Local Actor: Get Gravity Multiplier")]
    [UnitSurtitle("Local Actor Control Settings")]
    [UnitShortTitle("Get Gravity Multiplier")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetLocalAvatarGravityMultiplierNode : Unit
    {
        [DoNotSerialize]
        [PortLabel("Multiplier")]
        public ValueOutput gravityMultiplier { get; private set; }

        protected override void Definition()
        {
            gravityMultiplier = ValueOutput<float>(nameof(gravityMultiplier), (f) => SpatialBridge.actorService.localActor.avatar.gravityMultiplier);
        }
    }
}
