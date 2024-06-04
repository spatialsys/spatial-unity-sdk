using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial Synced Animator: Set Parameter")]
    [UnitSurtitle("Spatial Synced Animator")]
    [UnitShortTitle("Set Parameter")]
    [UnitCategory("Spatial\\Synced Object")]
    [TypeIcon(typeof(SpatialSyncedAnimator))]
    public class SyncedAnimatorSetParameterNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput syncedAnimator { get; private set; }

        [DoNotSerialize]
        public ValueInput parameter { get; private set; }
        [DoNotSerialize]
        public ValueInput value { get; private set; }

        protected override void Definition()
        {
            syncedAnimator = ValueInput<SpatialSyncedAnimator>(nameof(syncedAnimator), null).NullMeansSelf();
            parameter = ValueInput<string>(nameof(parameter), "");
            value = ValueInput<object>(nameof(value), null);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                SpatialBridge.spatialComponentService.SetSyncedAnimatorParameter(f.GetValue<SpatialSyncedAnimator>(syncedAnimator), f.GetValue<string>(parameter), f.GetValue<object>(value));
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }
    }
    [UnitTitle("Spatial Synced Animator: Set Trigger")]
    [UnitSurtitle("Spatial Synced Animator")]
    [UnitShortTitle("Set Trigger")]
    [UnitCategory("Spatial\\Synced Object")]
    [TypeIcon(typeof(SpatialSyncedAnimator))]
    public class SyncedAnimatorSetTriggerNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput syncedAnimator { get; private set; }

        [DoNotSerialize]
        public ValueInput trigger { get; private set; }

        protected override void Definition()
        {
            syncedAnimator = ValueInput<SpatialSyncedAnimator>(nameof(syncedAnimator), null).NullMeansSelf();
            trigger = ValueInput<string>(nameof(trigger), "");

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                SpatialBridge.spatialComponentService.SetSyncedAnimatorTrigger(f.GetValue<SpatialSyncedAnimator>(syncedAnimator), f.GetValue<string>(trigger));
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }
    }
}
