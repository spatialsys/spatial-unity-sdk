using System;
using UnityEngine;

namespace SpatialSys.UnitySDK.EditorSimulation
{
    public class EditorLocalAvatar : IAvatar
    {
        public bool isBodyLoaded { get; }
        public bool visibleLocally { get; set; } = true;
        public bool visibleRemotely { get; set; } = true;
        public string displayName { get; } = "Editor Local Avatar";
        public string nametagSubtext { get; set; }
        public float nametagBarValue { get; set; }
        public bool nametagBarVisible { get; set; }

        public Vector3 position { get; set; }
        public Quaternion rotation { get; set; }
        public Vector3 velocity { get; set; }

        public bool isGrounded { get; }
        public float groundFriction { get; set; } = 1f;
        public float walkSpeed { get; set; } = 3f;
        public float runSpeed { get; set; } = 6.875f;
        public float jumpHeight { get; set; } = 1.5f;
        public int maxJumpCount { get; set; } = 2;
        public float airControl { get; set; } = 1;
        public bool useVariableHeightJump { get; set; } = true;
        public float gravityMultiplier { get; set; } = 1.5f;
        public float fallingGravityMultiplier { get; set; } = 1f;
        public bool ragdollPhysicsActive => false;
        public Vector3 ragdollVelocity { get; set; } = Vector3.zero;
        public Material[] bodyMaterials => null;

#pragma warning disable 0067 // Disable "event is never used" warning
        public event Action onAvatarLoadComplete;
        public event Action onAvatarBeforeUnload;
        public event IAvatar.OnColliderHitDelegate onColliderHit;
        public event IAvatar.OnIsGroundedChangedDelegate onIsGroundedChanged;
        public event IAvatar.OnJumpDelegate onJump;
        public event Action onLanded;
        public event Action onEmote;
        public event IAvatar.OnAttachmentEquippedChangedDelegate onAttachmentEquippedChanged;
#pragma warning restore 0067

        public void SetPositionRotation(Vector3 position, Quaternion rotation) { }
        public void Move(Vector2 input, bool sprint = false) { }
        public void AddForce(Vector3 force) { }
        public void Jump() { }
        public void Sit(Transform target) { }
        public void Stand() { }

        public void SetRagdollPhysicsActive(bool active, Vector3 initialVelocity) { }
        public void AddRagdollForce(Vector3 force, bool ignoreMass = false) { }

        public void PlayEmote(AssetType assetType, string assetID, bool immediately = false, bool loop = false) { }
        public void StopEmote() { }

        public void SetAvatarBody(AssetType assetType, string assetID) { }
        public void ResetAvatarBody() { }
        public Transform GetAvatarBoneTransform(HumanBodyBones humanBone) => null;

        public EquipAttachmentRequest EquipAttachment(AssetType assetType, string assetID, bool equip = true, bool clearOccupiedPrimarySlot = true, string optionalTag = null)
        {
            EquipAttachmentRequest request = new()
            {
                succeeded = false,
            };
            request.InvokeCompletionEvent();
            return request;
        }
        public void ClearAttachments() { }
        public void ClearAttachmentSlot(SpatialAvatarAttachment.Slot slot) { }
        public void ClearAttachmentsByTag(string tag) { }
        public bool IsAttachmentEquipped(string assetID) => false;
    }
}
