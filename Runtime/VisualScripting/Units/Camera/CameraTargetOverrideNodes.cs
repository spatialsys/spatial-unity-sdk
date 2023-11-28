using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial Camera: Get Target Override")]
    [UnitSurtitle("Spatial Camera")]
    [UnitShortTitle("Get Target Override")]
    [UnitCategory("Spatial\\Camera")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetCameraTargetOverrideNode : Unit
    {
        [DoNotSerialize]
        [PortLabel("Target")]
        public ValueOutput target { get; private set; }

        protected override void Definition()
        {
            target = ValueOutput<Transform>(nameof(target), (f) => SpatialBridge.cameraService.targetOverride);
        }
    }

    [UnitTitle("Spatial Camera: Set Target Override")]
    [UnitSurtitle("Spatial Camera")]
    [UnitShortTitle("Set Target Override")]
    [UnitCategory("Spatial\\Camera")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class SetCameraTargetOverrideNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput target { get; private set; }
        [DoNotSerialize]
        public ValueInput cameraMode { get; private set; }

        protected override void Definition()
        {
            target = ValueInput<Transform>(nameof(target));
            cameraMode = ValueInput<SpatialCameraMode>(nameof(cameraMode), SpatialCameraMode.Actor);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                SpatialBridge.cameraService.SetTargetOverride(f.GetValue<Transform>(target), f.GetValue<SpatialCameraMode>(cameraMode));
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitTitle("Spatial Camera: Clear Target Override")]
    [UnitSurtitle("Spatial Camera")]
    [UnitShortTitle("Clear Target Override")]
    [UnitCategory("Spatial\\Camera")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class ClearCameraTargetOverrideNode : Unit
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
                SpatialBridge.cameraService.ClearTargetOverride();
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }
    }
}
