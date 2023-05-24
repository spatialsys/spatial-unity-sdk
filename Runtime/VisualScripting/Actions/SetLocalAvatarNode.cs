using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Local Actor: Set Avatar")]
    [UnitSurtitle("Local Actor")]
    [UnitShortTitle("Set Avatar")]
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

    [UnitTitle("Local Actor: Reset Avatar")]
    [UnitSurtitle("Local Actor")]
    [UnitShortTitle("Reset Avatar")]
    [UnitCategory("Spatial\\Actions")]
    [TypeIcon(typeof(SpatialAvatar))]
    public class ResetLocalAvatarNode : Unit
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
                ClientBridge.ResetLocalAvatar?.Invoke();
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }
    }
}
