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
        public ValueInput clearSlot { get; private set; }

        [DoNotSerialize]
        public ValueInput optionalTag { get; private set; }

        [DoNotSerialize]
        public ValueOutput succeeded { get; private set; }

        protected override void Definition()
        {
            sku = ValueInput<string>(nameof(sku));
            equip = ValueInput<bool>(nameof(equip), true);
            clearSlot = ValueInput<bool>(nameof(clearSlot), true);
            optionalTag = ValueInput<string>(nameof(optionalTag), "");
            succeeded = ValueOutput<bool>(nameof(succeeded));

            inputTrigger = ControlInputCoroutine(nameof(inputTrigger), ExecuteAsync);
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }

        private IEnumerator ExecuteAsync(Flow flow)
        {
            bool completed = false;
            ClientBridge.EquipAvatarAttachmentPackage?.Invoke(
                flow.GetValue<string>(sku),
                flow.GetValue<bool>(equip),
                flow.GetValue<bool>(clearSlot),
                flow.GetValue<string>(optionalTag),
                success => {
                    completed = true;
                    flow.SetValue(succeeded, success);
                }
            );
            yield return new WaitUntil(() => completed);
            yield return outputTrigger;
        }
    }

    [UnitTitle("Spatial: Equip Avatar Attachment Embedded Package Asset")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Equip Avatar Attachment Embedded Package Asset")]
    [UnitCategory("Spatial\\Avatar Attachments")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class EquipAvatarAttachmentEmbeddedPackageAssetNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput assetID { get; private set; }

        [DoNotSerialize]
        public ValueInput equip { get; private set; }

        [DoNotSerialize]
        public ValueInput clearSlot { get; private set; }

        [DoNotSerialize]
        public ValueInput optionalTag { get; private set; }

        [DoNotSerialize]
        public ValueOutput succeeded { get; private set; }

        protected override void Definition()
        {
            assetID = ValueInput<string>(nameof(assetID));
            equip = ValueInput<bool>(nameof(equip), true);
            clearSlot = ValueInput<bool>(nameof(clearSlot), true);
            optionalTag = ValueInput<string>(nameof(optionalTag), "");
            succeeded = ValueOutput<bool>(nameof(succeeded));

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                bool success = ClientBridge.EquipAvatarAttachmentEmbedded?.Invoke(
                    f.GetValue<string>(assetID),
                    f.GetValue<bool>(equip),
                    f.GetValue<bool>(clearSlot),
                    f.GetValue<string>(optionalTag)
                ) ?? false;
                f.SetValue(succeeded, success);
                return outputTrigger;
            });
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
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
            ClientBridge.EquipAvatarAttachmentItem?.Invoke(flow.GetValue<string>(itemID), flow.GetValue<bool>(equip), success => {
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
        [PortLabel("ID")]
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

    [UnitTitle("Spatial: Clear All Avatar Attachments")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Clear All Avatar Attachments")]
    [UnitCategory("Spatial\\Avatar Attachments")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class ClearAllAvatarAttachmentsNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        protected override void Definition()
        {
            inputTrigger = ControlInputCoroutine(nameof(inputTrigger), ExecuteAsync);
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }

        private IEnumerator ExecuteAsync(Flow flow)
        {
            ClientBridge.ClearAllAvatarAttachments?.Invoke();
            yield return outputTrigger;
        }
    }

    [UnitTitle("Spatial: Clear Avatar Attachment Slot")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Clear Avatar Attachment Slot")]
    [UnitCategory("Spatial\\Avatar Attachments")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class ClearAvatarAttachmentSlotNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput slot { get; private set; }

        protected override void Definition()
        {
            slot = ValueInput<SpatialAvatarAttachment.Slot>(nameof(slot), SpatialAvatarAttachment.Slot.None);

            inputTrigger = ControlInputCoroutine(nameof(inputTrigger), ExecuteAsync);
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }

        private IEnumerator ExecuteAsync(Flow flow)
        {
            ClientBridge.ClearAvatarAttachmentSlot?.Invoke(flow.GetValue<SpatialAvatarAttachment.Slot>(slot));
            yield return outputTrigger;
        }
    }

    [UnitTitle("Spatial: Clear Avatar Attachments By Tag")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Clear Avatar Attachments By Tag")]
    [UnitCategory("Spatial\\Avatar Attachments")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class ClearAvatarAttachmentsByTagNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput tag { get; private set; }

        protected override void Definition()
        {
            tag = ValueInput<string>(nameof(tag), "");

            inputTrigger = ControlInputCoroutine(nameof(inputTrigger), ExecuteAsync);
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }

        private IEnumerator ExecuteAsync(Flow flow)
        {
            ClientBridge.ClearAvatarAttachmentsByTag?.Invoke(flow.GetValue<string>(tag));
            yield return outputTrigger;
        }
    }
}