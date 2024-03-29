using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitCategory("Spatial\\Actor")]
    [UnitTitle("Local Actor: Set Use Variable Height Jump")]
    [UnitSurtitle("Local Actor Control Settings")]
    [UnitShortTitle("Set Use Variable Height Jump")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class SetLocalAvatarUseVariableHeightJumpNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        [PortLabel("Use Variable Height Jump")]
        public ValueInput useVariableHeight { get; private set; }

        protected override void Definition()
        {
            useVariableHeight = ValueInput<bool>(nameof(useVariableHeight), true); // This default should be matched with AvatarController
            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                SpatialBridge.actorService.localActor.avatar.useVariableHeightJump = f.GetValue<bool>(useVariableHeight);
                return outputTrigger;
            });
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitCategory("Spatial\\Actor")]
    [UnitTitle("Local Actor: Get Use Variable Height Jump")]
    [UnitSurtitle("Local Actor Control Settings")]
    [UnitShortTitle("Get Use Variable Height Jump")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetLocalAvatarUseVariableHeightJumpNode : Unit
    {
        [DoNotSerialize]
        [PortLabel("Use Variable Height Jump")]
        public ValueOutput useVariableHeight { get; private set; }

        protected override void Definition()
        {
            useVariableHeight = ValueOutput<bool>(nameof(useVariableHeight), (f) => SpatialBridge.actorService.localActor.avatar.useVariableHeightJump);
        }
    }
}
