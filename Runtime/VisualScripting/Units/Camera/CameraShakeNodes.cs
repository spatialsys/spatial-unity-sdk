using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
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
                SpatialBridge.cameraService.shakeAmplitude = f.GetValue<float>(shakeAmplitude);
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
                SpatialBridge.cameraService.shakeFrequency = f.GetValue<float>(shakeFrequency);
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
                SpatialBridge.cameraService.wobbleAmplitude = f.GetValue<float>(wobbleAmplitude);
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
                SpatialBridge.cameraService.wobbleFrequency = f.GetValue<float>(wobbleFrequency);
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
                SpatialBridge.cameraService.Shake(f.GetValue<float>(force));
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
                SpatialBridge.cameraService.Shake(f.GetValue<Vector3>(velocity));
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
                SpatialBridge.cameraService.Wobble(f.GetValue<float>(force));
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
                SpatialBridge.cameraService.Wobble(f.GetValue<Vector3>(velocity));
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
                SpatialBridge.cameraService.Kick(f.GetValue<Vector2>(degrees));
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
                SpatialBridge.cameraService.kickDecay = f.GetValue<float>(decaySpeed);
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }
}
