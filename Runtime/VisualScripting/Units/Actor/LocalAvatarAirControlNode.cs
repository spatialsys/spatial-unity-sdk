using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitCategory("Spatial\\Actor")]
    [UnitTitle("Local Actor: Set Air Control")]
    [UnitSurtitle("Local Actor Control Settings")]
    [UnitShortTitle("Set Air Control")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class SetLocalAvatarAirControlNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        [PortLabel("Air Control")]
        public ValueInput airControl { get; private set; }

        protected override void Definition()
        {
            airControl = ValueInput<float>(nameof(airControl), 1); // This default should be matched with AvatarController
            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                SpatialBridge.actorService.localActor.avatar.airControl = f.GetValue<float>(airControl);
                return outputTrigger;
            });
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitCategory("Spatial\\Actor")]
    [UnitTitle("Local Actor: Get Air Control")]
    [UnitSurtitle("Local Actor Control Settings")]
    [UnitShortTitle("Get Air Control")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetLocalAvatarAirControlNode : Unit
    {
        [DoNotSerialize]
        public ValueOutput airControl { get; private set; }

        protected override void Definition()
        {
            airControl = ValueOutput<float>(nameof(airControl), (f) => SpatialBridge.actorService.localActor.avatar.airControl);
        }
    }
}
