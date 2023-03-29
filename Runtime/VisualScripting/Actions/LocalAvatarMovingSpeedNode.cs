using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitCategory("Spatial\\Actions")]
    [UnitTitle("Local Actor: Set Moving Speed")]

    [UnitSurtitle("Local Actor Moving Speed")]
    [UnitShortTitle("Set Speed (m/s)")]

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
        public ValueInput speed { get; private set; }

        protected override void Definition()
        {
            speed = ValueInput<float>(nameof(speed), 3.0f); // This default should be matched with AvatarController movingSpeed.
            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                ClientBridge.SetLocalAvatarMovingSpeed.Invoke(f.GetValue<float>(speed));
                return outputTrigger;
            });
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitCategory("Spatial\\Get Actions")]
    [UnitTitle("Local Actor: Get Moving Speed")]

    [UnitSurtitle("Local Actor Moving Speed")]
    [UnitShortTitle("Get Speed (m/s)")]

    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetLocalAvatarMovingSpeedNode : Unit
    {
        [DoNotSerialize]
        [PortLabel("Speed")]
        public ValueOutput speed { get; private set; }

        protected override void Definition()
        {
            speed = ValueOutput<float>(nameof(speed), (f) => ClientBridge.GetLocalAvatarMovingSpeed.Invoke());
        }
    }
}
