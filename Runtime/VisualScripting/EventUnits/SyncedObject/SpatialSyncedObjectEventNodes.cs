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
    public class SpatialSyncedObjectEventOnOwnerChanged : EventUnit<(SpatialSyncedObject, int)>
    {
        private const string EVENT_HOOK_ID = "SpatialSyncedObjectOnOwnerChanged";

        [NullMeansSelf]
        [PortLabelHidden]
        [DoNotSerialize]
        public ValueInput syncedObjectRef { get; private set; }
        protected override bool register => true;

        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput ownerID { get; private set; }

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EVENT_HOOK_ID);
        }

        public static void TriggerEvent(SpatialSyncedObject spatialSyncedObject, int ownerID)
        {
            EventBus.Trigger(EVENT_HOOK_ID, spatialSyncedObject);
        }

        protected override void Definition()
        {
            base.Definition();
            syncedObjectRef = ValueInput<SpatialSyncedObject>(nameof(syncedObjectRef), null).NullMeansSelf();
            ownerID = ValueOutput<int>(nameof(ownerID));
        }

        protected override bool ShouldTrigger(Flow flow, (SpatialSyncedObject, int) args)
        {
            if (args.Item1 == null)
            {
                return false;
            }
            return flow.GetValue<SpatialSyncedObject>(syncedObjectRef) == args.Item1;
        }

        protected override void AssignArguments(Flow flow, (SpatialSyncedObject, int) args)
        {
            flow.SetValue(ownerID, args.Item2);
        }
    }

    [UnitTitle("Spatial Synced Object: On Object Initialized")]
    [UnitSurtitle("Spatial Synced Object")]
    [UnitShortTitle("On Object Initialized")]
    [UnitCategory("Events\\Spatial\\Synced Object")]
    [TypeIcon(typeof(SpatialSyncedObject))]
    public class SpatialSyncedObjectEventOnObjectInitialized : EventUnit<SpatialSyncedObject>
    {
        private const string EVENT_HOOK_ID = "SpatialSyncedObjectOnObjectIntialized";

        [NullMeansSelf]
        [PortLabelHidden]
        [DoNotSerialize]
        public ValueInput syncedObjectRef { get; private set; }
        protected override bool register => true;

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EVENT_HOOK_ID);
        }

        public static void TriggerEvent(SpatialSyncedObject spatialSyncedObject)
        {
            EventBus.Trigger(EVENT_HOOK_ID, spatialSyncedObject);
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