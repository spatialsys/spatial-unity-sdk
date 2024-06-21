using UnityEngine;
using System.Collections.Generic;

namespace SpatialSys.UnitySDK.Editor
{
    /// <summary>
    /// Base class for all package config types (SpaceTemplate, Avatar, etc.)
    /// </summary>
    public abstract class PackageConfig : ScriptableObject
    {
        public const int MAX_NAME_LENGTH = 50;

        public enum Scope
        {
            Universal = 0,
            World = 1
        }

#pragma warning disable 414 // suppress "never assigned" warnings until we use it
        [HideInInspector]
        public int version; // version of this config model; Used for making backwards-compatible changes
#pragma warning restore 414

        public string sku = "";
        [Tooltip("Display name of the asset as it would show up in Spatial")]
        public string packageName;
        public Texture2D thumbnail = null;
        [HideInInspector]
        public SavedProjectSettings savedProjectSettings;

        public abstract PackageType packageType { get; }
        public abstract Vector2Int thumbnailDimensions { get; }
        public abstract string bundleName { get; }
        /// <summary>
        /// A unique string used to identify which validation rules to use (e.g. in ValidComponents)
        /// </summary>
        public abstract string validatorID { get; }
        /// <summary>
        /// Defines the usage context to use when running validation tests, which can affect behavior, limit values, different restrictions, etc.
        /// This will apply to all dependencies and assets used by this config.
        /// Example: A SpatialAvatar embedded asset is a dependency of a Space config, which is implicitly World context, so it will always be validated as a World SpatialAvatar.
        /// </summary>
        public abstract Scope validatorUsageContext { get; }

        /// <summary>
        /// Can the thumbnail have no transparent pixels?
        /// </summary>
        public virtual bool allowOpaqueThumbnails => true;
        /// <summary>
        /// Can the thumbnail have transparent/semi-transparent pixels?
        /// </summary>
        public virtual bool allowTransparentThumbnails => true;
        /// <summary>
        /// Required ratio of pixels in the thumbnail that need to be (fully) transparent to pass validation. A value of 0 disables this validation rule.
        /// Not applicable if transparent pixels are not allowed.
        /// </summary>
        public virtual float thumbnailMinTransparentBgRatio => 0f;

        /// <summary>
        /// If true, then Addressables can be built from this package.
        /// </summary>
        public virtual bool supportsAddressables => false;

        /// <summary>
        /// Space or SpaceTemplate
        /// </summary>
        public bool isSpaceBasedPackage => packageType == PackageType.Space || packageType == PackageType.SpaceTemplate;

        /// <summary>
        /// A collection of all top level assets that are bundled within this package.
        /// </summary>
        public abstract IEnumerable<Object> assets { get; }

        /// <summary>
        /// A collection of all game objects contained within `assets`
        /// </summary>
        public IEnumerable<GameObject> gameObjectAssets
        {
            get
            {
                foreach (Object asset in assets)
                {
                    if (asset is GameObject go)
                    {
                        yield return go;
                    }
                    else if (asset is Component comp)
                    {
                        yield return comp.gameObject;
                    }
                }
            }
        }

        /// <summary>
        /// Is this asset used within this package?
        /// </summary>
        public bool ContainsAsset(Object target)
        {
            foreach (Object asset in assets)
            {
                if (asset == target)
                    return true;
            }
            return false;
        }
    }
}
