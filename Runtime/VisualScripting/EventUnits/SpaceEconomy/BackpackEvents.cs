using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial: On Backpack Menu Open Changed")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("On Backpack Menu Open Changed")]
    [UnitCategory("Events\\Spatial\\Space Economy")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class OnBackpackMenuOpenChangedNode : EventUnit<bool>
    {
        private const string EVENT_HOOK_ID = "SpatialOnBackpackMenuOpenChanged";
        protected override bool register => true;

        [DoNotSerialize]
        public ValueOutput isOpen { get; private set; }

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EVENT_HOOK_ID);
        }

        public static void TriggerEvent(bool isOpen)
        {
            EventBus.Trigger(EVENT_HOOK_ID, isOpen);
        }

        protected override void Definition()
        {
            base.Definition();
            isOpen = ValueOutput<bool>(nameof(isOpen));
        }

        protected override bool ShouldTrigger(Flow flow, bool isOpen)
        {
            return true;
        }

        protected override void AssignArguments(Flow flow, bool isOpen)
        {
            flow.SetValue(this.isOpen, isOpen);
        }
    }

    [UnitTitle("Spatial: On Backpack Item Owned Changed")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("On Backpack Item Owned Changed")]
    [UnitCategory("Events\\Spatial\\Space Economy")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class OnBackpackItemOwnedChangedNode : EventUnit<(string, bool)>
    {
        private const string EVENT_HOOK_ID = "SpatialOnBackpackItemOwnedChanged";
        protected override bool register => true;

        [DoNotSerialize]
        public ValueInput itemID { get; private set; }

        [DoNotSerialize]
        public ValueOutput isOwned { get; private set; }

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EVENT_HOOK_ID);
        }

        public static void TriggerEvent(string itemID, bool owned)
        {
            EventBus.Trigger(EVENT_HOOK_ID, (itemID, owned));
        }

        protected override void Definition()
        {
            base.Definition();
            itemID = ValueInput<string>(nameof(itemID), "");
            isOwned = ValueOutput<bool>(nameof(isOwned));
        }

        protected override bool ShouldTrigger(Flow flow, (string, bool) args)
        {
            return flow.GetValue<string>(itemID) == args.Item1;
        }

        protected override void AssignArguments(Flow flow, (string, bool) args)
        {
            flow.SetValue(isOwned, args.Item2);
        }
    }

    [UnitTitle("Spatial: On Backpack Any Item Owned Changed")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("On Backpack Any Item Owned Changed")]
    [UnitCategory("Events\\Spatial\\Space Economy")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class OnBackpackAnyItemOwnedChangedNode : EventUnit<(string, bool)>
    {
        private const string EVENT_HOOK_ID = "SpatialOnBackpackAnyItemOwnedChanged";
        protected override bool register => true;

        [DoNotSerialize]
        public ValueOutput itemID { get; private set; }

        [DoNotSerialize]
        public ValueOutput isOwned { get; private set; }

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EVENT_HOOK_ID);
        }

        public static void TriggerEvent(string itemID, bool owned)
        {
            EventBus.Trigger(EVENT_HOOK_ID, (itemID, owned));
        }

        protected override void Definition()
        {
            base.Definition();
            itemID = ValueOutput<string>(nameof(itemID));
            isOwned = ValueOutput<bool>(nameof(isOwned));
        }

        protected override bool ShouldTrigger(Flow flow, (string, bool) args)
        {
            return true;
        }

        protected override void AssignArguments(Flow flow, (string, bool) args)
        {
            flow.SetValue(itemID, args.Item1);
            flow.SetValue(isOwned, args.Item2);
        }
    }


    [UnitTitle("Spatial: On Backpack Item Amount Changed")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("On Backpack Item Amount Changed")]
    [UnitCategory("Events\\Spatial\\Space Economy")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class OnBackpackItemAmountChangedNode : EventUnit<(string, ulong)>
    {
        private const string EVENT_HOOK_ID = "SpatialOnBackpackItemAmountChanged";
        protected override bool register => true;

        [DoNotSerialize]
        public ValueInput itemID { get; private set; }

        [DoNotSerialize]
        public ValueOutput amount { get; private set; }

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EVENT_HOOK_ID);
        }

        public static void TriggerEvent(string itemID, ulong amount)
        {
            EventBus.Trigger(EVENT_HOOK_ID, (itemID, amount));
        }

        protected override void Definition()
        {
            base.Definition();
            itemID = ValueInput<string>(nameof(itemID), "");
            amount = ValueOutput<ulong>(nameof(amount));
        }

        protected override bool ShouldTrigger(Flow flow, (string, ulong) args)
        {
            return flow.GetValue<string>(itemID) == args.Item1;
        }

        protected override void AssignArguments(Flow flow, (string, ulong) args)
        {
            flow.SetValue(amount, args.Item2);
        }
    }

    [UnitTitle("Spatial: On Backpack Item Used")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("On Backpack Item Used")]
    [UnitCategory("Events\\Spatial\\Space Economy")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class OnBackpackItemUsedNode : EventUnit<string>
    {
        private const string EVENT_HOOK_ID = "SpatialOnBackpackItemUsed";
        protected override bool register => true;

        [DoNotSerialize]
        public ValueInput itemID { get; private set; }

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EVENT_HOOK_ID);
        }

        public static void TriggerEvent(string itemID)
        {
            EventBus.Trigger(EVENT_HOOK_ID, itemID);
        }


        protected override void Definition()
        {
            base.Definition();
            itemID = ValueInput<string>(nameof(itemID), "");
        }

        protected override bool ShouldTrigger(Flow flow, string arg)
        {
            return flow.GetValue<string>(itemID) == arg;
        }
    }

    [UnitTitle("Spatial: On Backpack Any Item Used")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("On Backpack Any Item Used")]
    [UnitCategory("Events\\Spatial\\Space Economy")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class OnBackpackAnyItemUsedNode : EventUnit<string>
    {
        private const string EVENT_HOOK_ID = "SpatialOnBackpackAnyItemUsed";
        protected override bool register => true;

        [DoNotSerialize]
        public ValueOutput itemID { get; private set; }

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EVENT_HOOK_ID);
        }

        public static void TriggerEvent(string itemID)
        {
            EventBus.Trigger(EVENT_HOOK_ID, itemID);
        }

        protected override void Definition()
        {
            base.Definition();
            itemID = ValueOutput<string>(nameof(itemID));
        }

        protected override bool ShouldTrigger(Flow flow, string itemID)
        {
            return true;
        }

        protected override void AssignArguments(Flow flow, string itemID)
        {
            flow.SetValue(this.itemID, itemID);
        }
    }

    [UnitTitle("Spatial: On Backpack Item Consumed")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("On Backpack Item Consumed")]
    [UnitCategory("Events\\Spatial\\Space Economy")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class OnBackpackItemConsumedNode : EventUnit<string>
    {
        private const string EVENT_HOOK_ID = "SpatialOnBackpackItemConsumed";
        protected override bool register => true;

        [DoNotSerialize]
        public ValueInput itemID { get; private set; }

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EVENT_HOOK_ID);
        }

        public static void TriggerEvent(string itemID)
        {
            EventBus.Trigger(EVENT_HOOK_ID, itemID);
        }

        protected override void Definition()
        {
            base.Definition();
            itemID = ValueInput<string>(nameof(itemID), "");
        }

        protected override bool ShouldTrigger(Flow flow, string arg)
        {
            return flow.GetValue<string>(itemID) == arg;
        }
    }

    [UnitTitle("Spatial: On Backpack Any Item Consumed")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("On Backpack Any Item Consumed")]
    [UnitCategory("Events\\Spatial\\Space Economy")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class OnBackpackAnyItemConsumedNode : EventUnit<string>
    {
        private const string EVENT_HOOK_ID = "SpatialOnBackpackAnyItemConsumed";
        protected override bool register => true;

        [DoNotSerialize]
        public ValueOutput itemID { get; private set; }

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EVENT_HOOK_ID);
        }

        public static void TriggerEvent(string itemID)
        {
            EventBus.Trigger(EVENT_HOOK_ID, itemID);
        }

        protected override void Definition()
        {
            base.Definition();
            itemID = ValueOutput<string>(nameof(itemID));
        }

        protected override bool ShouldTrigger(Flow flow, string itemID)
        {
            return true;
        }

        protected override void AssignArguments(Flow flow, string itemID)
        {
            flow.SetValue(this.itemID, itemID);
        }
    }

    [UnitTitle("Spatial: On Consumable Item Duration Expired")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("On Consumable Item Duration Expired")]
    [UnitCategory("Events\\Spatial\\Space Economy")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class OnConsumableItemDurationExpiredNode : EventUnit<string>
    {
        private const string EVENT_HOOK_ID = "SpatialOnConsumableItemDurationExpired";
        protected override bool register => true;

        [DoNotSerialize]
        public ValueInput itemID { get; private set; }

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EVENT_HOOK_ID);
        }

        public static void TriggerEvent(string itemID)
        {
            EventBus.Trigger(EVENT_HOOK_ID, itemID);
        }

        protected override void Definition()
        {
            base.Definition();
            itemID = ValueInput<string>(nameof(itemID), "");
        }

        protected override bool ShouldTrigger(Flow flow, string itemID)
        {
            return flow.GetValue<string>(this.itemID) == itemID;
        }
    }
}
