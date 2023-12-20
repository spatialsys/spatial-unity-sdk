using System;
using UnityEngine;

namespace SpatialSys.UnitySDK
{
    public interface IAvatar : IReadOnlyAvatar
    {
        /// <summary>
        /// An optional subtext shown in the nametag above the avatar head.
        /// </summary>
        new string nametagSubtext { get; set; }

        /// <summary>
        /// Optional nametag bar value shown in the nametag above the avatar head.
        /// </summary>
        new float nametagBarValue { get; set; }

        /// <summary>
        /// Whether the nametag bar is visible.
        /// </summary>
        new bool nametagBarVisible { get; set; }

        /// <summary>
        /// The current position of the avatar's visual representation in the scene. This is the position of the avatar
        /// at the feet.
        /// </summary>
        /// <returns>The current visual position of the avatar</returns>
        new Vector3 position { get; set; }

        /// <summary>
        /// The orientation of the avatar's visual representation in the scene.
        /// </summary>
        /// <returns>The current visual orientation of the avatar</returns>
        new Quaternion rotation { get; set; }

        /// <summary>
        /// The current velocity of the avatar
        /// </summary>
        /// <returns>The current velocity of the avatar</returns>
        new Vector3 velocity { get; set; }

        /// <summary>
        /// Whether the avatar is currently grounded (the feet are touching the ground)
        /// </summary>
        bool isGrounded { get; }

        /// <summary>
        /// Contribution of how much ground friction to apply to the character. A higher value will give the avatar
        /// more grip resulting in higher acceleration. This does not affect the avatar's maximum movement speed.
        /// Values should be between 0 and 1.
        /// </summary>
        float groundFriction { get; set; }

        /// <summary>
        /// The walking speed of the avatar in meters per second.
        /// </summary>
        float walkSpeed { get; set; }

        /// <summary>
        /// The running speed of the avatar in meters per second.
        /// </summary>
        float runSpeed { get; set; }

        /// <summary>
        /// The height in meters that the avatar can jump
        /// </summary>
        float jumpHeight { get; set; }

        /// <summary>
        /// Maximum number of times that the avatar can jump in a row before touching the ground.
        /// </summary>
        int maxJumpCount { get; set; }

        /// <summary>
        /// How much control the player has over the character while in the air.
        /// A value of 0 means no control, 1 means full control.
        /// </summary>
        float airControl { get; set; }

        /// <summary>
        /// When enabled, jump is higher depending on how long jump button is held down.
        /// Currently variable jump height may result in a slightly higher <see cref="jumpHeight"/>
        /// </summary>
        bool useVariableHeightJump { get; set; }

        /// <summary>
        /// Multiplier on top of the default gravity settings for the space just for the local avatar.
        /// </summary>
        float gravityMultiplier { get; set; }

        /// <summary>
        /// Multiplier on top of the default gravity settings for the space just for the local avatar while falling.
        /// This stacks on top of <see cref="gravityMultiplier"/>. This is useful for making the avatar fall faster
        /// than they jump.
        /// </summary>
        float fallingGravityMultiplier { get; set; }

        /// <summary>
        /// Is ragdoll physics currently active for the avatar?
        /// </summary>
        bool ragdollPhysicsActive { get; }

        /// <summary>
        /// The current velocity of the avatar's ragdoll body.
        /// </summary>
        Vector3 ragdollVelocity { get; set; }

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
        /// Event that is triggered when an avatar attachment is equipped or unequipped.
        /// </summary>
        event OnAttachmentEquippedChangedDelegate onAttachmentEquippedChanged;
        public delegate void OnAttachmentEquippedChangedDelegate(string assetID, bool equipped);

        /// <summary>
        /// Teleport the avatar to a new position and rotation.
        /// This is faster than setting the position and rotation separately.
        /// </summary>
        void SetPositionRotation(Vector3 position, Quaternion rotation);

        /// <summary>
        /// Move the avatar in the direction of the input vector.
        /// </summary>
        /// <param name="input">Input vector ranging -1 to +1 on x and y</param>
        /// <param name="sprint">Uses <see cref="runSpeed"/> instead of <see cref="walkSpeed"/></param>
        void Move(Vector2 input, bool sprint = false);

        /// <summary>
        /// Add a force to the avatar's rigidbody. This is different from ragdoll force, and just affects the avatar's
        /// character controller.
        /// </summary>
        void AddForce(Vector3 force);

        /// <summary>
        /// Teleports the avatar to the target transform and makes the avatar sit on the target.
        /// </summary>
        void Sit(Transform target);

        /// <summary>
        /// If currently sitting, this will make the avatar stand up.
        /// </summary>
        void Stand();

        /// <summary>
        /// If possible, make the avatar jump. Sometimes the avatar is not able to jump, such as when they are not
        /// grounded if <see cref="maxJumpCount"/> is exceeded.
        /// </summary>
        void Jump();

        /// <summary>
        /// Set ragdoll physics active state. This can only be done if ragdoll physics feature is enabled for the space.
        /// </summary>
        /// <param name="active">Active state of ragdoll</param>
        /// <param name="initialVelocity">When activated, how much should the initial velocity be</param>
        void SetRagdollPhysicsActive(bool active, Vector3 initialVelocity);

