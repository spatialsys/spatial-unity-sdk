using System;
using NUnit.Framework;
using UnityEngine;
using SpatialSys.UnitySDK.Editor;

namespace SpatialSys.UnitySDK.Tests
{
    [Obsolete]
    public class PackageConfig_OLDTests
    {
        [Test]
        public void UpgradeFromV0To1()
        {
            Texture2D tex = new Texture2D(1, 1);

            PackageConfig_OLD config = ScriptableObject.CreateInstance<PackageConfig_OLD>();
            config.configVersion = 0;
            config.packageName = "Spatial SDK";
            config.description = "Description";
            config.deprecated_usageType = PackageConfig_OLD.Deprecated.UsageTypeV0.Gallery;
            config.deprecated_environmentVariants = new PackageConfig_OLD.Deprecated.EnvironmentVariantV0[1] {
                new PackageConfig_OLD.Deprecated.EnvironmentVariantV0() {
                    name = "My Environment",
                    scene = null,
                    thumbnail = tex,
                    thumbnailColor = Color.blue
                }
            };

            config.UpgradeDataIfNecessary();

            // Confirm that the data was upgraded
            Assert.AreEqual(config.configVersion, PackageConfig_OLD.LATEST_VERSION);
            Assert.AreEqual(config.packageName, "Spatial SDK");
            Assert.AreEqual(config.description, "Description");
            Assert.AreEqual(config.environment.useCases, new string[] { "Gallery" });
            Assert.AreEqual(config.environment.variants.Length, 1);
            Assert.AreEqual(config.environment.variants[0].name, "My Environment");
            Assert.AreEqual(config.environment.variants[0].thumbnail, tex);
            Assert.AreEqual(config.environment.variants[0].thumbnailColor, Color.blue);
        }
    }
}