using System;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial UI: Close All Core GUI")]
    [UnitSurtitle("Spatial UI")]
    [UnitShortTitle("Close All Core GUI")]
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
                SpatialBridge.coreGUIService.CloseAllCoreGUI();
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

#pragma warning disable CS0618 // Type or member is obsolete
    [UnitTitle("Spatial UI: Set Core GUI Enabled (Obsolete)")]
    [UnitSurtitle("Spatial UI")]
    [UnitShortTitle("Set Core GUI Enabled (Obsolete)")]
    [UnitCategory("Spatial\\UI")]
    [TypeIcon(typeof(SpatialComponentBase))]
    [Obsolete("Use 'Set Core GUI Enabled' instead")]
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

                // Convert obsolete SpatialSystemGUIType to SpatialCoreGUITypeFlags
                SpatialSystemGUIType guiFlags = f.GetValue<SpatialSystemGUIType>(guiType);
                if (guiFlags.HasFlag(SpatialSystemGUIType.QuestSystem))
                    SpatialBridge.coreGUIService.SetCoreGUIEnabled(SpatialCoreGUITypeFlags.QuestSystem, isEnabled);

                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }
#pragma warning restore CS0618 // Type or member is obsolete

    [UnitTitle("Spatial UI: Set Core GUI Open")]
    [UnitSurtitle("Spatial UI")]
    [UnitShortTitle("Set Core GUI Open")]
    [UnitCategory("Spatial\\UI")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class SetCoreGUIOpenNode : Unit
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
        public ValueInput open { get; private set; }

        protected override void Definition()
        {
            guiType = ValueInput<SpatialCoreGUITypeFlags>(nameof(guiType), 0);
            open = ValueInput<bool>(nameof(open), false);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                SpatialCoreGUITypeFlags flags = f.GetValue<SpatialCoreGUITypeFlags>(guiType);
                SpatialBridge.coreGUIService.SetCoreGUIEnabled(flags, f.GetValue<bool>(open));
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitTitle("Spatial UI: Set Core GUI Enabled")]
    [UnitSurtitle("Spatial UI")]
    [UnitShortTitle("Set Core GUI Enabled")]
    [UnitCategory("Spatial\\UI")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class SetCoreGUIEnabledNode : Unit
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
            guiType = ValueInput<SpatialCoreGUITypeFlags>(nameof(guiType), 0);
            enabled = ValueInput<bool>(nameof(enabled), false);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                SpatialCoreGUITypeFlags flags = f.GetValue<SpatialCoreGUITypeFlags>(guiType);
                SpatialBridge.coreGUIService.SetCoreGUIEnabled(flags, f.GetValue<bool>(enabled));
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }
}