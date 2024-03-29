using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial: On Shop Menu Open Changed")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("On Shop Menu Open Changed")]
    [UnitCategory("Events\\Spatial\\Space Economy")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class OnShopMenuOpenChangedNode : EventUnit<bool>
    {
        private const string EVENT_HOOK_ID = "SpatialOnShopMenuOpenChanged";

        protected override bool register => true;

        [DoNotSerialize]
        public ValueOutput isOpen { get; private set; }

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EVENT_HOOK_ID);
        }

        public static void TriggerEvent(bool open)
        {
            EventBus.Trigger(EVENT_HOOK_ID, open);
        }

        protected override void Definition()
        {
            base.Definition();
            isOpen = ValueOutput<bool>(nameof(isOpen));
        }

        protected override void AssignArguments(Flow flow, bool isOpen)
        {
            flow.SetValue(this.isOpen, isOpen);
        }
    }

    [UnitTitle("Spatial: On Shop Item Purchased")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("On Shop Item Purchased")]
    [UnitCategory("Events\\Spatial\\Space Economy")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class OnShopSpecificItemPurchasedNode : EventUnit<string>
    {
        private const string EVENT_HOOK_ID = "SpatialOnShopItemPurchased";

        protected override bool register => true;

        [DoNotSerialize]
        public ValueInput itemID { get; private set; }

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EVENT_HOOK_ID);
        }

        public static void TriggerEvent(ItemPurchasedEventArgs args)
        {
            EventBus.Trigger(EVENT_HOOK_ID, args.itemID);
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

    [UnitTitle("Spatial: On Shop Any Item Purchased")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("On Shop Any Item Purchased")]
    [UnitCategory("Events\\Spatial\\Space Economy")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class OnShopItemPurchasedNode : EventUnit<string>
    {
        private const string EVENT_HOOK_ID = "SpatialOnShopAnyItemPurchased";

        protected override bool register => true;

        [DoNotSerialize]
        public ValueOutput itemID { get; private set; }

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EVENT_HOOK_ID);
        }

        public static void TriggerEvent(ItemPurchasedEventArgs args)
        {
            EventBus.Trigger(EVENT_HOOK_ID, args.itemID);
        }

        protected override void Definition()
        {
            base.Definition();
            itemID = ValueOutput<string>(nameof(itemID));
        }

        protected override void AssignArguments(Flow flow, string itemID)
        {
            flow.SetValue(this.itemID, itemID);
        }
    }
}