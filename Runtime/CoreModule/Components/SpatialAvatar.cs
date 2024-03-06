using System.Collections.Generic;
using UnityEngine;

namespace SpatialSys.UnitySDK
{
    [RequireComponent(typeof(Animator))]
    [DocumentationCategory("Spatial Components")]
    public class SpatialAvatar : SpatialPackageAsset
    {
        public const float DEFAULT_CHARACTER_CONTROLLER_HEIGHT = 1.8f;
        public const float DEFAULT_CHARACTER_CONTROLLER_RADIUS = 0.28f;

        public override string prettyName => "Custom Avatar";
        public override string tooltip => "This component is used to define a custom avatar for Spatial";
        public override string documentationURL => "https://docs.spatial.io/components/custom-avatars";

        [Tooltip("The default animation set to use for this avatar. The difference is usually the walk and sitting animations.")]
        public SpatialAvatarDefaultAnimSetType defaultAnimSetType;

        [Tooltip("Optionally override specific animations for this avatar")]
        public SpatialAvatarAnimOverrides animOverrides;

        [Tooltip("The height (in meters) that the character controller is set to at runtime. Visualized as a cyan capsule in the scene view.")]
        [Clamp(0.01f, 25f)]
        public float characterControllerHeight = DEFAULT_CHARACTER_CONTROLLER_HEIGHT;

        [Tooltip("The radius (in meters) that the character controller is set to at runtime. It is recommended to encapsulate the main body without the limbs for better gameplay. Visualized as a cyan capsule in the scene view.")]
        [Clamp(0.005f, 12.5f)]
        public float characterControllerRadius = DEFAULT_CHARACTER_CONTROLLER_RADIUS;

        // Variables below are assigned automatically at publish.
        // These arrays will be either null or empty if there is no ragdoll setup found.
        [HideInInspector] public CharacterJoint[] ragdollJoints = null;
        [HideInInspector] public Collider[] ragdollColliders = null;
        [HideInInspector] public Rigidbody[] ragdollRigidbodies = null;

        public bool hasRagdollSetup => ragdollJoints?.Length > 0;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            // Root level transform should always be identity
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.matrix = Matrix4x4.Translate(transform.position);

            Color gizmoColor = Color.cyan;
            if (Application.isPlaying)
                gizmoColor.a *= 0.5f;
            Gizmos.color = gizmoColor;

            // Capsule cannot be squashed smaller than a sphere.
            float minHeight = characterControllerRadius * 2f;
            if (characterControllerHeight <= minHeight)
            {
                Gizmos.DrawWireSphere(Vector3.up * characterControllerRadius, characterControllerRadius);
                return;
            }

            // There's no Gizmos.DrawWireCapsule, so do it manually with Handles API...
            void DrawWireCapsule(Vector3 center, float height, float radius, Color color)
            {
                using (new UnityEditor.Handles.DrawingScope(color, Gizmos.matrix))
                {
                    float arcsCenterOffset = height * 0.5f - radius;
                    Vector3 topCenter = center + (Vector3.up * arcsCenterOffset);
                    Vector3 bottomCenter = center + (Vector3.down * arcsCenterOffset);

                    // Top hemisphere
                    UnityEditor.Handles.DrawWireArc(topCenter, normal: Vector3.forward, from: Vector3.right, angle: 180f, radius);
                    UnityEditor.Handles.DrawWireArc(topCenter, normal: Vector3.left, from: Vector3.forward, angle: 180f, radius);
                    UnityEditor.Handles.DrawWireDisc(topCenter, normal: Vector3.up, radius);

                    // Vertical lines
                    UnityEditor.Handles.DrawLine(topCenter + (Vector3.forward * radius), bottomCenter + (Vector3.forward * radius));
                    UnityEditor.Handles.DrawLine(topCenter + (Vector3.left * radius), bottomCenter + (Vector3.left * radius));
                    UnityEditor.Handles.DrawLine(topCenter + (Vector3.back * radius), bottomCenter + (Vector3.back * radius));
                    UnityEditor.Handles.DrawLine(topCenter + (Vector3.right * radius), bottomCenter + (Vector3.right * radius));

                    // Bottom hemisphere
                    UnityEditor.Handles.DrawWireArc(bottomCenter, normal: Vector3.forward, from: Vector3.left, angle: 180f, radius);
                    UnityEditor.Handles.DrawWireArc(bottomCenter, normal: Vector3.left, from: Vector3.back, angle: 180f, radius);
                    UnityEditor.Handles.DrawWireDisc(bottomCenter, normal: Vector3.down, radius);
                }
            }

            UnityEngine.Rendering.CompareFunction prevZTestMode = UnityEditor.Handles.zTest;

            UnityEditor.Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
            DrawWireCapsule(Vector3.up * characterControllerHeight * 0.5f, characterControllerHeight, characterControllerRadius, Gizmos.color);

            // Lower opacity when behind objects.
            UnityEditor.Handles.zTest = UnityEngine.Rendering.CompareFunction.Greater;
            Color zTestFailedColor = Gizmos.color;
            zTestFailedColor.a *= 0.2f;
            DrawWireCapsule(Vector3.up * characterControllerHeight * 0.5f, characterControllerHeight, characterControllerRadius, zTestFailedColor);

            UnityEditor.Handles.zTest = prevZTestMode;
        }
#endif
    }

    [DocumentationCategory("Spatial Components")]
    public enum SpatialAvatarDefaultAnimSetType
    {
        Unset = 0,
        Feminine = 1,
        Masculine = 2
    }

    [System.Serializable]
    [DocumentationCategory("Spatial Components")]
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
    [DocumentationCategory("Spatial Components")]
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