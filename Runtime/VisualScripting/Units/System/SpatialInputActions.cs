using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using SpatialSys.UnitySDK.Internal;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial Input: Set Overriden Avatar Input Capture")]
    [UnitSurtitle("Spatial Input")]
    [UnitShortTitle("Set Overriden Avatar Input Capture")]
    [UnitCategory("Spatial\\System")]
    [TypeIcon(typeof(InputIcon))]
    public class SetInputOverrides : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput movement { get; private set; }
        [DoNotSerialize]
        public ValueInput jump { get; private set; }
        [DoNotSerialize]
        public ValueInput sprint { get; private set; }
        [DoNotSerialize]
        public ValueInput actionButton { get; private set; }

        protected override void Definition()
        {
            movement = ValueInput<bool>(nameof(movement), true);
            jump = ValueInput<bool>(nameof(jump), true);
            sprint = ValueInput<bool>(nameof(sprint), true);
            actionButton = ValueInput<bool>(nameof(actionButton), true);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                SpatialBridge.SetInputOverrides.Invoke(
                    f.GetValue<bool>(movement),
                    f.GetValue<bool>(jump),
                    f.GetValue<bool>(sprint),
                    f.GetValue<bool>(actionButton),
                    f.stack.self
                );
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }

        public override void Uninstantiate(GraphReference instance)
        {
            base.Uninstantiate(instance);
            if (instance?.gameObject != null)
                SpatialBridge.OnInputGraphRootObjectDestroyed?.Invoke(instance.gameObject);
        }
    }

    [UnitTitle("Spatial Input: Start Vehicle Input Capture")]
    [UnitSurtitle("Spatial Input")]
    [UnitShortTitle("Start Vehicle Input Capture")]
    [UnitCategory("Spatial\\System")]
    [TypeIcon(typeof(InputIcon))]
    public class StartVehicleInputCapture : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput flags { get; private set; }

        [DoNotSerialize]
        public ValueInput primaryButtonSprite { get; private set; }

        [DoNotSerialize]
        public ValueInput secondaryButtonSprite { get; private set; }

        protected override void Definition()
        {
            flags = ValueInput<VehicleInputFlags>(nameof(flags), VehicleInputFlags.Steer1D | VehicleInputFlags.Throttle | VehicleInputFlags.Reverse | VehicleInputFlags.Exit);
            primaryButtonSprite = ValueInput<Sprite>(nameof(primaryButtonSprite), null);
            secondaryButtonSprite = ValueInput<Sprite>(nameof(secondaryButtonSprite), null);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                SpatialBridge.StartVehicleInputCapture.Invoke(
                    f.GetValue<VehicleInputFlags>(flags),
                    f.GetValue<Sprite>(primaryButtonSprite),
                    f.GetValue<Sprite>(secondaryButtonSprite),
                    f.stack.self
                );
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }

        public override void Uninstantiate(GraphReference instance)
        {
            base.Uninstantiate(instance);
            if (instance?.gameObject != null)
                SpatialBridge.OnInputGraphRootObjectDestroyed?.Invoke(instance.gameObject);
        }
    }

    [UnitTitle("Spatial Input: Start Custom Player Input Capture")]
    [UnitSubtitle("Disables default Spatial player input\nIncluding camera control and mobile on-screen controls")]
    [UnitSurtitle("Spatial Input")]
    [UnitShortTitle("Start Custom Player Input Capture")]
    [UnitCategory("Spatial\\System")]
    [TypeIcon(typeof(InputIcon))]
    public class StartCustomInputCapture : Unit
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
                SpatialBridge.StartCompleteCustomInputCapture.Invoke(
                    f.stack.self
                );
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }

        public override void Uninstantiate(GraphReference instance)
        {
            base.Uninstantiate(instance);
            if (instance?.gameObject != null)
                SpatialBridge.OnInputGraphRootObjectDestroyed?.Invoke(instance.gameObject);
        }
    }

    [UnitTitle("Spatial Input: Release Input Capturer")]
    [UnitSurtitle("Spatial Input")]
    [UnitShortTitle("Release Input Capture")]
    [UnitCategory("Spatial\\System")]
    [TypeIcon(typeof(InputIcon))]
    public class ReleaseInputCapture : Unit
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
                SpatialBridge.ReleaseInputCapture.Invoke(
                    f.stack.self
                );
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }
    }
}