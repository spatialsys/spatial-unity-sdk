using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial: Open URL")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Open URL")]
    [UnitCategory("Spatial\\System")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class OpenURLNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput url { get; private set; }

        protected override void Definition()
        {
            url = ValueInput<string>(nameof(url), "https://");

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                ClientBridge.OpenURL?.Invoke(f.GetValue<string>(url));
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }
    }
}
