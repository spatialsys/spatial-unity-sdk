using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace SpatialSys.UnitySDK.Editor
{
    public class SpaceTemplateConfig : PackageConfig
    {
        public static readonly Vector2Int MINI_THUMBNAIL_TEXTURE_DIMENSIONS = new Vector2Int(64, 64);

        public Variant[] variants = new Variant[1] { new Variant() };

        public override PackageType packageType => PackageType.SpaceTemplate;
        public override Vector2Int thumbnailDimensions => new Vector2Int(1024, 512);
        public override string bundleName => throw new System.InvalidOperationException("Access the bundle names through the variants array");

        public override bool allowTransparentThumbnails => false;

        public override IEnumerable<Object> assets
        {
            get
            {
                foreach (Variant variant in variants)
                {
                    if (variant.scene != null)
                        yield return variant.scene;
                }
            }
        }

        [System.Serializable]
        public class Variant
        {
            public string name = "Default";
            [HideInInspector]
            public string id; // We want this to be unique and never change
            public SceneAsset scene = null;
            public Texture2D thumbnail = null;
            public Texture2D miniThumbnail = null;
            public Color thumbnailColor = Color.blue;

            public string bundleName => EditorUtility.GetAssetBundleName(scene);

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
    }
}
