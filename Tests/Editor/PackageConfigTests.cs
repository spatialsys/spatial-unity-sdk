using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using SpatialSys.UnitySDK;
using SpatialSys.UnitySDK.Editor;

namespace SpatialSys.UnitySDK.Tests
{
    public class PackageConfigTests
    {
        [Test]
        public void UpgradeFromV0To1()
        {
            Texture2D tex = new Texture2D(1, 1);

            PackageConfig config = ScriptableObject.CreateInstance<PackageConfig>();
            config.configVersion = 0;
            config.packageName = "Spatial SDK";
            config.description = "Description";
#pragma warning disable 0618
            config.deprecated_usageType = PackageConfig.Deprecated.UsageTypeV0.Gallery;
            config.deprecated_environmentVariants = new PackageConfig.Deprecated.EnvironmentVariantV0[1] {
                new PackageConfig.Deprecated.EnvironmentVariantV0() {
                    name = "My Environment",
                    scene = null,
                    thumbnail = tex,
                    thumbnailColor = Color.blue
                }
            };
#pragma warning restore 0618

            config.UpgradeDataIfNecessary();

            // Confirm that the data was upgraded
            Assert.AreEqual(config.configVersion, PackageConfig.LATEST_VERSION);
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