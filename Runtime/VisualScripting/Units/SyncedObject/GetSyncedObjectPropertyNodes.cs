using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial Synced Object: Get By ID")]
    [UnitSurtitle("Spatial Synced Object")]
    [UnitShortTitle("Get Synced Object By ID")]
    [UnitCategory("Spatial\\Synced Object")]
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

            syncedObject = ValueOutput<SpatialSyncedObject>(nameof(syncedObject), (f) => SpatialBridge.spaceContentService.GetSyncedObjectByID(f.GetValue<int>(objectID)));
        }
    }

    [UnitTitle("Spatial Synced Object: Get Is Synced")]
    [UnitSurtitle("Spatial Synced Object")]
    [UnitShortTitle("Get Synced Object Is Synced")]
    [UnitCategory("Spatial\\Synced Object")]
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

            isSynced = ValueOutput<bool>(nameof(isSynced), (f) => SpatialBridge.spaceContentService.GetSyncedObjectIsSynced(f.GetValue<SpatialSyncedObject>(syncedObject)));
        }
    }


    [UnitTitle("Spatial Synced Object: Get ID")]
    [UnitSurtitle("Spatial Synced Object")]
    [UnitShortTitle("Get Synced Object ID")]
    [UnitCategory("Spatial\\Synced Object")]
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

            objectID = ValueOutput<int>(nameof(objectID), (f) => SpatialBridge.spaceContentService.GetSyncedObjectID(f.GetValue<SpatialSyncedObject>(syncedObject)));
        }
    }

    [UnitTitle("Spatial Synced Object: Get Owner")]
    [UnitSurtitle("Spatial Synced Object")]
    [UnitShortTitle("Get Owner of Synced Object")]
    [UnitCategory("Spatial\\Synced Object")]
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

            owner = ValueOutput<int>(nameof(owner), (f) => SpatialBridge.spaceContentService.GetSyncedObjectOwner(f.GetValue<SpatialSyncedObject>(syncedObject)));
        }
    }

    [UnitTitle("Spatial Synced Object: Get Has Control")]
    [UnitSurtitle("Spatial Synced Object")]
    [UnitShortTitle("Get Has Control Of Synced Object")]
    [UnitCategory("Spatial\\Synced Object")]
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

            hasControl = ValueOutput<bool>(nameof(hasControl), (f) => SpatialBridge.spaceContentService.GetSyncedObjectHasControl(f.GetValue<SpatialSyncedObject>(syncedObject)));
        }
    }
}
