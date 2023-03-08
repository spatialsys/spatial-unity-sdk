using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial Synced Object: Get By ID")]
    [UnitSurtitle("Spatial Synced Object")]
    [UnitShortTitle("Get Synced Object By ID")]
    [UnitCategory("Spatial\\Get Actions")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetSyncedObjectByIDNode : Unit
    {
        [DoNotSerialize]
        public ValueInput objectID { get; private set; }

        [DoNotSerialize]
        public ValueOutput syncedObject { get; private set; }

        protected override void Definition()
        {
            objectID = ValueInput<int>(nameof(objectID));

            syncedObject = ValueOutput<SpatialSyncedObject>(nameof(syncedObject), (f) => ClientBridge.GetSyncedObjectByID.Invoke(f.GetValue<int>(objectID)));
        }
    }

    [UnitTitle("Spatial Synced Object: Get Is Synced")]
    [UnitSurtitle("Spatial Synced Object")]
    [UnitShortTitle("Get Synced Object Is Synced")]
    [UnitCategory("Spatial\\Get Actions")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetSyncedObjectIsSyncedNode : Unit
    {
        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput syncedObject { get; private set; }

        [DoNotSerialize]
        public ValueOutput isSynced { get; private set; }

        protected override void Definition()
        {
            syncedObject = ValueInput<SpatialSyncedObject>(nameof(syncedObject), null).NullMeansSelf();

            isSynced = ValueOutput<bool>(nameof(isSynced), (f) => ClientBridge.GetSyncedObjectIsSynced.Invoke(f.GetValue<SpatialSyncedObject>(syncedObject)));
        }
    }


    [UnitTitle("Spatial Synced Object: Get ID")]
    [UnitSurtitle("Spatial Synced Object")]
    [UnitShortTitle("Get Synced Object ID")]
    [UnitCategory("Spatial\\Get Actions")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetSyncedObjectIDNode : Unit
    {
        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput syncedObject { get; private set; }

        [DoNotSerialize]
        public ValueOutput objectID { get; private set; }

        protected override void Definition()
        {
            syncedObject = ValueInput<SpatialSyncedObject>(nameof(syncedObject), null).NullMeansSelf();

            objectID = ValueOutput<int>(nameof(objectID), (f) => ClientBridge.GetSyncedObjectID.Invoke(f.GetValue<SpatialSyncedObject>(syncedObject)));
        }
    }

    [UnitTitle("Spatial Synced Object: Get Owner")]
    [UnitSurtitle("Spatial Synced Object")]
    [UnitShortTitle("Get Owner of Synced Object")]
    [UnitCategory("Spatial\\Get Actions")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetSyncedObjectOwnerNode : Unit
    {
        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput syncedObject { get; private set; }

        [DoNotSerialize]
        public ValueOutput owner { get; private set; }

        protected override void Definition()
        {
            syncedObject = ValueInput<SpatialSyncedObject>(nameof(syncedObject), null).NullMeansSelf();

            owner = ValueOutput<int>(nameof(owner), (f) => ClientBridge.GetSyncedObjectOwner.Invoke(f.GetValue<SpatialSyncedObject>(syncedObject)));
        }
    }

    [UnitTitle("Spatial Synced Object: Get Has Control")]
    [UnitSurtitle("Spatial Synced Object")]
    [UnitShortTitle("Get Has Control Of Synced Object")]
    [UnitCategory("Spatial\\Get Actions")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetSyncedObjectHasControlNode : Unit
    {
        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput syncedObject { get; private set; }

        [DoNotSerialize]
        public ValueOutput hasControl { get; private set; }

        protected override void Definition()
        {
            syncedObject = ValueInput<SpatialSyncedObject>(nameof(syncedObject), null).NullMeansSelf();

            hasControl = ValueOutput<bool>(nameof(hasControl), (f) => ClientBridge.GetSyncedObjectHasControl.Invoke(f.GetValue<SpatialSyncedObject>(syncedObject)));
        }
    }
}
