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

        [PackageTest(PackageType.AvatarAttachment)]
        public static void EnsureScopeIsWorldForPublishing(AvatarAttachmentConfig config)
        {
            if (SpatialValidator.validationContext == ValidationContext.PublishingPackage && config.usageContext != PackageConfig.Scope.World)
            {
                SpatialValidator.AddResponse(
                    new SpatialTestResponse(
                        config,
                        TestResponseType.Fail,
                        "We currently only allow you to publish world-scoped avatar attachments. Please change the scope to World.",
                        "Publishing universal attachments will be supported in the future."
                    )
                );
            }
        }

        [PackageTest(PackageType.AvatarAttachment)]
        public static void EnsureAvatarAttachmentMeshesMeetGuidelines(AvatarAttachmentConfig config)
        {
            if (config.prefab == null)
                return;

            ValidationUtility.EnsureObjectMeshesMeetGuidelines(config.prefab, 50000, 22500, 4, 2.5f);
        }
    }
}
