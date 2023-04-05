using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial Local Avatar: Sit Down")]
    [UnitSurtitle("Spatial Local Avatar")]
    [UnitShortTitle("Sit Down")]
    [UnitCategory("Spatial\\Actions")]
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
                ClientBridge.SendLocalAvatarToSeat.Invoke(f.GetValue<Transform>(seat));
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitTitle("Spatial Local Avatar: Stand Up")]
    [UnitSurtitle("Spatial Local Avatar")]
    [UnitShortTitle("Stand Up")]
    [UnitCategory("Spatial\\Actions")]
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
                ClientBridge.SetLocalAvatarToStand.Invoke();
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }
    }
}
