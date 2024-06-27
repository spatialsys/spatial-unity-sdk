using System;
using UnityEngine;

namespace SpatialSys.UnitySDK
{
    [DocumentationCategory("Services/Actor Service")]
    public interface IReadOnlyAvatar : IReadOnlySpaceObjectComponent, IEquatable<IReadOnlyAvatar>, IEquatable<IAvatar>
    {
        /// <summary>
        /// Whether the avatar body is fully loaded.
        /// When users join a space, their avatar body is not immediately loaded. This property will be false until
        /// the avatar body is loaded. It's also possible for users to switch their avatars on the fly, so this
        /// property may change at any time.
        /// Use the <see cref="onAvatarLoadComplete"/> and <see cref="onAvatarBeforeUnload"/> event to be notified
        /// when the avatar body is loaded or unloaded.
        /// </summary>
        bool isBodyLoaded { get; }

        /// <summary>
        /// Whether the avatar is visible in the scene.
        /// </summary>
        bool visibleLocally { get; set; }

        /// <summary>
        /// Whether the avatar is visible in the scene to other remote users.
        /// </summary>
        bool visibleRemotely { get; }

        /// <summary>
        /// The display name shown in the nametag above the avatar's head.
        /// </summary>
        string displayName { get; }

        /// <summary>
        /// Whether the nametag should be visible to other users.
        /// </summary>
        bool nametagVisible { get; }

        /// <summary>
        /// An optional subtext shown in the nametag above the avatar's head.
        /// </summary>
        string nametagSubtext { get; }

        /// <summary>
        /// Optional nametag bar value shown in the nametag above the avatar's head.
        /// </summary>
        float nametagBarValue { get; }

        /// <summary>
        /// Whether the nametag bar is visible.
        /// </summary>
        bool nametagBarVisible { get; }

        /// <summary>
        /// The current position of the avatar's visual representation in the scene. This is the position of the avatar
        /// at the feet.
        /// </summary>
        /// <returns>The current visual position of the avatar</returns>
        Vector3 position { get; }

        /// <summary>
        /// The orientation of the avatar's visual representation in the scene.
        /// Currently this is always locked to Y-up.
        /// </summary>
        /// <returns>The current visual orientation of the avatar</returns>
        Quaternion rotation { get; }

        /// <summary>
        /// The current velocity of the avatar
        /// </summary>
        /// <returns>The current velocity of the avatar</returns>
        Vector3 velocity { get; }

        /// <summary>
        /// Whether the avatar is currently grounded (the feet are touching the ground)
        /// </summary>
        bool isGrounded { get; }

        /// <summary>
        /// Contribution of how much ground friction to apply to the character. A higher value will give the avatar
        /// more grip resulting in higher acceleration. This does not affect the avatar's maximum movement speed.
        /// Values should be between 0 and 1.
        /// </summary>
        float groundFriction { get; }

        /// <summary>
        /// The walking speed of the avatar in meters per second.
        /// </summary>
        float walkSpeed { get; }

        /// <summary>
        /// The running speed of the avatar in meters per second.
        /// </summary>
        float runSpeed { get; }

        /// <summary>
        /// The height in meters that the avatar can jump
        /// </summary>
        float jumpHeight { get; }

        /// <summary>
        /// Maximum number of times that the avatar can jump in a row before touching the ground.
        /// </summary>
        int maxJumpCount { get; }

        /// <summary>
        /// How much control the player has over the character while in the air.
        /// A value of 0 means no control, 1 means full control.
        /// </summary>
        float airControl { get; }

        /// <summary>
        /// When enabled, jump is higher depending on how long jump button is held down.
        /// Currently variable jump height may result in a slightly higher <see cref="jumpHeight"/>
        /// </summary>
        bool useVariableHeightJump { get; }

        /// <summary>
        /// Multiplier on top of the default gravity settings for the space just for the local avatar.
        /// </summary>
        float gravityMultiplier { get; }

