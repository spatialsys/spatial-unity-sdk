using System.Collections.Generic;
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

        private Dictionary<AvatarAnimationClipType, AnimationClip> _lookup;
        public IReadOnlyDictionary<AvatarAnimationClipType, AnimationClip> lookup
        {
            get
            {
                if (_lookup == null)
                {
                    _lookup = new Dictionary<AvatarAnimationClipType, AnimationClip>();
                    foreach (var (type, clip) in AllOverrideClips())
                        _lookup[type] = clip;
                }
                return _lookup;
            }
        }

        /// <summary>
        /// Enumerate over all override animation clips
        /// </summary>
        /// <returns>IEnumerable<(serializedPropertyName, config)></returns>
        public IEnumerable<(AvatarAnimationClipType, AnimationClip)> AllOverrideClips()
        {
            if (idle != null)
                yield return (AvatarAnimationClipType.Idle, idle);
            if (walk != null)
                yield return (AvatarAnimationClipType.Walk, walk);
            if (jog != null)
                yield return (AvatarAnimationClipType.Jog, jog);
            if (run != null)
                yield return (AvatarAnimationClipType.Run, run);

            if (jumpStartIdle != null)
                yield return (AvatarAnimationClipType.JumpStartIdle, jumpStartIdle);
            if (jumpStartMoving != null)
                yield return (AvatarAnimationClipType.JumpStartMoving, jumpStartMoving);
            if (jumpInAir != null)
                yield return (AvatarAnimationClipType.JumpInAir, jumpInAir);
            if (jumpLandStanding != null)
                yield return (AvatarAnimationClipType.JumpLandStanding, jumpLandStanding);
            if (jumpLandWalking != null)
                yield return (AvatarAnimationClipType.JumpLandWalking, jumpLandWalking);
            if (jumpLandRunning != null)
                yield return (AvatarAnimationClipType.JumpLandRunning, jumpLandRunning);
            if (jumpLandHigh != null)
                yield return (AvatarAnimationClipType.JumpLandHigh, jumpLandHigh);
            if (jumpMultiple != null)
                yield return (AvatarAnimationClipType.JumpMultiple, jumpMultiple);

            if (fall != null)
                yield return (AvatarAnimationClipType.Fall, fall);

            if (sit != null)
                yield return (AvatarAnimationClipType.Sit, sit);

            if (climbIdle != null)
                yield return (AvatarAnimationClipType.ClimbIdle, climbIdle);
            if (climbUp != null)
                yield return (AvatarAnimationClipType.ClimbUp, climbUp);
            if (climbEndTop != null)
                yield return (AvatarAnimationClipType.ClimbEndTop, climbEndTop);
        }
    }

    /// <summary>
    /// Defines all the animation clips that are part of the avatar animation controller
    /// </summary>
    public enum AvatarAnimationClipType
    {
        Idle,
        Walk,
        Jog,
        Run,

        JumpStartIdle,
        JumpStartMoving,
        JumpInAir,
        JumpLandStanding,
        JumpLandWalking,
        JumpLandRunning,
        JumpLandHigh,
        JumpMultiple,

        Fall,

        Sit,

        Emote, // Not overridable

        ClimbIdle,
        ClimbUp,
        ClimbEndTop,

        // Always last
        Count
    }
}