using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitCategory("Spatial\\Actions")]
    [UnitTitle("Local Avatar: Play Emote Animation")]

    [UnitSurtitle("Local Avatar")]
    [UnitShortTitle("Play Emote Animation")]

    [TypeIcon(typeof(SpatialAvatarAnimation))]
    public class PlayAvatarEmoteAnimationNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput sku { get; private set; }

        [DoNotSerialize]
        public ValueInput immediately { get; private set; }

        protected override void Definition()
        {
            sku = ValueInput<string>(nameof(sku), "");
            immediately = ValueInput<bool>(nameof(immediately), false);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                ClientBridge.PlayLocalAvatarEmoteAnimation?.Invoke(f.GetValue<string>(sku), f.GetValue<bool>(immediately));
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }
    }
}
