using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial: On Avatar Attachment Equip Changed")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("On Avatar Attachment Equip Changed")]
    [UnitCategory("Events\\Spatial")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class OnAvatarAttachmentEquipChangedNode : EventUnit<(string, bool)>
    {
        public static string eventName = "SpatialOnAvatarAttachmentEquipChanged";
        protected override bool register => true;

        [DoNotSerialize]
        public ValueInput itemID { get; private set; }

        [DoNotSerialize]
        public ValueOutput isEquipped { get; private set; }

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(eventName);
        }

        protected override void Definition()
        {
            base.Definition();
            itemID = ValueInput<string>(nameof(itemID), "");
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