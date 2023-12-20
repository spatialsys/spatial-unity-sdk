using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitCategory("Spatial\\Actor")]
    [UnitTitle("Local Actor: Set Nametag Subtext")]
    [UnitSurtitle("Local Actor Nametag")]
    [UnitShortTitle("Set Subtext")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class LocalActorSetNametagSubtextNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput subtext { get; private set; }

        protected override void Definition()
        {
            subtext = ValueInput<string>(nameof(subtext), string.Empty);
            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                SpatialBridge.actorService.localActor.avatar.nametagSubtext = f.GetValue<string>(subtext);
                return outputTrigger;
            });
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitCategory("Spatial\\Actor")]
    [UnitTitle("Local Actor: Set Nametag Bar Visible")]
    [UnitSurtitle("Local Actor Nametag")]
    [UnitShortTitle("Set Bar Visible")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class LocalActorSetNametagBarVisibleNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput visible { get; private set; }

        protected override void Definition()
        {
            visible = ValueInput<bool>(nameof(visible), false);
            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                SpatialBridge.actorService.localActor.avatar.nametagBarVisible = f.GetValue<bool>(visible);
                return outputTrigger;
            });
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitCategory("Spatial\\Actor")]
    [UnitTitle("Local Actor: Set Nametag Bar Value")]
    [UnitSurtitle("Local Actor Nametag")]
    [UnitShortTitle("Set Bar Value")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class LocalActorSetNametagBarValueNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput value { get; private set; }

        protected override void Definition()
        {
            value = ValueInput<float>(nameof(value), 0f);
            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                SpatialBridge.actorService.localActor.avatar.nametagBarValue = f.GetValue<float>(value);
                return outputTrigger;
            });
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }
}