        /// <summary>
        /// Multiplier on top of the default gravity settings for the space just for the local avatar while falling.
        /// This stacks on top of <see cref="gravityMultiplier"/>. This is useful for making the avatar fall faster
        /// than they jump.
        /// </summary>
        float fallingGravityMultiplier { get; }

        /// <summary>
        /// Is ragdoll physics currently active for the avatar?
        /// </summary>
        bool ragdollPhysicsActive { get; }

        /// <summary>
        /// The current velocity of the avatar's ragdoll body.
        /// </summary>
        Vector3 ragdollVelocity { get; }

        /// <summary>
        /// Returns a collection of all materials used on the avatar's body. This can be used to change the appearance
        /// of the avatar.
        /// </summary>
        /// <returns>All materials for the avatar's sub meshes, or null if <see cref="isBodyLoaded"/> is false</returns>
        Material[] bodyMaterials { get; }

        /// <summary>
        /// Event that triggers when the avatar has finished loading.
        /// <see cref="isBodyLoaded"/> will be set to true before this event is triggered.
        /// </summary>
        event Action onAvatarLoadComplete;

        /// <summary>
        /// Event that triggers when the avatar is about to be unloaded. If the avatar is owned by an actor, this
        /// may be because the actor has changed their avatar, or because the actor is leaving or disconnecting.
        /// This can be used to "deconstruct" anything that was created in the <see cref="onAvatarLoadComplete"/> event.
        /// <see cref="isBodyLoaded"/> will be set to false after this event is triggered.
        /// </summary>
        event Action onAvatarBeforeUnload;

        /// <summary>
        /// Event that triggers when the avatar is respawned. Respawn can happen when:
        /// - A user's avatar enters a space and is spawned for the first time
        /// - An avatar is explicitly respawned with <see cref="IAvatar.Respawn"/>
        /// - An avatar goes out of bounds
        /// 
        /// For avatars created explicitly through <see cref="ISpaceContentService.SpawnAvatar"/> this event will not trigger
        /// </summary>
        event OnRespawnedDelegate onRespawned;
        public delegate void OnRespawnedDelegate(AvatarRespawnEventArgs args);

        /// <summary>
        /// Event that is triggered when the avatar's collider hits another collider.
        /// </summary>
        event OnColliderHitDelegate onColliderHit;
        public delegate void OnColliderHitDelegate(ControllerColliderHit hit, Vector3 avatarVelocity);

        /// <summary>
        /// Event that is triggered when the avatar's grounded state changes (<see cref="isGrounded"/>)
        /// </summary>
        event OnIsGroundedChangedDelegate onIsGroundedChanged;
        public delegate void OnIsGroundedChangedDelegate(bool isGrounded);

        /// <summary>
        /// Event that is triggered when the avatar starts to jump
        /// </summary>
        event OnJumpDelegate onJump;
        public delegate void OnJumpDelegate(bool isGrounded);

        /// <summary>
        /// Event that is triggered when the avatar lands on the ground
        /// </summary>
        event Action onLanded;

        /// <summary>
        /// Event that is triggered when an emote avatar animation is started.
        /// Note that this doesn't trigger immediately when <see cref="PlayEmote"/> is called, but when the animation
        /// is loaded (asynchronously) and started.
        /// </summary>
        event Action onEmote;

        /// <summary>
        /// Get the transform of a bone in the avatar's body.
        /// </summary>
        /// <returns>The transform of the bone if it can be found. Returns null if <see cref="isBodyLoaded"/> is false</returns>
        /// <example><code source="Services/ActorExamples.cs" region="GetAvatarBoneTransform" lang="csharp"/></example>
        Transform GetAvatarBoneTransform(HumanBodyBones humanBone);
    }

    [DocumentationCategory("Services/Actor Service")]
    public struct AvatarRespawnEventArgs
    {
        /// <summary>
        /// Whether this is the first time the avatar has spawned in the space.
        /// </summary>
        public bool isFirstSpawn;
    }
}
