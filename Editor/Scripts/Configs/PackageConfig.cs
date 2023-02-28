using UnityEngine;
using System.Collections.Generic;

namespace SpatialSys.UnitySDK.Editor
{
    // Sync any changes here with the corresponding enum in the Spatial API
    public enum PackageType
    {
        Environment = 0,
        Avatar = 1,
        AvatarAnimation = 2,
        PrefabObject = 3,
    }

    /// <summary>
    /// Base class for all package config types (Environment, Avatar, etc.)
    /// </summary>
    public abstract class PackageConfig : ScriptableObject
    {
#pragma warning disable 414 // suppress "never assigned" warnings until we use it
        [HideInInspector]
        public int version; // version of this config model; Used for making backwards-compatible changes
#pragma warning restore 414

        [HideInInspector]
        public string sku = "";
        [Tooltip("Display name of the asset as it would show up in Spatial")]
        public string packageName;
        public Texture2D thumbnail = null;

        public abstract PackageType packageType { get; }
        public abstract Vector2Int thumbnailDimensions { get; }
        public abstract string bundleName { get; }

        /// <summary>
        /// A collection of all assets (excluding their dependencies) that this package uses.
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
        /// Is this asset used within this package
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
