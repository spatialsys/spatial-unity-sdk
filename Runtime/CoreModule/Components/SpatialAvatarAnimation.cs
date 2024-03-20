using UnityEngine;

namespace SpatialSys.UnitySDK
{
    [DocumentationCategory("Components")]
    public class SpatialAvatarAnimation : SpatialPackageAsset
    {
        public override string prettyName => "Avatar Animation";
        public override string tooltip => "This component is used to define a custom avatar animation for Spatial";
        public override string documentationURL => "https://docs.spatial.io/components/custom-avatar-animations";

        public Animator animator => GetComponent<Animator>();

        public AnimationClip targetClip;
    }
}