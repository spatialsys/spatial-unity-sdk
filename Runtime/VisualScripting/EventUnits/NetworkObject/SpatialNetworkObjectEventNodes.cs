using System;
using UnityEngine;
using Unity.VisualScripting;
using SpatialSys.UnitySDK;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial Network Object: On Owner Changed")]
    [UnitSurtitle("Spatial Network Object")]
    [UnitShortTitle("On Owner Changed")]
    [UnitCategory("Events\\Spatial\\Network Object")]
    [TypeIcon(typeof(SpatialNetworkObject))]
    public class NetworkObject_OnOwnerChangedNode : EventUnit<(SpatialNetworkObject, int)>
    {
        private const string EVENT_HOOK_ID = "NetworkObject_OnOwnerChanged";

        [NullMeansSelf]
        [PortLabelHidden]
        [DoNotSerialize]
        public ValueInput networkObjectRef { get; private set; }
        protected override bool register => true;

        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput ownerID { get; private set; }

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EVENT_HOOK_ID);
        }

        public static void TriggerEvent(SpatialNetworkObject networkObject, int ownerID)
        {
            EventBus.Trigger(EVENT_HOOK_ID, (networkObject, ownerID));
        }

        protected override void Definition()
        {
            base.Definition();
            networkObjectRef = ValueInput<SpatialNetworkObject>(nameof(networkObjectRef), null).NullMeansSelf();
            ownerID = ValueOutput<int>(nameof(ownerID));
        }

        protected override bool ShouldTrigger(Flow flow, (SpatialNetworkObject, int) args)
        {
            if (args.Item1 == null)
                return false;

            return flow.GetValue<SpatialNetworkObject>(networkObjectRef) == args.Item1;
        }

        protected override void AssignArguments(Flow flow, (SpatialNetworkObject, int) args)
        {
            flow.SetValue(ownerID, args.Item2);
        }
    }

    [UnitTitle("Spatial Network Object: On Spawned")]
    [UnitSurtitle("Spatial Network Object")]
    [UnitShortTitle("On Spawned")]
    [UnitCategory("Events\\Spatial\\Network Object")]
    [TypeIcon(typeof(SpatialNetworkObject))]
    public class NetworkObject_OnSpawnedNode : EventUnit<SpatialNetworkObject>
    {
        private const string EVENT_HOOK_ID = "NetworkObject_OnSpawned";

        [NullMeansSelf]
        [PortLabelHidden]
        [DoNotSerialize]
        public ValueInput networkObjectRef { get; private set; }

        protected override bool register => true;

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EVENT_HOOK_ID);
        }

        public static void TriggerEvent(SpatialNetworkObject networkObject)
        {
            EventBus.Trigger(EVENT_HOOK_ID, networkObject);
        }

        protected override void Definition()
        {
            base.Definition();
            networkObjectRef = ValueInput<SpatialNetworkObject>(nameof(networkObjectRef), null).NullMeansSelf();
        }

        protected override bool ShouldTrigger(Flow flow, SpatialNetworkObject args)
        {
            if (args == null)
                return false;

            return flow.GetValue<SpatialNetworkObject>(networkObjectRef) == args;
        }
    }

    [UnitTitle("Spatial Network Object: On Despawned")]
    [UnitSurtitle("Spatial Network Object")]
    [UnitShortTitle("On Despawned")]
    [UnitCategory("Events\\Spatial\\Network Object")]
    [TypeIcon(typeof(SpatialNetworkObject))]
    public class NetworkObject_OnDespawnedNode : EventUnit<SpatialNetworkObject>
    {
        private const string EVENT_HOOK_ID = "NetworkObject_OnDespawned";

        [NullMeansSelf]
        [PortLabelHidden]
        [DoNotSerialize]
        public ValueInput networkObjectRef { get; private set; }

        protected override bool register => true;

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EVENT_HOOK_ID);
        }

        public static void TriggerEvent(SpatialNetworkObject networkObject)
        {
            EventBus.Trigger(EVENT_HOOK_ID, networkObject);
        }

        protected override void Definition()
        {
            base.Definition();
            networkObjectRef = ValueInput<SpatialNetworkObject>(nameof(networkObjectRef), null).NullMeansSelf();
        }

        protected override bool ShouldTrigger(Flow flow, SpatialNetworkObject args)
        {
            if (args == null)
                return false;

            return flow.GetValue<SpatialNetworkObject>(networkObjectRef) == args;
        }
    }
}