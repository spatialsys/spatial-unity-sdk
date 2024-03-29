using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitCategory("Spatial\\Actor")]
    [UnitTitle("Local Actor: Set Maximum Jump Count")]
    [UnitSurtitle("Local Actor Control Settings")]
    [UnitShortTitle("Set Max Jump Count")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class SetLocalAvatarMaxJumpCountNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        [PortLabel("Maximum Jump Count")]
        public ValueInput maxJumpCount { get; private set; }

        protected override void Definition()
        {
            maxJumpCount = ValueInput<int>(nameof(maxJumpCount), 2); // This default should be matched with AvatarController
            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                SpatialBridge.actorService.localActor.avatar.maxJumpCount = f.GetValue<int>(maxJumpCount);
                return outputTrigger;
            });
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitCategory("Spatial\\Actor")]
    [UnitTitle("Local Actor: Get Maximum Jump Count")]
    [UnitSurtitle("Local Actor Control Settings")]
    [UnitShortTitle("Get Max Jump Count")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetLocalAvatarMaxJumpCountNode : Unit
    {
        [DoNotSerialize]
        [PortLabel("Maximum Jump Count")]
        public ValueOutput maxJumpCount { get; private set; }

        protected override void Definition()
        {
            maxJumpCount = ValueOutput<int>(nameof(maxJumpCount), (f) => SpatialBridge.actorService.localActor.avatar.maxJumpCount);
        }
    }
}
