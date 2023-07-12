using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
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
                ClientBridge.SetPlayerCameraThirdPersonOffset?.Invoke(f.GetValue<Vector3>(offset));
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
                ClientBridge.SetPlayerCameraThirdPersonFOV?.Invoke(f.GetValue<float>(fov));
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
                ClientBridge.SetPlayerCameraFirstPersonFOV?.Invoke(f.GetValue<float>(fov));
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
                ClientBridge.SetPlayerCameraForceFirstPerson?.Invoke(f.GetValue<bool>(forceFirstPerson));
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
                ClientBridge.SetPlayerCameraLockRotation?.Invoke(f.GetValue<bool>(lockCameraRotation));
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
                ClientBridge.SetPlayerCameraZoomDistance?.Invoke(f.GetValue<float>(zoomDistance));
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
                ClientBridge.SetPlayerCameraMinZoomDistance?.Invoke(f.GetValue<float>(minZoomDistance));
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
                ClientBridge.SetPlayerCameraMaxZoomDistance?.Invoke(f.GetValue<float>(maxZoomDistance));
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitTitle("Player Camera: Set Shake Amplitude")]
    [UnitSurtitle("Player Camera")]
    [UnitShortTitle("Set Shake Amplitude")]
    [UnitCategory("Spatial\\Camera")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class SetPlayerCameraShakeAmplitudeNode : Unit
    {
        [DoNotSerialize, PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize, PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput shakeAmplitude { get; private set; }

        protected override void Definition()
        {
            shakeAmplitude = ValueInput<float>(nameof(shakeAmplitude), 1f);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                ClientBridge.SetPlayerCameraShakeAmplitude?.Invoke(f.GetValue<float>(shakeAmplitude));
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitTitle("Player Camera: Set Shake Frequency")]
    [UnitSurtitle("Player Camera")]
    [UnitShortTitle("Set Shake Frequency")]
    [UnitCategory("Spatial\\Camera")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class SetPlayerCameraShakeFrequencyNode : Unit
    {
        [DoNotSerialize, PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize, PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput shakeFrequency { get; private set; }

        protected override void Definition()
        {
            shakeFrequency = ValueInput<float>(nameof(shakeFrequency), 15f);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                ClientBridge.SetPlayerCameraShakeFrequency?.Invoke(f.GetValue<float>(shakeFrequency));
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitTitle("Player Camera: Set Wobble Amplitude")]
    [UnitSurtitle("Player Camera")]
    [UnitShortTitle("Set Wobble Amplitude")]
    [UnitCategory("Spatial\\Camera")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class SetPlayerCameraWobbleAmplitudeNode : Unit
    {
        [DoNotSerialize, PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize, PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput wobbleAmplitude { get; private set; }

        protected override void Definition()
        {
            wobbleAmplitude = ValueInput<float>(nameof(wobbleAmplitude), .7f);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                ClientBridge.SetPlayerCameraWobbleAmplitude?.Invoke(f.GetValue<float>(wobbleAmplitude));
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitTitle("Player Camera: Set Wobble Frequency")]
    [UnitSurtitle("Player Camera")]
    [UnitShortTitle("Set Wobble Frequency")]
    [UnitCategory("Spatial\\Camera")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class SetPlayerCameraWobbleFrequencyNode : Unit
    {
        [DoNotSerialize, PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize, PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput wobbleFrequency { get; private set; }

        protected override void Definition()
        {
            wobbleFrequency = ValueInput<float>(nameof(wobbleFrequency), .2f);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                ClientBridge.SetPlayerCameraWobbleFrequency?.Invoke(f.GetValue<float>(wobbleFrequency));
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitTitle("Player Camera: Shake (Force)")]
    [UnitSurtitle("Player Camera")]
    [UnitShortTitle("Shake")]
    [UnitCategory("Spatial\\Camera")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class PlayerCameraShakeForceNode : Unit
    {
        [DoNotSerialize, PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize, PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput force { get; private set; }

        protected override void Definition()
        {
            force = ValueInput<float>(nameof(force), 1f);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                ClientBridge.PlayerCameraShakeForce?.Invoke(f.GetValue<float>(force));
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitTitle("Player Camera: Shake (Velocity)")]
    [UnitSurtitle("Player Camera")]
    [UnitShortTitle("Shake")]
    [UnitCategory("Spatial\\Camera")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class PlayerCameraShakeVelocityNode : Unit
    {
        [DoNotSerialize, PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize, PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput velocity { get; private set; }

        protected override void Definition()
        {
            velocity = ValueInput<Vector3>(nameof(velocity), Vector3.zero);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                ClientBridge.PlayerCameraShakeVelocity?.Invoke(f.GetValue<Vector3>(velocity));
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitTitle("Player Camera: Wobble (Force)")]
    [UnitSurtitle("Player Camera")]
    [UnitShortTitle("Wobble")]
    [UnitCategory("Spatial\\Camera")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class PlayerCameraWobbleForceNode : Unit
    {
        [DoNotSerialize, PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize, PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput force { get; private set; }

        protected override void Definition()
        {
            force = ValueInput<float>(nameof(force), 1f);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                ClientBridge.PlayerCameraWobbleForce?.Invoke(f.GetValue<float>(force));
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitTitle("Player Camera: Wobble (Velocity)")]
    [UnitSurtitle("Player Camera")]
    [UnitShortTitle("Wobble")]
    [UnitCategory("Spatial\\Camera")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class PlayerCameraWobbleVelocityNode : Unit
    {
        [DoNotSerialize, PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize, PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput velocity { get; private set; }

        protected override void Definition()
        {
            velocity = ValueInput<Vector3>(nameof(velocity), Vector3.zero);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                ClientBridge.PlayerCameraWobbleVelocity?.Invoke(f.GetValue<Vector3>(velocity));
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitTitle("Player Camera: Kick (Degrees)")]
    [UnitSurtitle("Player Camera")]
    [UnitShortTitle("Kick")]
    [UnitCategory("Spatial\\Camera")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class PlayerCameraKickDegreesNode : Unit
    {
        [DoNotSerialize, PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize, PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput degrees { get; private set; }

        protected override void Definition()
        {
            degrees = ValueInput<Vector2>(nameof(degrees), Vector2.zero);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                ClientBridge.PlayerCameraAddKick?.Invoke(f.GetValue<Vector2>(degrees));
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitTitle("Player Camera: Set Kick Decay Speed")]
    [UnitSurtitle("Player Camera")]
    [UnitShortTitle("Set Kick Decay Speed")]
    [UnitCategory("Spatial\\Camera")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class SetPlayerCameraKickDecaySpeedNode : Unit
    {
        [DoNotSerialize, PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize, PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput decaySpeed { get; private set; }

        protected override void Definition()
        {
            decaySpeed = ValueInput<float>(nameof(decaySpeed), 200f);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                ClientBridge.SetPlayerCameraKickDecaySpeed?.Invoke(f.GetValue<float>(decaySpeed));
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }
}
