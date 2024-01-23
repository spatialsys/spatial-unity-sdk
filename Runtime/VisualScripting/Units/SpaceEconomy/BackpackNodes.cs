using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial: Set Backpack Menu Open")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Set Backpack Menu Open")]
    [UnitCategory("Spatial\\Space Economy")]
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
                SpatialBridge.coreGUIService.SetCoreGUIOpen(SpatialCoreGUITypeFlags.Backpack, f.GetValue<bool>(open));
                return outputTrigger;
            });
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitTitle("Spatial: Add Backpack Item")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Add Backpack Item")]
    [UnitCategory("Spatial\\Space Economy")]
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
        public ValueInput showToastMessage { get; private set; }

        [DoNotSerialize]
        public ValueOutput succeeded { get; private set; }

        protected override void Definition()
        {
            itemID = ValueInput<string>(nameof(itemID), "");
            amount = ValueInput<ulong>(nameof(amount), 1);
            showToastMessage = ValueInput<bool>(nameof(showToastMessage), true);
            succeeded = ValueOutput<bool>(nameof(succeeded));

            inputTrigger = ControlInputCoroutine(nameof(inputTrigger), ExecuteAsync);
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }

        private IEnumerator ExecuteAsync(Flow flow)
        {
            AddInventoryItemRequest request = SpatialBridge.inventoryService.AddItem(flow.GetValue<string>(itemID), flow.GetValue<ulong>(amount), !flow.GetValue<bool>(showToastMessage));
            yield return request;
            flow.SetValue(succeeded, request.succeeded);
            yield return outputTrigger;
        }
    }

    [UnitTitle("Spatial: Delete Backpack Item")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Delete Backpack Item")]
    [UnitCategory("Spatial\\Space Economy")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class DeleteBackpackItemNode : Unit
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
            DeleteInventoryItemRequest request = SpatialBridge.inventoryService.DeleteItem(flow.GetValue<string>(itemID));
            yield return request;
            flow.SetValue(succeeded, request.succeeded);
            yield return outputTrigger;
        }
    }

    [UnitTitle("Spatial: Get Backpack Item")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Get Backpack Item")]
    [UnitCategory("Spatial\\Space Economy")]
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

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                if (SpatialBridge.inventoryService.items.TryGetValue(f.GetValue<string>(itemID), out IInventoryItem item))
                {
                    f.SetValue(isOwned, item.isOwned);
                    f.SetValue(amount, item.amount);
                }
                else
                {
                    f.SetValue(isOwned, false);
                    f.SetValue(amount, (ulong)0);
                }
                return outputTrigger;
            });
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitTitle("Spatial: Use Backpack Item")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Use Backpack Item")]
    [UnitCategory("Spatial\\Space Economy")]
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
            if (SpatialBridge.inventoryService.items.TryGetValue(flow.GetValue<string>(itemID), out IInventoryItem item))
            {
                UseInventoryItemRequest request = item.Use();
                yield return request;
                flow.SetValue(succeeded, request.succeeded);
            }
            else
            {
                flow.SetValue(succeeded, false);
            }
        }
    }

    [UnitTitle("Spatial: Set Backpack Item Enabled")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Set Backpack Item Enabled")]
    [UnitCategory("Spatial\\Space Economy")]
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
                if (SpatialBridge.inventoryService.items.TryGetValue(f.GetValue<string>(itemID), out IInventoryItem item))
                {
                    item.SetEnabled(f.GetValue<bool>(enabled), f.GetValue<string>(disabledMessage));
                }
                else
                {
                    SpatialBridge.loggingService.LogError($"SetBackpackItemEnabledNode: Item {f.GetValue<string>(itemID)} not found");
                }
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitTitle("Spatial: Set Backpack Item Type Enabled")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Set Backpack Item Type Enabled")]
    [UnitCategory("Spatial\\Space Economy")]
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
                SpatialBridge.inventoryService.SetItemTypeEnabled(f.GetValue<ItemType>(itemType), f.GetValue<bool>(enabled), f.GetValue<string>(disabledMessage));
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitTitle("Spatial: Get Consumable Item State")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Get Consumable Item State")]
    [UnitCategory("Spatial\\Space Economy")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetConsumableItemStateNode : Unit
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
        public ValueOutput isActive { get; private set; }

        [DoNotSerialize]
        public ValueOutput durationRemaining { get; private set; }

        [DoNotSerialize]
        public ValueOutput onCooldown { get; private set; }

        [DoNotSerialize]
        public ValueOutput cooldownRemaining { get; private set; }

        protected override void Definition()
        {
            itemID = ValueInput<string>(nameof(itemID), "");

            isActive = ValueOutput<bool>(nameof(isActive));
            durationRemaining = ValueOutput<float>(nameof(durationRemaining));
            onCooldown = ValueOutput<bool>(nameof(onCooldown));
            cooldownRemaining = ValueOutput<float>(nameof(cooldownRemaining));

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                if (SpatialBridge.inventoryService.items.TryGetValue(f.GetValue<string>(itemID), out IInventoryItem item))
                {
                    f.SetValue(isActive, item.isConsumeActive);
                    f.SetValue(durationRemaining, item.consumableDurationRemaining);
                    f.SetValue(onCooldown, item.isOnCooldown);
                    f.SetValue(cooldownRemaining, item.consumableCooldownRemaining);
                }
                else
                {
                    SpatialBridge.loggingService.LogError($"GetConsumableItemStateNode: Item {f.GetValue<string>(itemID)} not found");
                }
                return outputTrigger;
            });
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }
}
