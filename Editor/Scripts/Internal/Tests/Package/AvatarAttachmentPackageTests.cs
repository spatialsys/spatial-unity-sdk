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
            if (SpatialValidator.runContext == ValidationRunContext.PublishingPackage && config.validatorUsageContext != PackageConfig.Scope.World)
            {
                // Allow publishing universal auras only if flag is enabled.
                if (SpatialFeatureFlags.currentFlags.universalAuraPublishing &&
                    config.validatorUsageContext == PackageConfig.Scope.Universal &&
                    config.prefab != null &&
                    config.prefab.primarySlot == SpatialAvatarAttachment.Slot.Aura)
                {
                    return;
                }

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

            ValidationUtility.EnsureObjectMeshesMeetGuidelines(config.prefab,
                vertexCountLimit: (config.validatorUsageContext == AvatarConfig.Scope.Universal) ? 2500 : 25000,
                triangleCountLimit: (config.validatorUsageContext == AvatarConfig.Scope.Universal) ? 1000 : 15000,
                subMeshCountLimit: (config.validatorUsageContext == AvatarConfig.Scope.Universal) ? 1 : 5,
                // Bounds for particle systems aren't properly measured, so we don't enforce a bounds size limit for aura attachments
                boundsSizeMinLimit: (config.prefab.primarySlot == SpatialAvatarAttachment.Slot.Aura) ? null : 0.01f,
                boundsSizeMaxLimit: (config.validatorUsageContext == AvatarConfig.Scope.Universal) ? 3f : 25f,
                textureMemoryLimit: (config.validatorUsageContext == AvatarConfig.Scope.Universal) ? 2 * 1024 * 1024 : 10 * 1024 * 1024
            );
        }
    }
}
