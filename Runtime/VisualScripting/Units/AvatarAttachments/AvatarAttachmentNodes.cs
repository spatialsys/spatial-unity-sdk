using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial: Equip Avatar Attachment Package")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Equip Avatar Attachment Package")]
    [UnitCategory("Spatial\\Avatar Attachments")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class EquipAvatarAttachmentPackageNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput sku { get; private set; }

        [DoNotSerialize]
        public ValueInput equip { get; private set; }

        [DoNotSerialize]
        public ValueOutput succeeded { get; private set; }

        protected override void Definition()
        {
            sku = ValueInput<string>(nameof(sku));
            equip = ValueInput<bool>(nameof(equip), true);
            succeeded = ValueOutput<bool>(nameof(succeeded));

            inputTrigger = ControlInputCoroutine(nameof(inputTrigger), ExecuteAsync);
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }

        private IEnumerator ExecuteAsync(Flow flow)
        {
            bool completed = false;
            ClientBridge.EquipAvatarAttachmentPackage.Invoke(flow.GetValue<string>(sku), flow.GetValue<bool>(equip), success => {
                completed = true;
                flow.SetValue(succeeded, success);
            });
            yield return new WaitUntil(() => completed);
            yield return outputTrigger;
        }
    }

    [UnitTitle("Spatial: Equip Avatar Attachment Item")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Equip Avatar Attachment Item")]
    [UnitCategory("Spatial\\Avatar Attachments")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class EquipAvatarAttachmentItemNode : Unit
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
        public ValueInput equip { get; private set; }

        [DoNotSerialize]
        public ValueOutput succeeded { get; private set; }

        protected override void Definition()
        {
            itemID = ValueInput<string>(nameof(itemID));
            equip = ValueInput<bool>(nameof(equip), true);
            succeeded = ValueOutput<bool>(nameof(succeeded));

            inputTrigger = ControlInputCoroutine(nameof(inputTrigger), ExecuteAsync);
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }

        private IEnumerator ExecuteAsync(Flow flow)
        {
            bool completed = false;
            ClientBridge.EquipAvatarAttachmentItem.Invoke(flow.GetValue<string>(itemID), flow.GetValue<bool>(equip), success => {
                completed = true;
                flow.SetValue(succeeded, success);
            });
            yield return new WaitUntil(() => completed);
            yield return outputTrigger;
        }
    }

    [UnitTitle("Spatial: Is Avatar Attachment Equipped")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Is Avatar Attachment Equipped")]
    [UnitCategory("Spatial\\Avatar Attachments")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class IsAvatarAttachmentEquippedNode : Unit
    {
        [DoNotSerialize]
        public ValueInput itemIDOrPackageSKU { get; private set; }

        [DoNotSerialize]
        public ValueOutput equipped { get; private set; }

        protected override void Definition()
        {
            itemIDOrPackageSKU = ValueInput<string>(nameof(itemIDOrPackageSKU), "");
            equipped = ValueOutput<bool>(nameof(equipped), (flow) => {
                return ClientBridge.IsAvatarAttachmentEquipped(flow.GetValue<string>(itemIDOrPackageSKU));
            });
        }
    }
}