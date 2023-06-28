using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitCategory("Spatial\\Actor")]
    [UnitTitle("Local Actor: Set Walk Speed")]

    [UnitSurtitle("Local Actor Control Settings")]
    [UnitShortTitle("Set Walk Speed (m/s)")]

    [TypeIcon(typeof(SpatialComponentBase))]
    public class SetLocalAvatarMovingSpeedNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        [PortLabel("Walk Speed")]
        public ValueInput speed { get; private set; }

        protected override void Definition()
        {
            speed = ValueInput<float>(nameof(speed), 3.0f); // This default should be matched with AvatarController movingSpeed.
            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                ClientBridge.SetLocalAvatarMovingSpeed?.Invoke(f.GetValue<float>(speed));
                return outputTrigger;
            });
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitCategory("Spatial\\Actor")]
    [UnitTitle("Local Actor: Get Walk Speed")]

    [UnitSurtitle("Local Actor Control Settings")]
    [UnitShortTitle("Get Walk Speed (m/s)")]

    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetLocalAvatarMovingSpeedNode : Unit
    {
        [DoNotSerialize]
        [PortLabel("Walk Speed")]
        public ValueOutput speed { get; private set; }

        protected override void Definition()
        {
            speed = ValueOutput<float>(nameof(speed), (f) => ClientBridge.GetLocalAvatarMovingSpeed?.Invoke() ?? 3.0f);
        }
    }
}
