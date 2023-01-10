using UnityEngine;

namespace SpatialSys.UnitySDK.Editor
{
    public enum PackageType
    {
        Environment = 0,
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

        public abstract PackageType packageType { get; }

        /// <summary>
        /// This this asset the main asset for the package (or any of its variants if it supports variants)
        /// </summary>
        public abstract bool IsMainAssetForPackage(Object asset);
    }
}
