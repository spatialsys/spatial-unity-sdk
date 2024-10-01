using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial: Teleport To Space")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Teleport To Space")]
    [UnitCategory("Spatial\\System")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class TeleportToSpaceNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput spaceID { get; private set; }
        public ValueInput showPopup { get; private set; }

        protected override void Definition()
        {
            spaceID = ValueInput<string>(nameof(spaceID), "");
            showPopup = ValueInput<bool>(nameof(showPopup), true);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                SpatialBridge.spaceService.TeleportToSpace(f.GetValue<string>(spaceID), f.GetValue<bool>(showPopup));
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }
    }
}
