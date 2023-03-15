using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Local Avatar: Play Emote Animation")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Play Local Avatar Emote Animation")]
    [UnitCategory("Spatial\\Actions")]
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

        protected override void Definition()
        {
            sku = ValueInput<string>(nameof(sku));

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                ClientBridge.PlayLocalAvatarEmoteAnimation?.Invoke(f.GetValue<string>(sku));
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }
    }
}
