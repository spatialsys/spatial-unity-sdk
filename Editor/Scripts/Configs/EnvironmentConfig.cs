using UnityEditor;
using UnityEngine;

namespace SpatialSys.UnitySDK.Editor
{
    public class EnvironmentConfig : PackageConfig
    {
        public Variant[] variants = new Variant[1] { new Variant() };

        public override PackageType packageType => PackageType.Environment;

        [System.Serializable]
        public class Variant
        {
            public string name = "Default";
            [HideInInspector]
            public string id; // We want this to be unique and never change
            public SceneAsset scene = null;
            public Texture2D thumbnail = null; // 1024x512
            public Texture2D miniThumbnail = null; // 64x64
            public Color thumbnailColor = Color.blue;

            public string bundleName
            {
                get
                {
                    if (scene == null)
                        return null;

                    string scenePath = AssetDatabase.GetAssetPath(scene);
                    AssetImporter importer = AssetImporter.GetAtPath(scenePath);
                    if (importer == null)
                        return null;

                    return importer.assetBundleName;
                }
            }

            public static string NewID()
            {
                return System.Guid.NewGuid().ToString().Replace("-", "");
            }
        }

        private void OnValidate()
        {
            // If a new variant is added, unity will essentially duplicate the last variant in the array so we need to clear it
            if (variants.Length >= 2)
            {
                // Check if a duplicate was just made
                Variant beforeLastVariant = variants[variants.Length - 2];
                Variant lastVariant = variants[variants.Length - 1];
                if (beforeLastVariant.id == lastVariant.id &&
                    beforeLastVariant.scene == lastVariant.scene &&
                    beforeLastVariant.thumbnail == lastVariant.thumbnail)
                {
                    var newVariant = new Variant();
                    newVariant.name = $"Variant{variants.Length}";
                    variants[variants.Length - 1] = newVariant;
                }
            }

            // Assign unique IDs to variants if they don't have one
            foreach (Variant variant in variants)
            {
                if (string.IsNullOrEmpty(variant.id))
                    variant.id = Variant.NewID();
            }
        }

        public override bool IsMainAssetForPackage(Object asset)
        {
            foreach (Variant variant in variants)
            {
                if (variant.scene == asset)
                    return true;
            }

            return false;
        }
    }
}
