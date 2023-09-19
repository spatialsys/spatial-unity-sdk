using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitCategory("Spatial\\Actor")]
    [UnitTitle("Local Actor: Play Emote Animation")]

    [UnitSurtitle("Local Actor")]
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

        [DoNotSerialize]
        public ValueInput loop { get; private set; }

        protected override void Definition()
        {
            sku = ValueInput<string>(nameof(sku), "");
            immediately = ValueInput<bool>(nameof(immediately), false);
            loop = ValueInput<bool>(nameof(loop), false);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                ClientBridge.PlayLocalAvatarEmoteAnimation?.Invoke(f.GetValue<string>(sku), f.GetValue<bool>(immediately), f.GetValue<bool>(loop));
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitCategory("Spatial\\Actor")]
    [UnitTitle("Local Actor: Stop Emote Animation")]
    [UnitSurtitle("Local Actor")]
    [UnitShortTitle("Stop Emote Animation")]
    [TypeIcon(typeof(SpatialAvatarAnimation))]
    public class StopAvatarEmoteAnimationNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        protected override void Definition()
        {
            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                ClientBridge.StopLocalAvatarEmoteAnimation?.Invoke();
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }
    }
}
