using System;
using UnityEngine;

namespace SpatialSys.UnitySDK
{
    public interface IReadOnlyAvatar
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
        /// The display name shown in the nametag above the avatar's head.
        /// </summary>
        string displayName { get; }

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
        Vector3 position { get; }

        /// <summary>
        /// The orientation of the avatar's visual representation in the scene.
        /// Currently this is always locked to Y-up.
        /// </summary>
        Quaternion rotation { get; }

        /// <summary>
        /// The current velocity of the avatar
        /// </summary>
        Vector3 velocity { get; }

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
        /// Get the transform of a bone in the avatar's body.
        /// </summary>
        /// <returns>The transform of the bone if it can be found. Returns null if <see cref="isBodyLoaded"/> is false</returns>
        Transform GetAvatarBoneTransform(HumanBodyBones humanBone);
    }
}
