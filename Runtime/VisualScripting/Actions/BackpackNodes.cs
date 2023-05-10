using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace SpatialSys.UnitySDK.VisualScripting
{
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
            var id = flow.GetValue<string>(itemID);
            var qty = flow.GetValue<int>(amount);
            if (!string.IsNullOrEmpty(id) && qty > 0)
            {
                bool completed = false;
                ClientBridge.AddBackpackItem.Invoke(id, qty, success => {
                    completed = true;
                    flow.SetValue(succeeded, success);
                });
                yield return new WaitUntil(() => completed);
            }
            else
            {
                flow.SetValue(succeeded, false);
            }

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
            amount = ValueOutput<int>(nameof(amount));

            inputTrigger = ControlInputCoroutine(nameof(inputTrigger), ExecuteAsync);
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }

        private IEnumerator ExecuteAsync(Flow flow)
        {
            var id = flow.GetValue<string>(itemID);
            if (!string.IsNullOrEmpty(id))
            {
                bool completed = false;
                ClientBridge.GetBackpackItem.Invoke(id, resp => {
                    completed = true;
                    flow.SetValue(isOwned, resp.userOwnsItem);
                    flow.SetValue(amount, resp.amount);
                });
                yield return new WaitUntil(() => completed);
            }
            else
            {
                flow.SetValue(isOwned, false);
                flow.SetValue(amount, 0);
            }

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
            var id = flow.GetValue<string>(itemID);
            if (!string.IsNullOrEmpty(id))
            {
                bool completed = false;
                ClientBridge.UseBackpackItem.Invoke(id, success => {
                    completed = true;
                    flow.SetValue(succeeded, success);
                });
                yield return new WaitUntil(() => completed);
            }
            else
            {
                flow.SetValue(succeeded, false);
            }

            yield return outputTrigger;
        }
    }
}
