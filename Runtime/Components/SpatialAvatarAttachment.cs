using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialSys.UnitySDK
{
    public class SpatialAvatarAttachment : SpatialPackageAsset
    {
        public enum Slot
        {
            None = 0,
            [Tooltip("Effects surrounding the avatar, such as a halo")]
            Aura = 1,
            LeftHand = 2,
            RightHand = 3,
            Feet = 4,
            Head = 5,
            Torso = 6,
            [Tooltip("For items that are worn on the avatar's back, separated from the torso, such as a backpack or jetpack")]
            Back = 7,
            [Tooltip("A special slot to set an active pet that follows the avatar")]
            Pet = 8,
            [Tooltip("Additional miscellaneous slot that doesn't fit anywhere else. This can be for belts, slings, rings, necklaces, etc.")]
            Accessory = 9,
            // Note: if you add more slots, you'll need to update the SlotMask enum below
        }

        [Flags]
        public enum SlotMask
        {
            None = 0,
            Aura = 1 << 0,
            LeftHand = 1 << 1,
            RightHand = 1 << 2,
            Feet = 1 << 3,
            Head = 1 << 4,
            Torso = 1 << 5,
            Back = 1 << 6,
            Pet = 1 << 7,
            Accessory = 1 << 8,
        }

        public enum Category
        {
            [Tooltip("The primary slot type will be used as the category when applicable")]
            Unspecified = 0,
            [Tooltip("Auras are typically used for effects that surround the avatar, such as a halo")]
            Aura = 1,
            [Tooltip("Things like swords, shields, etc")]
            Weapon = 2,
            [Tooltip("Things like skateboards, hoverboards, jetpacks, wheelchairs, rollerblades, magic carpets, etc")]
            Rideable = 3,
        }

        public enum AttachmentAnimatorType
        {
            None = 0,
            [Tooltip("The attachment will use its own animator, with the same animator controller as the avatar, but with different animations; Typically used for attachments that correspond to the avatar's animations")]
            Standard = 1,
            [Tooltip("The attachment will use a custom animator; Typically used for attachments that have their own animations that don't correspond to the avatar's animations")]
            Custom = 2,
        }

        public override string prettyName => "Avatar Attachment";
        public override string tooltip => "This component is used to define an object that can be attached or equipped by an avatar";
        public override string documentationURL => "https://docs.spatial.io/avatar-attachment";

        private const int LATEST_VERSION = 1;
        [HideInInspector]
        public int version = 0;

        [Tooltip("The main slot that the attachment will be equipped to")]
        public Slot primarySlot = Slot.None;
        [Tooltip("Additional slots that the attachment needs; For example, a sword might need a 'right hand' slot and a 'left hand' slot")]
        public SlotMask additionalSlots; // FIXME: for now, lets not allow any additional slots for Aura attachments

        [Tooltip("The category is used to group attachments together in various menus")]
        public Category category = Category.Unspecified;

        [Tooltip("The attachment is skinned to the humanoid skeleton of the avatar. This is useful for parts that need to bend with the avatar's body, such as armor.")]
        public bool isSkinnedToHumanoidSkeleton = false;

        [Tooltip("When enabled, the attachment will be parented to the avatar's bone specified below")]
        public bool attachToBone = false;
        public HumanBodyBones attachBoneTarget;
        public Vector3 attachBoneOffset;
        public Quaternion attachBoneRotationOffset;

        // IK target settings
        [Tooltip("IK targets can be useful for two handed weapons, for example to make sure that the avatar's hands are always on the weapon's handle")]
        public bool ikTargetsEnabled = false;
        public Transform ikLeftHandTarget;
        public Transform ikRightHandTarget;
        public Transform ikLeftFootTarget;
        public Transform ikRightFootTarget;

        // Supports custom actions
        [Tooltip("Does this attachment support custom actions? Such as an 'Attack' action for a sword")]
        public bool customActionsEnabled = false;
        [Tooltip("The number of custom actions that this attachment supports; Editing this value will add or remove the custom action slots")]
        public int customActionsCount = 0;

        [Tooltip("When enabled, and the attachment is equipped, the avatar's animations will be overridden with the ones specified below")]
        public bool overrideAvatarAnimations = false;
        [Tooltip("Which parts of the avatar body should these override animations apply to?")]
        public SpatialAttachmentAvatarAnimSettings avatarAnimSettings;

        [Tooltip("Specify the attachment's animator type; If your attachment has an animator component, you can use it to animate the attachment either individually or in-sync with avatar animations.")]
        public AttachmentAnimatorType attachmentAnimatorType = AttachmentAnimatorType.None;
        [Tooltip("When the animatorType is set to 'Standard', the attachment will play these animations in-sync with the avatar's animations")]
        public AttachmentAnimationClips attachmentAnimClips;

        public SlotMask occupiedSlots => primarySlot.ToSlotMask() | additionalSlots;
        public bool skinningFeatureAvailable => false; //true; // TODO: when skinning feature is implemented uncomment this
        public bool attachToBoneFeatureAvailable => !isSkinnedToHumanoidSkeleton;
        public bool ikFeatureAvailable => false; //primarySlot != Slot.Aura && !isSkinnedToHumanoidSkeleton; // TODO: when IK feature is implemented uncomment this
        public bool customActionsFeatureAvailable => false; //primarySlot != Slot.Aura; // TODO: when custom actions feature is implemented uncomment this
        public bool ikFeatureActive
        {
            get
            {
                SpatialAvatarAttachment.SlotMask occupied = occupiedSlots;
                return ikTargetsEnabled && ikFeatureAvailable &&
                    (
                        occupied.HasFlag(Slot.LeftHand.ToSlotMask()) && ikLeftHandTarget != null ||
                        occupied.HasFlag(Slot.RightHand.ToSlotMask()) && ikRightHandTarget != null ||
                        occupied.HasFlag(Slot.Feet.ToSlotMask()) && (ikLeftFootTarget != null || ikRightFootTarget != null)
                    );
            }
        }
        public bool animatorFeatureAvailable => !isSkinnedToHumanoidSkeleton;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

#if UNITY_EDITOR
            // Root level transform should always be identity
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
#endif

            UpgradeDataIfNecessary();

            // Only grow so that we don't lose data as the user changes the value
            int customActionCountActual = (customActionsFeatureAvailable && customActionsEnabled) ? customActionsCount : 0;
            if (attachmentAnimClips?.customActions != null && attachmentAnimClips.customActions.Length != customActionCountActual)
                Array.Resize(ref attachmentAnimClips.customActions, customActionCountActual);
            if (avatarAnimSettings?.customActions != null && avatarAnimSettings.customActions.Length != customActionCountActual)
                Array.Resize(ref avatarAnimSettings.customActions, customActionCountActual);
        }

        public void UpgradeDataIfNecessary()
        {
            if (version == LATEST_VERSION)
                return;

            // Version 0 was the initial version; Converting to V1 is just about setting good defaults
            if (version == 0)
            {
                // Good defaults when the component is just added
                avatarAnimSettings = new SpatialAttachmentAvatarAnimSettings();
                avatarAnimSettings.sit = new AttachmentAvatarAnimConfig();
                avatarAnimSettings.sit.attachmentVisible = false;
                avatarAnimSettings.sit.disableIK = true;
                avatarAnimSettings.emote = new AttachmentAvatarAnimConfig();
                avatarAnimSettings.emote.attachmentVisible = false;
                avatarAnimSettings.emote.disableIK = true;

                version = 1;
            }

            // Add future upgrade paths here
        }
#endif
    }

    public static class SpatialAvatarAttachmentSlotMaskExtensions
    {
        public static SpatialAvatarAttachment.SlotMask ToSlotMask(this SpatialAvatarAttachment.Slot slot)
        {
            return (SpatialAvatarAttachment.SlotMask)(1 << ((int)slot) - 1);
        }
    }

    [System.Serializable]
    public class SpatialAttachmentAvatarAnimSettings
    {
        public AttachmentAvatarAnimConfig idle;
        public AttachmentAvatarAnimConfig walk;
        public AttachmentAvatarAnimConfig jog;
        public AttachmentAvatarAnimConfig run;

        [Tooltip("The start of the jump animation in the case of a standing jump")]
        public AttachmentAvatarAnimConfig jumpStartIdle;
        [Tooltip("The start of the jump animation in the case of a moving jump")]
        public AttachmentAvatarAnimConfig jumpStartMoving;
        [Tooltip("The 'in air' part of the jump animation")]
        public AttachmentAvatarAnimConfig jumpInAir;
        [Tooltip("The land part of the jump animation, when the avatar is standing")]
        public AttachmentAvatarAnimConfig jumpLandStanding;
        [Tooltip("The land part of the jump animation, when the avatar is walking")]
        public AttachmentAvatarAnimConfig jumpLandWalking;
        [Tooltip("The land part of the jump animation, when the avatar is running")]
        public AttachmentAvatarAnimConfig jumpLandRunning;
        [Tooltip("The land part of the jump animation, when the avatar is landing from a high jump")]
        public AttachmentAvatarAnimConfig jumpLandHigh;
        [Tooltip("The double jump animation")]
        public AttachmentAvatarAnimConfig jumpMultiple;

        [Tooltip("The fall animation when the avatar is falling from very high up")]
        public AttachmentAvatarAnimConfig fall;

        [Tooltip("This sit avatar animation; This can be overridden by other systems")]
        public AttachmentAvatarAnimConfig sit;

        [Tooltip("The custom reactions and emote animations that the avatar can play")]
        public AttachmentAvatarAnimConfig emote;

        [Tooltip("Some attachments may give the user additional abilities, such as a Sword attachment has an 'Attack' action. This is where you can specify the animation that the avatar should play for that action.")]
        public AttachmentAvatarAnimConfig[] customActions;

        /// <summary>
        /// Enumerate over all settings excluding custom actions
        /// </summary>
        /// <returns>IEnumerable<(serializedPropertyName, config)></returns>
        public IEnumerable<(string, AttachmentAvatarAnimConfig)> AllSettings()
        {
            yield return (nameof(idle), idle);
            yield return (nameof(walk), walk);
            yield return (nameof(jog), jog);
            yield return (nameof(run), run);
            yield return (nameof(jumpStartIdle), jumpStartIdle);
            yield return (nameof(jumpStartMoving), jumpStartMoving);
            yield return (nameof(jumpInAir), jumpInAir);
            yield return (nameof(jumpLandStanding), jumpLandStanding);
            yield return (nameof(jumpLandWalking), jumpLandWalking);
            yield return (nameof(jumpLandRunning), jumpLandRunning);
            yield return (nameof(jumpLandHigh), jumpLandHigh);
            yield return (nameof(jumpMultiple), jumpMultiple);
            yield return (nameof(fall), fall);
            yield return (nameof(sit), sit);
            yield return (nameof(emote), emote);
        }
    }

    [System.Serializable]
    public class AttachmentAvatarAnimConfig
    {
        [Tooltip("Optional animation clip to override the avatar's animation with. For example, running with a gun equipped should look different.")]
        public AnimationClip overrideClip;
        [Tooltip("Optional animation clip override specifically for male avatars.")]
        public AnimationClip overrideClipMale;

        [Tooltip("Should the attachment be rendered for this animation clip?")]
        public bool attachmentVisible = true;

        [Tooltip("If there are any IK settings for this attachment, disable them for this animation clip")]
        public bool disableIK = false;
    }

    [System.Serializable]
    public class AttachmentAnimationClips : SpatialAvatarAnimOverrides
    {
        // TODO: validate that clips have the same length as the avatar clips

        [Tooltip("Some attachments may give the user addition abilities, such as a Sword attachment has an 'Attack' action. This is where you can specify the animation that the attachment animator should play for that action.")]
        public AnimationClip[] customActions;
    }
}