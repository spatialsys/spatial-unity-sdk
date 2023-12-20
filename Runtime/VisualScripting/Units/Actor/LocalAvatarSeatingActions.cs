using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Local Actor: Sit Down")]
    [UnitSurtitle("Local Actor")]
    [UnitShortTitle("Sit Down")]
    [UnitCategory("Spatial\\Actor")]
    [TypeIcon(typeof(SpatialSeatHotspot))]
    public class SendLocalAvatarToSeatNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        [NullMeansSelf]
        public ValueInput seat { get; private set; }

        protected override void Definition()
        {
            seat = ValueInput<Transform>(nameof(seat), null).NullMeansSelf();

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                SpatialBridge.actorService.localActor.avatar.Sit(f.GetValue<Transform>(seat));
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitTitle("Local Actor: Stand Up")]
    [UnitSurtitle("Local Actor")]
    [UnitShortTitle("Stand Up")]
    [UnitCategory("Spatial\\Actor")]
    [TypeIcon(typeof(SpatialSeatHotspot))]
    public class SetLocalAvatarToStand : Unit
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
                SpatialBridge.actorService.localActor.avatar.Stand();
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }
    }
}
