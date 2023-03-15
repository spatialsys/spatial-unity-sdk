using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial: Set Local Avatar")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Set Local Avatar")]
    [UnitCategory("Spatial\\Actions")]
    [TypeIcon(typeof(SpatialAvatar))]
    public class SetLocalAvatarNode : Unit
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
                ClientBridge.SetLocalAvatar?.Invoke(f.GetValue<string>(sku));
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }
    }
}
