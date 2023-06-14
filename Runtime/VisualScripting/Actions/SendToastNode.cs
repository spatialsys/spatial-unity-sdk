using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial: Send Toast")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Send Toast")]
    [UnitCategory("Spatial\\Actions")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class SendToastNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput message { get; private set; }
        [DoNotSerialize]
        public ValueInput duration { get; private set; }

        protected override void Definition()
        {
            message = ValueInput<string>(nameof(message), "");
            duration = ValueInput<float>(nameof(duration), 1f);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                ClientBridge.SendToast?.Invoke(f.GetValue<string>(message), f.GetValue<float>(duration));
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }
}
