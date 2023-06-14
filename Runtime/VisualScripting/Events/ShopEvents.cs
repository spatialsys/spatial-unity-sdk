using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial: On Shop Menu Open Changed")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("On Shop Menu Open Changed")]
    [UnitCategory("Events\\Spatial")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class OnShopMenuOpenChangedNode : EventUnit<bool>
    {
        public static string eventName = "SpatialOnShopMenuOpenChanged";
        protected override bool register => true;

        [DoNotSerialize]
        public ValueOutput isOpen { get; private set; }

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(eventName);
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

    [UnitTitle("Spatial: On Shop Item Purchased")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("On Shop Item Purchased")]
    [UnitCategory("Events\\Spatial")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class OnShopItemPurchasedNode : EventUnit<string>
    {
        public static string eventName = "SpatialOnShopItemPurchased";
        protected override bool register => true;

        [DoNotSerialize]
        public ValueOutput itemID { get; private set; }

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(eventName);
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
}