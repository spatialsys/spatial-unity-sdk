using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpatialSys.UnitySDK.Editor
{
    public static class AvatarAttachmentComponentTests
    {
        /// <summary>
        /// Checks that the attachment component's additional slot type list has no duplicate entries.
        /// </summary>
        [ComponentTest(typeof(SpatialAvatarAttachment))]
        public static void CheckAdditionalSlotTypesForDuplicates(SpatialAvatarAttachment attachment)
        {
            if (attachment.additionalSlotTypes == null)
                return;

            var encounteredTypes = new HashSet<SpatialAvatarAttachment.SlotType>();
            foreach (SpatialAvatarAttachment.SlotType slotType in attachment.additionalSlotTypes)
            {
                if (slotType == attachment.primarySlotType)
                {
                    var resp = new SpatialTestResponse(
                        attachment,
                        TestResponseType.Fail,
                        $"The prefab references the primary slot type ({attachment.primarySlotType}) inside of the additional slot types list",
                        "Additional slot types should not include the primary slot type. Remove the primary slot type from the additional slot types list on the prefab to fix this issue."
                    );
                    resp.SetAutoFix(isSafe: true, "Removes the primary slot type from the additional slot types list on the prefab",
                        (Object obj) => {
                            var avatarAttachment = obj as SpatialAvatarAttachment;
                            if (avatarAttachment == null)
                                return;
                            var addSlotTypes = new List<SpatialAvatarAttachment.SlotType>(avatarAttachment.additionalSlotTypes);
                            addSlotTypes.Remove(attachment.primarySlotType);
                            avatarAttachment.additionalSlotTypes = addSlotTypes.ToArray();
                            UnityEditor.EditorUtility.SetDirty(avatarAttachment);
                        }
                    );

                    SpatialValidator.AddResponse(resp);
                }
                else if (!encounteredTypes.Add(slotType))
                {
                    var resp = new SpatialTestResponse(
                        attachment,
                        TestResponseType.Fail,
                        $"The prefab contains a duplicated slot type ({slotType}) inside of the additional slot types list",
                        "Ensure there's only one instance per slot type defined in the additional slot types list on the prefab to fix this issue."
                    );
                    resp.SetAutoFix(isSafe: true, "Removes all duplicate values defined in the additional slot types list",
                        (Object obj) => {
                            var avatarAttachment = obj as SpatialAvatarAttachment;
                            if (avatarAttachment == null)
                                return;
                            var addSlotTypesSet = new HashSet<SpatialAvatarAttachment.SlotType>(avatarAttachment.additionalSlotTypes);
                            avatarAttachment.additionalSlotTypes = addSlotTypesSet.ToArray();
                            UnityEditor.EditorUtility.SetDirty(avatarAttachment);
                        }
                    );

                    SpatialValidator.AddResponse(resp);
                }
            }
        }
    }
}
