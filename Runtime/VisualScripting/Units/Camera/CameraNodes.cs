using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial Camera State")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Camera State")]
    [UnitCategory("Spatial\\Camera")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetCameraStateNode : Unit
    {
        [DoNotSerialize]
        [PortLabel("Position")]
        public ValueOutput cameraPosition { get; private set; }
        [DoNotSerialize]
        [PortLabel("Rotation")]
        public ValueOutput cameraRotation { get; private set; }
        [DoNotSerialize]
        [PortLabel("Forward")]
        public ValueOutput cameraForward { get; private set; }

        protected override void Definition()
        {
            cameraPosition = ValueOutput<Vector3>(nameof(cameraPosition), (f) => SpatialBridge.cameraService.position);
            cameraRotation = ValueOutput<Quaternion>(nameof(cameraRotation), (f) => SpatialBridge.cameraService.rotation);
            cameraForward = ValueOutput<Vector3>(nameof(cameraForward), (f) => SpatialBridge.cameraService.forward);
        }
    }

    [UnitTitle("Player Camera: Set Third Person Offset")]
    [UnitSurtitle("Player Camera")]
    [UnitShortTitle("Set Third Person Offset")]
    [UnitCategory("Spatial\\Camera")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class SetPlayerCameraThirdPersonOffsetNode : Unit
    {
        [DoNotSerialize, PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize, PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput offset { get; private set; }

        protected override void Definition()
        {
            offset = ValueInput<Vector3>(nameof(offset), new Vector3(0f, 0f, 0f));

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                SpatialBridge.cameraService.thirdPersonOffset = f.GetValue<Vector3>(offset);
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitTitle("Player Camera: Set Third Person FOV")]
    [UnitSurtitle("Player Camera")]
    [UnitShortTitle("Set Third Person FOV")]
    [UnitCategory("Spatial\\Camera")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class SetPlayerCameraThirdPersonFOVNode : Unit
    {
        [DoNotSerialize, PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize, PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput fov { get; private set; }

        protected override void Definition()
        {
            fov = ValueInput<float>(nameof(fov), 70f);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                SpatialBridge.cameraService.thirdPersonFov = f.GetValue<float>(fov);
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitTitle("Player Camera: Set First Person FOV")]
    [UnitSurtitle("Player Player Camera")]
    [UnitShortTitle("Set First Person FOV")]
    [UnitCategory("Spatial\\Camera")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class SetPlayerCameraFirstPersonFOVNode : Unit
    {
        [DoNotSerialize, PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize, PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput fov { get; private set; }

        protected override void Definition()
        {
            fov = ValueInput<float>(nameof(fov), 80f);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                SpatialBridge.cameraService.firstPersonFov = f.GetValue<float>(fov);
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitTitle("Player Camera: Set Force First Person")]
    [UnitSurtitle("Player Camera")]
    [UnitShortTitle("Set Force First Person")]
    [UnitCategory("Spatial\\Camera")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class SetPlayerCameraForceFirstPersonNode : Unit
    {
        [DoNotSerialize, PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize, PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput forceFirstPerson { get; private set; }

        protected override void Definition()
        {
            forceFirstPerson = ValueInput<bool>(nameof(forceFirstPerson), false);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                SpatialBridge.cameraService.forceFirstPerson = f.GetValue<bool>(forceFirstPerson);
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitTitle("Player Camera: Lock Player Camera Rotation")]
    [UnitSurtitle("Player Camera")]
    [UnitShortTitle("Set Lock Camera Rotation")]
    [UnitCategory("Spatial\\Camera")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class SetPlayerCameraLockCameraRotationNode : Unit
    {
        [DoNotSerialize, PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize, PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput lockCameraRotation { get; private set; }

        protected override void Definition()
        {
            lockCameraRotation = ValueInput<bool>(nameof(lockCameraRotation), false);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                SpatialBridge.cameraService.lockCameraRotation = f.GetValue<bool>(lockCameraRotation);
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitTitle("Player Camera: Set Zoom Distance")]
    [UnitSurtitle("Player Camera")]
    [UnitShortTitle("Set Zoom Distance")]
    [UnitCategory("Spatial\\Camera")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class SetPlayerCameraZoomDistanceNode : Unit
    {
        [DoNotSerialize, PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize, PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput zoomDistance { get; private set; }

        protected override void Definition()
        {
            zoomDistance = ValueInput<float>(nameof(zoomDistance), 6f);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                SpatialBridge.cameraService.zoomDistance = f.GetValue<float>(zoomDistance);
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitTitle("Player Camera: Set Min Zoom Distance")]
    [UnitSurtitle("Player Camera")]
    [UnitShortTitle("Set Min Zoom Distance")]
    [UnitCategory("Spatial\\Camera")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class SetPlayerCameraMinZoomDistanceNode : Unit
    {
        [DoNotSerialize, PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize, PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput minZoomDistance { get; private set; }

        protected override void Definition()
        {
            minZoomDistance = ValueInput<float>(nameof(minZoomDistance), .05f);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                SpatialBridge.cameraService.minZoomDistance = f.GetValue<float>(minZoomDistance);
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitTitle("Player Camera: Set Max Zoom Distance")]
    [UnitSurtitle("Player Camera")]
    [UnitShortTitle("Set Max Zoom Distance")]
    [UnitCategory("Spatial\\Camera")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class SetPlayerCameraMaxZoomDistanceNode : Unit
    {
        [DoNotSerialize, PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize, PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput maxZoomDistance { get; private set; }

        protected override void Definition()
        {
            maxZoomDistance = ValueInput<float>(nameof(maxZoomDistance), 10f);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                SpatialBridge.cameraService.maxZoomDistance = f.GetValue<float>(maxZoomDistance);
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitCategory("Spatial\\Camera")]
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
                SpatialBridge.cameraService.rotationMode = f.GetValue<SpatialCameraRotationMode>(cameraRotationMode);
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitCategory("Spatial\\Camera")]
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
            cameraRotationMode = ValueOutput<SpatialCameraRotationMode>(nameof(cameraRotationMode), (f) => SpatialBridge.cameraService.rotationMode);
        }
    }
}
