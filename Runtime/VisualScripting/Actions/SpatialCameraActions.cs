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
}
