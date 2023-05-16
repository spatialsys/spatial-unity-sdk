using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial Camera: Set Target Override")]
    [UnitSurtitle("Spatial Camera")]
    [UnitShortTitle("Set Target Override")]
    [UnitCategory("Spatial\\Actions")]
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
                ClientBridge.SetCameraTargetOverride.Invoke(f.GetValue<Transform>(target), f.GetValue<SpatialCameraMode>(cameraMode));
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitTitle("Spatial Camera: Clear Target Override")]
    [UnitSurtitle("Spatial Camera")]
    [UnitShortTitle("Clear Target Override")]
    [UnitCategory("Spatial\\Actions")]
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
                ClientBridge.ClearCameraTargetOverride.Invoke();
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitCategory("Spatial\\Actions")]
    [UnitTitle("Spatial Camera: Set Rotation Mode")]

    [UnitSurtitle("Spatial Camera")]
    [UnitShortTitle("Set Rotation Mode")]

    [TypeIcon(typeof(SpatialComponentBase))]
    public class SetCameraRotationModeNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        [PortLabel("Rotation Mode")]
        public ValueInput cameraRotationMode { get; private set; }

        protected override void Definition()
        {
            cameraRotationMode = ValueInput<SpatialCameraRotationMode>(nameof(cameraRotationMode), SpatialCameraRotationMode.AutoRotate);
            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                ClientBridge.SetCameraRotationMode.Invoke(f.GetValue<SpatialCameraRotationMode>(cameraRotationMode));
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitCategory("Spatial\\Get Actions")]
    [UnitTitle("Spatial Camera: Get Rotation Mode")]

    [UnitSurtitle("Spatial Camera")]
    [UnitShortTitle("Get Rotation Mode")]

    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetCameraRotationModeNode : Unit
    {
        [DoNotSerialize]
        [PortLabel("Rotation Mode")]
        public ValueOutput cameraRotationMode { get; private set; }

        protected override void Definition()
        {
            cameraRotationMode = ValueOutput<SpatialCameraRotationMode>(nameof(cameraRotationMode), (f) => (SpatialCameraRotationMode)ClientBridge.GetCameraRotationMode.Invoke());
        }
    }
}
