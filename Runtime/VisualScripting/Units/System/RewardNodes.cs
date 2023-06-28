using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial: Reward Badge")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Reward Badge")]
    [UnitCategory("Spatial\\System")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class RewardBadgeNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput badgeID { get; private set; }

        protected override void Definition()
        {
            badgeID = ValueInput<string>(nameof(badgeID), "");

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                var id = f.GetValue<string>(badgeID);
                if (!string.IsNullOrEmpty(id))
                    ClientBridge.RewardBadge.Invoke(id);
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }
}
