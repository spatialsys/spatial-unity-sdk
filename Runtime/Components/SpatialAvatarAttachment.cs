using UnityEngine;

namespace SpatialSys.UnitySDK
{
    public class SpatialAvatarAttachment : SpatialPackageAsset
    {
        public enum SlotType
        {
            Aura = 0,
        }

        public enum Category
        {
            Unspecified = 0,
        }

        public override string prettyName => "Avatar Attachment";
        public override string tooltip => "This component is used to define an object that can be attached or equipped by an avatar";
        public override string documentationURL => "https://docs.spatial.io/avatar-attachment";

        public SlotType primarySlotType = SlotType.Aura;
        public SlotType[] additionalSlotTypes;
        public Category category = Category.Unspecified;
    }
}