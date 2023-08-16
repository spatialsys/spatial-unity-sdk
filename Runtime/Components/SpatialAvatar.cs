using UnityEngine;

namespace SpatialSys.UnitySDK
{
    [RequireComponent(typeof(Animator))]
    public class SpatialAvatar : SpatialPackageAsset
    {
        public override string prettyName => "Custom Avatar";
        public override string tooltip => "This component is used to define a custom avatar for Spatial";
        public override string documentationURL => "https://docs.spatial.io/components/custom-avatars";

        [Tooltip("The default animation set to use for this avatar. The difference is usually the walk and sitting animations.")]
        public SpatialAvatarDefaultAnimSetType defaultAnimSetType;

        [Tooltip("Optionally override specific animations for this avatar")]
        public SpatialAvatarAnimOverrides animOverrides;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            // Root level transform should always be identity
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }
#endif
    }

    public enum SpatialAvatarDefaultAnimSetType
    {
        Unset = 0,
        Feminine = 1,
        Masculine = 2
    }

    [System.Serializable]
    public class SpatialAvatarAnimOverrides
    {
        public AnimationClip idle;
        public AnimationClip walk;
        public AnimationClip jog;
        public AnimationClip run;

        [Tooltip("The start of the jump animation in the case of a standing jump")]
        public AnimationClip jumpStartIdle;
        [Tooltip("The start of the jump animation in the case of a moving jump")]
        public AnimationClip jumpStartMoving;
        [Tooltip("The 'in air' part of the jump animation")]
        public AnimationClip jumpInAir;
        [Tooltip("The land part of the jump animation, when the avatar is standing")]
        public AnimationClip jumpLandStanding;
        [Tooltip("The land part of the jump animation, when the avatar is walking")]
        public AnimationClip jumpLandWalking;
        [Tooltip("The land part of the jump animation, when the avatar is running")]
        public AnimationClip jumpLandRunning;
        [Tooltip("The land part of the jump animation, when the avatar is landing from a high jump")]
        public AnimationClip jumpLandHigh;
        [Tooltip("The double jump animation")]
        public AnimationClip jumpMultiple;

        [Tooltip("The fall animation when the avatar is falling from very high up")]
        public AnimationClip fall;

        public AnimationClip sit;

        [Tooltip("The climb animation when the avatar is not moving")]
        public AnimationClip climbIdle;
        [Tooltip("The climb animation when the avatar is moving up")]
        public AnimationClip climbUp;
        [Tooltip("The end of the climb animation when the avatar climbs to the top")]
        public AnimationClip climbEndTop;
    }
}