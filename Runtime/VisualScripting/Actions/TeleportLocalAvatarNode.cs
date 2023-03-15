using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial: Teleport Local Avatar")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Teleport Local Avatar")]
    [UnitCategory("Spatial\\Actions")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class TeleportAvatarSelfNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput newPosition { get; private set; }

        protected override void Definition()
        {
            newPosition = ValueInput<Vector3>(nameof(newPosition), Vector3.zero);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                ClientBridge.SetLocalAvatarPosition.Invoke(f.GetValue<Vector3>(newPosition));
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }
    }
}
