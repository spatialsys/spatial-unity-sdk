using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial Synced Object: Takeover Ownership")]
    [UnitSurtitle("Spatial Synced Object")]
    [UnitShortTitle("Takeover Ownership")]
    [UnitCategory("Spatial\\Synced Object")]
    [TypeIcon(typeof(SpatialSyncedObject))]
    public class TakeoverSyncedObjectOwnershipNode : Unit
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
        public ValueInput syncedObject { get; private set; }

        protected override void Definition()
        {
            syncedObject = ValueInput<SpatialSyncedObject>(nameof(syncedObject), null).NullMeansSelf();

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                SpatialBridge.spatialComponentService.TakeoverSyncedObjectOwnership(f.GetValue<SpatialSyncedObject>(syncedObject));
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitTitle("Spatial Synced Object: If Owned Locally")]
    [UnitSurtitle("Spatial Synced Object")]
    [UnitShortTitle("If Owned Locally")]
    [UnitCategory("Spatial\\Synced Object")]
    [TypeIcon(typeof(SpatialSyncedObject))]
    public class IfSyncedObjectIsOwnedLocallyNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabel("True")]
        public ControlOutput outputTriggerTrue { get; private set; }
        [DoNotSerialize]
        [PortLabel("False")]
        public ControlOutput outputTriggerFalse { get; private set; }

        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput syncedObject { get; private set; }

        protected override void Definition()
        {
            syncedObject = ValueInput<SpatialSyncedObject>(nameof(syncedObject), null).NullMeansSelf();

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                if (SpatialBridge.spatialComponentService.GetSyncedObjectIsLocallyOwned(f.GetValue<SpatialSyncedObject>(syncedObject)))
                {
                    return outputTriggerTrue;
                }
                return outputTriggerFalse;
            });

            outputTriggerTrue = ControlOutput(nameof(outputTriggerTrue));
            outputTriggerFalse = ControlOutput(nameof(outputTriggerFalse));

            Succession(inputTrigger, outputTriggerTrue);
            Succession(inputTrigger, outputTriggerFalse);
        }
    }
}
