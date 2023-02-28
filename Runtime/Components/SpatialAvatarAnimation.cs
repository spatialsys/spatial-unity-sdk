using UnityEngine;

namespace SpatialSys.UnitySDK
{
    public class SpatialAvatarAnimation : SpatialPackageAsset
    {
        public override string prettyName => "Avatar Animation";
        public override string tooltip => "This component is used to define a custom avatar animation for Spatial";
        public override string documentationURL => "https://spatialxr.notion.site/Custom-Avatar-Animations-1a91550f8c4f4975b8c6e785aed35a91";

        public Animator animator => GetComponent<Animator>();

        public AnimationClip targetClip;
    }
}