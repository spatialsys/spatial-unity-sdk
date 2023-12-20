using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial: On Avatar Attachment Equip Changed")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("On Avatar Attachment Equip Changed")]
    [UnitCategory("Events\\Spatial\\Avatar Attachments")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class OnAvatarAttachmentEquipChangedNode : EventUnit<(string, bool)>
    {
        private const string EVENT_HOOK_ID = "SpatialOnAvatarAttachmentEquipChanged";

        protected override bool register => true;

        [DoNotSerialize]
        [PortLabel("Asset ID")]
        public ValueInput itemID { get; private set; }

        [DoNotSerialize]
        public ValueOutput isEquipped { get; private set; }

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EVENT_HOOK_ID);
        }

        public static void TriggerEvent(string assetID, bool isEquipped)
        {
            EventBus.Trigger(EVENT_HOOK_ID, (assetID, isEquipped));
        }

        protected override void Definition()
        {
            base.Definition();
            itemID = ValueInput<string>(nameof(itemID), ""); // is actually item ID or package SKU
            isEquipped = ValueOutput<bool>(nameof(isEquipped));
        }

        protected override bool ShouldTrigger(Flow flow, (string, bool) args)
        {
            return flow.GetValue<string>(itemID) == args.Item1;
        }

        protected override void AssignArguments(Flow flow, (string, bool) args)
        {
            flow.SetValue(isEquipped, args.Item2);
        }
    }
}