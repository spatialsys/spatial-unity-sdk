using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial: Set Backpack Menu Open")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Set Backpack Menu Open")]
    [UnitCategory("Spatial\\Actions")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class SetBackpackMenuOpenNode : Unit
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
                ClientBridge.SetBackpackMenuOpen.Invoke(f.GetValue<bool>(open));
                return outputTrigger;
            });
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitTitle("Spatial: Add Backpack Item")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Add Backpack Item")]
    [UnitCategory("Spatial\\Actions")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class AddBackpackItemNode : Unit
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
        public ValueInput amount { get; private set; }

        [DoNotSerialize]
        public ValueOutput succeeded { get; private set; }

        protected override void Definition()
        {
            itemID = ValueInput<string>(nameof(itemID), "");
            amount = ValueInput<int>(nameof(amount), 1);
            succeeded = ValueOutput<bool>(nameof(succeeded));

            inputTrigger = ControlInputCoroutine(nameof(inputTrigger), ExecuteAsync);
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }

        private IEnumerator ExecuteAsync(Flow flow)
        {
            bool completed = false;
            ClientBridge.AddBackpackItem.Invoke(flow.GetValue<string>(itemID), flow.GetValue<int>(amount), success => {
                completed = true;
                flow.SetValue(succeeded, success);
            });
            yield return new WaitUntil(() => completed);
            yield return outputTrigger;
        }
    }

    [UnitTitle("Spatial: Get Backpack Item")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Get Backpack Item")]
    [UnitCategory("Spatial\\Actions")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetBackpackItemNode : Unit
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
        public ValueOutput isOwned { get; private set; }

        [DoNotSerialize]
        public ValueOutput amount { get; private set; }

        protected override void Definition()
        {
            itemID = ValueInput<string>(nameof(itemID), "");
            isOwned = ValueOutput<bool>(nameof(isOwned));
            amount = ValueOutput<ulong>(nameof(amount));

            inputTrigger = ControlInputCoroutine(nameof(inputTrigger), ExecuteAsync);
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }

        private IEnumerator ExecuteAsync(Flow flow)
        {
            bool completed = false;
            ClientBridge.GetBackpackItem.Invoke(flow.GetValue<string>(itemID), resp => {
                completed = true;
                flow.SetValue(isOwned, resp.userOwnsItem);
                flow.SetValue(amount, resp.amount);
            });
            yield return new WaitUntil(() => completed);
            yield return outputTrigger;
        }
    }

    [UnitTitle("Spatial: Use Backpack Item")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Use Backpack Item")]
    [UnitCategory("Spatial\\Actions")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class UseBackpackItemNode : Unit
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
        public ValueOutput succeeded { get; private set; }

        protected override void Definition()
        {
            itemID = ValueInput<string>(nameof(itemID), "");
            succeeded = ValueOutput<bool>(nameof(succeeded));

            inputTrigger = ControlInputCoroutine(nameof(inputTrigger), ExecuteAsync);
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }

        private IEnumerator ExecuteAsync(Flow flow)
        {
            bool completed = false;
            ClientBridge.UseBackpackItem.Invoke(flow.GetValue<string>(itemID), success => {
                completed = true;
                flow.SetValue(succeeded, success);
            });
            yield return new WaitUntil(() => completed);
            yield return outputTrigger;
        }
    }

    [UnitTitle("Spatial: Set Backpack Item Enabled")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Set Backpack Item Enabled")]
    [UnitCategory("Spatial\\Actions")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class SetBackpackItemEnabledNode : Unit
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
                ClientBridge.SetBackpackItemEnabled.Invoke(f.GetValue<string>(itemID), f.GetValue<bool>(enabled), f.GetValue<string>(disabledMessage));
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitTitle("Spatial: Set Backpack Item Type Enabled")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Set Backpack Item Type Enabled")]
    [UnitCategory("Spatial\\Actions")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class SetBackpackItemTypeEnabledNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput itemType { get; private set; }

        [DoNotSerialize]
        public ValueInput enabled { get; private set; }

        [DoNotSerialize]
        public ValueInput disabledMessage { get; private set; }

        protected override void Definition()
        {
            itemType = ValueInput<ItemType>(nameof(itemType), ItemType.Avatar);
            enabled = ValueInput<bool>(nameof(enabled), true);
            disabledMessage = ValueInput<string>(nameof(disabledMessage), null);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                ClientBridge.SetBackpackItemTypeEnabled.Invoke(f.GetValue<ItemType>(itemType), f.GetValue<bool>(enabled), f.GetValue<string>(disabledMessage));
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }
    }
}