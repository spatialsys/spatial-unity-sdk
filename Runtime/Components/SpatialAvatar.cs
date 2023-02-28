using UnityEngine;

namespace SpatialSys.UnitySDK
{
    [RequireComponent(typeof(Animator))]
    public class SpatialAvatar : SpatialPackageAsset
    {
        public override string prettyName => "Custom Avatar";
        public override string tooltip => "This component is used to define a custom avatar for Spatial";
        public override string documentationURL => "https://spatialxr.notion.site/Custom-Avatars-f11056b1f0984ac1a2b8c559e33ccbce";
    }
}