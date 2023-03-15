using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial: Add Force To Local Avatar")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Add Force To Local Avatar")]
    [UnitCategory("Spatial\\Actions")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class AddForceToLocalAvatarNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput force { get; private set; }

        protected override void Definition()
        {
            force = ValueInput<Vector3>(nameof(force), Vector3.zero);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                ClientBridge.AddForceToLocalAvatar.Invoke(f.GetValue<Vector3>(force));
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }
    }
}
