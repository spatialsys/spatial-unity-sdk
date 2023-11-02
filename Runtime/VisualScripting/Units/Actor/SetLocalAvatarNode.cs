using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Local Actor: Set Avatar From Package")]
    [UnitSurtitle("Local Actor")]
    [UnitShortTitle("Set Avatar from Package")]
    [UnitCategory("Spatial\\Actor")]
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
                ClientBridge.SetLocalAvatarFromPackage?.Invoke(f.GetValue<string>(sku));
                return outputTrigger;
            });
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitTitle("Local Actor: Set Avatar From Embedded Package Asset")]
    [UnitSurtitle("Local Actor")]
    [UnitShortTitle("Set Avatar from Embedded Package Asset")]
    [UnitCategory("Spatial\\Actor")]
    [TypeIcon(typeof(SpatialAvatar))]
    public class SetLocalAvatarFromEmbeddedNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput assetID { get; private set; }

        protected override void Definition()
        {
            assetID = ValueInput<string>(nameof(assetID));

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                ClientBridge.SetLocalAvatarFromEmbedded?.Invoke(f.GetValue<string>(assetID));
                return outputTrigger;
            });
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitTitle("Local Actor: Reset Avatar")]
    [UnitSurtitle("Local Actor")]
    [UnitShortTitle("Reset Avatar")]
    [UnitCategory("Spatial\\Actor")]
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
