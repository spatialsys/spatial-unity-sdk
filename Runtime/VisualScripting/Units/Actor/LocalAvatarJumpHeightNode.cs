using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitCategory("Spatial\\Actor")]
    [UnitTitle("Local Actor: Set Jump Height")]
    [UnitSurtitle("Local Actor Control Settings")]
    [UnitShortTitle("Set Jump Height (meter)")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class SetLocalAvatarJumpHeightNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        [PortLabel("Jump Height")]
        public ValueInput height { get; private set; }

        protected override void Definition()
        {
            height = ValueInput<float>(nameof(height), 1.5f); // This default should be matched with AvatarController
            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                SpatialBridge.actorService.localActor.avatar.jumpHeight = f.GetValue<float>(height);
                return outputTrigger;
            });
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitCategory("Spatial\\Actor")]
    [UnitTitle("Local Actor: Get Jump Height")]
    [UnitSurtitle("Local Actor Control Settings")]
    [UnitShortTitle("Get Jump Height (meter)")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetLocalAvatarJumpHeightNode : Unit
    {
        [DoNotSerialize]
        [PortLabel("Jump Height")]
        public ValueOutput height { get; private set; }

        protected override void Definition()
        {
            height = ValueOutput<float>(nameof(height), (f) => SpatialBridge.actorService.localActor.avatar.jumpHeight);
        }
    }
}
