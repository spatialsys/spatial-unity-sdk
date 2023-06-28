using System;
using UnityEngine;
using Unity.VisualScripting;
using SpatialSys.UnitySDK;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial Synced Object: On Owner Changed")]
    [UnitSurtitle("Spatial Synced Object")]
    [UnitShortTitle("On Owner Changed")]
    [UnitCategory("Events\\Spatial\\Synced Object")]
    [TypeIcon(typeof(SpatialSyncedObject))]
    public class SpatialSyncedObjectEventOnOwnerChanged : EventUnit<SpatialSyncedObject>
    {
        public static string eventName = "SpatialSyncedObjectOnOwnerChanged";

        [NullMeansSelf]
        [PortLabelHidden]
        [DoNotSerialize]
        public ValueInput syncedObjectRef { get; private set; }
        protected override bool register => true;

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(eventName);
        }

        protected override void Definition()
        {
            base.Definition();
            syncedObjectRef = ValueInput<SpatialSyncedObject>(nameof(syncedObjectRef), null).NullMeansSelf();
        }

        protected override bool ShouldTrigger(Flow flow, SpatialSyncedObject args)
        {
            if (args == null)
            {
                return false;
            }
            if (flow.GetValue<SpatialSyncedObject>(syncedObjectRef) == args)
            {
                return true;
            }
            return false;
        }
    }

    [UnitTitle("Spatial Synced Object: On Object Initialized")]
    [UnitSurtitle("Spatial Synced Object")]
    [UnitShortTitle("On Object Initialized")]
    [UnitCategory("Events\\Spatial\\Synced Object")]
    [TypeIcon(typeof(SpatialSyncedObject))]
    public class SpatialSyncedObjectEventOnObjectInitialized : EventUnit<SpatialSyncedObject>
    {
        public static string eventName = "SpatialSyncedObjectOnObjectIntialized";

        [NullMeansSelf]
        [PortLabelHidden]
        [DoNotSerialize]
        public ValueInput syncedObjectRef { get; private set; }
        protected override bool register => true;

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(eventName);
        }

        protected override void Definition()
        {
            base.Definition();
            syncedObjectRef = ValueInput<SpatialSyncedObject>(nameof(syncedObjectRef), null).NullMeansSelf();
        }

        protected override bool ShouldTrigger(Flow flow, SpatialSyncedObject args)
        {
            if (args == null)
            {
                return false;
            }
            if (flow.GetValue<SpatialSyncedObject>(syncedObjectRef) == args)
            {
                return true;
            }
            return false;
        }
    }
}