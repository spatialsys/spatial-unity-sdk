using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial: Set Shop Menu Open")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Set Shop Menu Open")]
    [UnitCategory("Spatial\\Actions")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class SetShopMenuOpenNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput open { get; private set; }

        protected override void Definition()
        {
            open = ValueInput<bool>(nameof(open));

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                ClientBridge.SetShopMenuOpen.Invoke(f.GetValue<bool>(open));
                return outputTrigger;
            });
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitTitle("Spatial: Set Shop Item Enabled")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Set Shop Item Enabled")]
    [UnitCategory("Spatial\\Actions")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class SetShopItemEnabledNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput itemID { get; private set; }

        [DoNotSerialize]
        public ValueInput enabled { get; private set; }

        [DoNotSerialize]
        public ValueInput disabledMessage { get; private set; }

        protected override void Definition()
        {
            itemID = ValueInput<string>(nameof(itemID), "");
            enabled = ValueInput<bool>(nameof(enabled), true);
            disabledMessage = ValueInput<string>(nameof(disabledMessage), null);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                ClientBridge.SetShopItemEnabled.Invoke(f.GetValue<string>(itemID), f.GetValue<bool>(enabled), f.GetValue<string>(disabledMessage));
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitTitle("Spatial: Set Shop Item Visibility")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Set Shop Item Visibility")]
    [UnitCategory("Spatial\\Actions")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class SetShopItemVisibilityNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput itemID { get; private set; }

        [DoNotSerialize]
        public ValueInput visible { get; private set; }

        protected override void Definition()
        {
            itemID = ValueInput<string>(nameof(itemID), "");
            visible = ValueInput<bool>(nameof(visible), true);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                ClientBridge.SetShopItemVisibility.Invoke(f.GetValue<string>(itemID), f.GetValue<bool>(visible));
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitTitle("Spatial: Purchase Shop Item")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Purchase Shop Item")]
    [UnitCategory("Spatial\\Actions")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class PurchaseShopItemNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput itemID { get; private set; }

        [DoNotSerialize]
        public ValueInput amount { get; private set; } // total amount in inventory right now

        [DoNotSerialize]
        public ValueOutput succeeded { get; private set; }

        protected override void Definition()
        {
            itemID = ValueInput<string>(nameof(itemID), "");
            amount = ValueInput<ulong>(nameof(amount), 1);
            succeeded = ValueOutput<bool>(nameof(succeeded));

            inputTrigger = ControlInputCoroutine(nameof(inputTrigger), ExecuteAsync);
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }

        private IEnumerator ExecuteAsync(Flow flow)
        {
            bool completed = false;
            ClientBridge.PurchaseShopItem.Invoke(flow.GetValue<string>(itemID), flow.GetValue<ulong>(amount), success => {
                completed = true;
                flow.SetValue(succeeded, success);
            });
            yield return new WaitUntil(() => completed);
            yield return outputTrigger;
        }
    }
}
