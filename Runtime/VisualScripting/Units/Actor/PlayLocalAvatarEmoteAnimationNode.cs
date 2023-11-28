using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitCategory("Spatial\\Actor")]
    [UnitTitle("Local Actor: Play Emote Animation from Package")]
    [UnitSurtitle("Local Actor")]
    [UnitShortTitle("Play Emote Animation Package")]
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
        [PortLabel("Package SKU")]
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
                SpatialBridge.PlayLocalAvatarPackageEmote?.Invoke(f.GetValue<string>(sku), f.GetValue<bool>(immediately), f.GetValue<bool>(loop));
                return outputTrigger;
            });
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitCategory("Spatial\\Actor")]
    [UnitTitle("Local Actor: Play Emote Animation from Embedded Package Asset")]
    [UnitSurtitle("Local Actor")]
    [UnitShortTitle("Play Emote Animation Embedded")]
    [TypeIcon(typeof(SpatialAvatarAnimation))]
    public class PlayAvatarEmbeddedEmoteAnimationNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput assetID { get; private set; }

        [DoNotSerialize]
        public ValueInput immediately { get; private set; }

        [DoNotSerialize]
        public ValueInput loop { get; private set; }

        protected override void Definition()
        {
            assetID = ValueInput<string>(nameof(assetID), "");
            immediately = ValueInput<bool>(nameof(immediately), false);
            loop = ValueInput<bool>(nameof(loop), false);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                SpatialBridge.PlayLocalAvatarEmbeddedEmote?.Invoke(f.GetValue<string>(assetID), f.GetValue<bool>(immediately), f.GetValue<bool>(loop));
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
                SpatialBridge.StopLocalAvatarEmoteAnimation?.Invoke();
                return outputTrigger;
            });
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }
}
