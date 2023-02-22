using UnityEngine;

namespace SpatialSys.UnitySDK
{
    public class SpatialAvatarAnimation : SpatialComponentBase
    {
        public override string prettyName => "Avatar Animation";
        public override string tooltip => "This component is used to define a custom avatar animation for Spatial";
        public override string documentationURL => null;

        public Animator animator => GetComponent<Animator>();

        public AnimationClip targetClip;
    }
}