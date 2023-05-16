using UnityEngine;

namespace SpatialSys.UnitySDK.Editor
{
    public static class AvatarAttachmentPackageTests
    {
        [PackageTest(PackageType.AvatarAttachment)]
        public static void EnsurePrefabIsAssigned(AvatarAttachmentConfig config)
        {
            if (config.prefab == null)
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(config, TestResponseType.Fail, "A prefab with the Spatial Avatar Attachment component must be assigned in the config.")
                );
            }
        }
    }
}
