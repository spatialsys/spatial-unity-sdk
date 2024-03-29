using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitCategory("Spatial\\Actor")]
    [UnitTitle("Local Actor: Set Run Speed")]
    [UnitSurtitle("Local Actor Control Settings")]
    [UnitShortTitle("Set Run Speed (m/s)")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class SetLocalAvatarRunSpeedNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        [PortLabel("Run Speed")]
        public ValueInput speed { get; private set; }

        protected override void Definition()
        {
            speed = ValueInput<float>(nameof(speed), 6.875f); // This default should be matched with AvatarController RunSpeed.
            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                SpatialBridge.actorService.localActor.avatar.runSpeed = f.GetValue<float>(speed);
                return outputTrigger;
            });
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitCategory("Spatial\\Actor")]
    [UnitTitle("Local Actor: Get Run Speed")]
    [UnitSurtitle("Local Actor Control Settings")]
    [UnitShortTitle("Get Run Speed (m/s)")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetLocalAvatarRunSpeedNode : Unit
    {
        [DoNotSerialize]
        [PortLabel("Run Speed")]
        public ValueOutput speed { get; private set; }

        protected override void Definition()
        {
            speed = ValueOutput<float>(nameof(speed), (f) => SpatialBridge.actorService.localActor.avatar.runSpeed);
        }
    }
}
