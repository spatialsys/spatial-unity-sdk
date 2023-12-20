using System;
using UnityEngine;

namespace SpatialSys.UnitySDK.EditorSimulation
{
    public class EditorReadOnlyAvatar : IReadOnlyAvatar
    {
        public bool isBodyLoaded => false;

        public string displayName => string.Empty;
        public string nametagSubtext => string.Empty;
        public float nametagBarValue => 0f;
        public bool nametagBarVisible => false;

        public Vector3 position => Vector3.zero;
        public Quaternion rotation => Quaternion.identity;
        public Vector3 velocity => Vector3.zero;

        public Material[] bodyMaterials => null;

#pragma warning disable 0067 // Disable "event is never used" warning
        public event Action onAvatarLoadComplete;
        public event Action onAvatarBeforeUnload;
#pragma warning restore 0067

        public Transform GetAvatarBoneTransform(HumanBodyBones humanBone) => null;
    }
}
