using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using SpatialSys.UnitySDK;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial UI: Minimize System GUI")]
    [UnitSurtitle("Spatial UI")]
    [UnitShortTitle("Minimize System GUI")]
    [UnitCategory("Spatial\\UI")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class MinimizeSpatialSystemUI : Unit
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
                ClientBridge.MinimizeSystemUI?.Invoke();
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitTitle("Spatial UI: Set System GUI Enabled")]
    [UnitSurtitle("Spatial UI")]
    [UnitShortTitle("Set System GUI Enabled")]
    [UnitCategory("Spatial\\UI")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class SetSystemGUIEnabled : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput guiType { get; private set; }

        [DoNotSerialize]
        public ValueInput enabled { get; private set; }

        protected override void Definition()
        {
            guiType = ValueInput<SpatialSystemGUIType>(nameof(guiType), 0);
            enabled = ValueInput<bool>(nameof(enabled), false);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                bool isEnabled = f.GetValue<bool>(enabled);
                SpatialSystemGUIType guiFlags = f.GetValue<SpatialSystemGUIType>(guiType);
                ClientBridge.SetSystemGUIEnabled?.Invoke(guiFlags, isEnabled);
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }
}