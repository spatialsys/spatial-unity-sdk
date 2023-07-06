using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using SpatialSys.UnitySDK;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial UI: Create Floating Text (Random Force)")]
    [UnitSurtitle("Spatial UI")]
    [UnitShortTitle("Create Floating Text")]
    [UnitCategory("Spatial\\UI")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class CreateFloatingTextFloatForceNode : Unit
    {
        [SerializeAs(nameof(animStyle))]
        private FloatingTextAnimStyle _animStyle;
        [DoNotSerialize]
        [Inspectable, UnitHeaderInspectable("Style")]
        public FloatingTextAnimStyle animStyle { get => _animStyle; set => _animStyle = value; }

        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput text { get; private set; }
        [DoNotSerialize]
        public ValueInput style { get; private set; }
        [DoNotSerialize]
        [PortLabel("Position")]
        public ValueInput textPosition { get; private set; }
        [DoNotSerialize]
        public ValueInput color { get; private set; }


        [DoNotSerialize]
        public ValueInput force { get; private set; }
        [DoNotSerialize]
        public ValueInput gravity { get; private set; }

        [DoNotSerialize]
        public ValueInput lifetime { get; private set; }
        [DoNotSerialize]
        public ValueInput scaleCurve { get; private set; }
        [DoNotSerialize]
        public ValueInput alphaCurve { get; private set; }

        protected override void Definition()
        {
            text = ValueInput<string>(nameof(text), "");
            textPosition = ValueInput<Vector3>(nameof(textPosition), Vector3.zero);
            color = ValueInput<Color>(nameof(color), Color.white);
            force = ValueInput<float>(nameof(force), 0f);
            gravity = ValueInput<bool>(nameof(gravity), false);

            if (animStyle == FloatingTextAnimStyle.Custom)
            {
                scaleCurve = ValueInput<AnimationCurve>(nameof(scaleCurve), new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1)));
                alphaCurve = ValueInput<AnimationCurve>(nameof(alphaCurve), new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1)));
                lifetime = ValueInput<float>(nameof(lifetime), 1f);
            }


            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {

                float forceValue = f.GetValue<float>(force);
                Vector3 randomForce = new Vector3(Random.Range(-forceValue, forceValue), Random.Range(-forceValue, forceValue), Random.Range(-forceValue, forceValue));
                bool isCustom = animStyle == FloatingTextAnimStyle.Custom;

                ClientBridge.CreateFloatingText?.Invoke(
                    f.GetValue<string>(text),
                    animStyle,
                    f.GetValue<Vector3>(textPosition),
                    randomForce,
                    f.GetValue<Color>(color),
                    f.GetValue<bool>(gravity),
                    isCustom ? f.GetValue<AnimationCurve>(scaleCurve) : null,
                    isCustom ? f.GetValue<AnimationCurve>(alphaCurve) : null,
                    isCustom ? f.GetValue<float>(lifetime) : 1f//non-custom lifetime is ignored.
                );
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitTitle("Spatial UI: Create Floating Text (Vector3 Force)")]
    [UnitSurtitle("Spatial UI")]
    [UnitShortTitle("Create Floating Text")]
    [UnitCategory("Spatial\\UI")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class CreateFloatingTextVector3ForceNode : Unit
    {
        [SerializeAs(nameof(animStyle))]
        private FloatingTextAnimStyle _animStyle;
        [DoNotSerialize]
        [Inspectable, UnitHeaderInspectable("Style")]
        public FloatingTextAnimStyle animStyle { get => _animStyle; set => _animStyle = value; }

        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput text { get; private set; }
        [DoNotSerialize]
        [PortLabel("Position")]
        public ValueInput textPosition { get; private set; }
        [DoNotSerialize]
        public ValueInput color { get; private set; }
        [DoNotSerialize]
        public ValueInput force { get; private set; }
        [DoNotSerialize]
        public ValueInput gravity { get; private set; }

        [DoNotSerialize]
        public ValueInput lifetime { get; private set; }
        [DoNotSerialize]
        public ValueInput scaleCurve { get; private set; }
        [DoNotSerialize]
        public ValueInput alphaCurve { get; private set; }

        protected override void Definition()
        {
            text = ValueInput<string>(nameof(text), "");
            textPosition = ValueInput<Vector3>(nameof(textPosition), Vector3.zero);
            color = ValueInput<Color>(nameof(color), Color.white);
            force = ValueInput<Vector3>(nameof(force), Vector3.zero);
            gravity = ValueInput<bool>(nameof(gravity), false);

            if (animStyle == FloatingTextAnimStyle.Custom)
            {
                scaleCurve = ValueInput<AnimationCurve>(nameof(scaleCurve), new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1)));
                alphaCurve = ValueInput<AnimationCurve>(nameof(alphaCurve), new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1)));
                lifetime = ValueInput<float>(nameof(lifetime), 1f);
            }

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {

                bool isCustom = animStyle == FloatingTextAnimStyle.Custom;

                ClientBridge.CreateFloatingText?.Invoke(
                    f.GetValue<string>(text),
                    animStyle,
                    f.GetValue<Vector3>(textPosition),
                    f.GetValue<Vector3>(force),
                    f.GetValue<Color>(color),
                    f.GetValue<bool>(gravity),
                    isCustom ? f.GetValue<AnimationCurve>(scaleCurve) : null,
                    isCustom ? f.GetValue<AnimationCurve>(alphaCurve) : null,
                    isCustom ? f.GetValue<float>(lifetime) : 1f//non-custom lifetime is ignored.
                );
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }
}