        /// <summary>
        /// Add a force to the avatar's ragdoll body. This will only work if ragdoll physics are enabled for the space
        /// and avatar. See <see cref="SetRagdollPhysicsActive"/>.
        /// </summary>
        /// <param name="force">Amount of force to apply</param>
        /// <param name="ignoreMass">Ignore the mass of the avatar (big avatars need more force to move)</param>
        void AddRagdollForce(Vector3 force, bool ignoreMass = false);

        /// <summary>
        /// Play an emote on the avatar (Defined by the <see cref="SpatialAvatarAnimation"/> component)
        /// </summary>
        /// <param name="assetType">Which asset type to play this from. For <see cref="AssetType.BackpackItem"/>, this will only work if the user owns that item</param>
        /// <param name="assetID">
        /// ID of the asset:
        /// - <see cref="AssetType.BackpackItem"/>: The itemID found in Spatial Studio
        /// - <see cref="AssetType.Package"/>: The packageSKU found in Spatial Studio or Unity
        /// - <see cref="AssetType.EmbeddedAsset"/>: The assetID specified in the <see cref="UnitySDK.Editor.SpaceConfig"/> in Unity Editor
        /// </param>
        /// <param name="immediately">Play the emote immidiately, without any transitions. Ignored when assetType is <see cref="AssetType.BackpackItem"/></param>
        /// <param name="loop">Loop the animation until it is stopped. Ignored when assetType is <see cref="AssetType.BackpackItem"/></param>
        void PlayEmote(AssetType assetType, string assetID, bool immediately = false, bool loop = false);

        /// <summary>
        /// Stop the current emote on the avatar that was started with <see cref="PlayEmote"/>
        /// </summary>
        void StopEmote();

        /// <summary>
        /// Set the avatar body to an avatar defined by <see cref="SpatialAvatar"/> component.
        /// This temporarily overrides the user's profile avatar just for this session, and can be reverted with
        /// <see cref="ResetAvatarBody"/>
        /// </summary>
        /// <param name="assetType">Which asset type to set this from. <see cref="AssetType.BackpackItem"/> is not supported here</param>
        /// <param name="assetID">
        /// ID of the asset:
        /// - <see cref="AssetType.Package"/>: The packageSKU found in Spatial Studio or Unity
        /// - <see cref="AssetType.EmbeddedAsset"/>: The assetID specified in the <see cref="UnitySDK.Editor.SpaceConfig"/> in Unity Editor
        /// </param>
        void SetAvatarBody(AssetType assetType, string assetID);

        /// <summary>
        /// Reset the avatar body asset to what the user has set in their profile if it was overridden with <see cref="SetAvatarBody"/>.
        /// </summary>
        void ResetAvatarBody();

        /// <summary>
        /// Equip an attachment to the avatar (Defined by the <see cref="SpatialAvatarAttachment"/> component)
        /// </summary>
        /// <param name="assetType">Which asset type to play this from. For <see cref="AssetType.BackpackItem"/>, this will only work if the user owns that item</param>
        /// <param name="assetID">
        /// ID of the asset:
        /// - <see cref="AssetType.BackpackItem"/>: The itemID found in Spatial Studio
        /// - <see cref="AssetType.Package"/>: The packageSKU found in Spatial Studio or Unity
        /// - <see cref="AssetType.EmbeddedAsset"/>: The assetID specified in the <see cref="UnitySDK.Editor.SpaceConfig"/> in Unity Editor
        /// </param>
        /// <param name="equip">Should it be equipped or not</param>
        /// <param name="clearOccupiedPrimarySlot">
        /// When equipping, should any conflicting primary slot be cleared, or should this equip additively on top of the
        /// existing attachments? Used when <paramref name="equip"/> is set to true
        /// </param>
        /// <param name="optionalTag">A string tag that can be used to reference some attachments. Used by <see cref="ClearAttachmentsByTag"/></param>
        EquipAttachmentRequest EquipAttachment(AssetType assetType, string assetID, bool equip = true, bool clearOccupiedPrimarySlot = true, string optionalTag = null);

        /// <summary>
        /// Clear all avatar attachments that were equipped with <see cref="EquipAttachment"/>.
        /// This does not affect the user's profile avatar, just the attachments that were equipped in this session.
        /// </summary>
        void ClearAttachments();

        /// <summary>
        /// Clear avatar attachments currently equipped in a specific slot.
        /// This does not affect the user's profile avatar, just the attachments that were equipped in this session.
        /// </summary>
        /// <param name="slot">The slot to clear</param>
        void ClearAttachmentSlot(SpatialAvatarAttachment.Slot slot);

        /// <summary>
        /// Clear avatar attachments currently equipped that have this tag set.
        /// This does not affect the user's profile avatar, just the attachments that were equipped in this session.
        /// </summary>
        /// <param name="tag">The tag that was set with <see cref="EquipAttachment"/></param>
        void ClearAttachmentsByTag(string tag);

        /// <summary>
        /// Is the attachment currently equipped?
        /// </summary>
        /// <param name="assetID">This can be itemID, packageSKU or embeddedAssetID</param>
        bool IsAttachmentEquipped(string assetID);
    }

    public class EquipAttachmentRequest : SpatialAsyncOperation
    {
        public bool succeeded;
    }
}
