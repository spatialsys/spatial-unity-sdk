using System;
using System.Collections.Generic;
using SpatialSys.UnitySDK.Internal;
using UnityEngine;

namespace SpatialSys.UnitySDK
{
    /// <summary>
    /// Service for interacting with a space's synchronized content.
    /// </summary>
    /// <example><code source="Services/SpaceContentServiceExamples.cs" region="Service Example"/></example>
    [DocumentationCategory("Services/Space Content Service")]
    public interface ISpaceContentService
    {
        /// <summary>
        /// A dictionary of all space objects currently in the space, keyed by ObjectID.
        /// </summary>
        IReadOnlyDictionary<int, IReadOnlySpaceObject> allObjects { get; }

        /// <summary>
        /// A dictionary of all avatar space object components in the space, keyed by ObjectID.
        /// </summary>
        IReadOnlyDictionary<int, IReadOnlyAvatar> avatars { get; }

        /// <summary>
        /// A dictionary of all prefab space object components in the space, keyed by ObjectID.
        /// </summary>
        IReadOnlyDictionary<int, IReadOnlyPrefabObject> prefabs { get; }

        /// <summary>
        /// A dictionary of all network objects in the space, keyed by ObjectID.
        /// </summary>
        IReadOnlyDictionary<int, SpatialNetworkObject> networkObjects { get; }

        /// <summary>
        /// True if the scene has been fully initialized.
        /// </summary>
        bool isSceneInitialized { get; }

        /// <summary>
        /// Event fired when the main space package has been fully initialized.
        /// </summary>
        event Action onSceneInitialized;

        /// <summary>
        /// Event fired when a space object is spawned.
        /// </summary>
        event Action<IReadOnlySpaceObject> onObjectSpawned;

        /// <summary>
        /// Event fired when a space object is destroyed. The returned object is disposed, meaning it can not be edited, but can still be read from.
        /// </summary>
        event Action<IReadOnlySpaceObject> onObjectDestroyed;

        /// <summary>
        /// Spawn a space object that doesn't have any visual representation.
        /// </summary>
        /// <returns><c>Async</c> Returns the created spaceObject once it has been registered with the space.</returns>
        SpawnSpaceObjectRequest SpawnSpaceObject();

        /// <summary>
        /// Spawn a space object that doesn't have any visual representation.
        /// </summary>
        /// <param name="position">Position of the object to instantiate.</param>
        /// <param name="rotation">Rotation of the object to instantiate.</param>
        /// <returns><c>Async</c> Returns the created spaceObject once it has been registered with the space.</returns>
        SpawnSpaceObjectRequest SpawnSpaceObject(Vector3 position, Quaternion rotation);

        /// <summary>
        /// Destroy the Space Object with the given ID if the local client has ownership.
        /// </summary>
        /// <returns><c>Async</c> Returns the request which will complete once the operation has been performed.</returns>
        DestroySpaceObjectRequest DestroySpaceObject(int objectID);

        /// <summary>
        /// Try to take ownership of a Space Object. This can fail if the object doesn't exist or if the ownership is
        /// fixed to another actor. Since the server can reject the request, the ownership change is not immediate.
        /// </summary>
        /// <param name="objectID">The space object to take ownership from</param>
        SpaceObjectOwnershipTransferRequest TakeOwnership(int objectID);

        /// <summary>
        /// Transfer the ownership of an object to another actor. This will only succeed if the local actor is the owner
        /// or <see cref="IReadOnlySpaceObject.hasControl"/> of the object.
        /// This will fail if:
        /// - The object doesn't exist
        /// - The new owner actor is invalid
        /// - <see cref="IReadOnlySpaceObject.flags"/> has <see cref="SpaceObjectFlags.AllowOwnershipTransfer"/> disabled
        /// - If the object is flagged as a <see cref="SpaceObjectFlags.MasterClientObject"/>
        /// </summary>
        /// <param name="objectID">The space object to transfer ownership of</param>
        /// <param name="newOwnerActor">The actor ID of the new owner</param>
        SpaceObjectOwnershipTransferRequest TransferOwnership(int objectID, int newOwnerActor);

        /// <summary>
        /// If the local actor is currently the owner of the object, this will release the ownership of the object
        /// and set the owner to 0. This will not destroy the object, but it will make it available for other actors
        /// to take ownership of.
        /// If succeeded the <see cref="SpaceObjectFlags.AllowOwnershipTransfer"/> flag will be enabled if it wasn't,
        /// to allow other actors to take ownership of it.
        /// </summary>
        /// <param name="objectID">The space object to release ownership of</param>
        /// <returns>True if ownership was successfully released</returns>
        bool ReleaseOwnership(int objectID);

        /// <summary>
        /// Try to find an instance of <see cref="SpatialNetworkObject"/> by its object ID.
        /// </summary>
        bool TryFindNetworkObject(int objectID, out SpatialNetworkObject networkObject);

        /// <summary>
        /// Spawns an instance of a networked prefab in the space.
        /// </summary>
        /// <param name="prefab">A prefab that has a <see cref="SpatialNetworkObject"/> component attached</param>
        /// <param name="position">Optional initial position</param>
        /// <param name="rotation">Optional initial rotation</param>
        /// <returns><c>Async</c> Returns the spawned GameObject once it has been instantiated</returns>
        SpawnNetworkObjectRequest SpawnNetworkObject(SpatialNetworkObject prefab, Vector3? position = null, Quaternion? rotation = null);

        /// <summary>
        /// Spawns an NPC avatar, owned by the local actor and visible to all other actors.
        /// This object will remain alive in the server until explicitly destroyed (or the server instance was shut down).
        /// The ownership of this object can be transferred to another actor if requested, and by default the ownership is transferred
        /// to the master client when the owner disconnects.
        /// </summary>
        /// <param name="assetType">Which asset type to set this from. <see cref="AssetType.BackpackItem"/> is not supported here</param>
        /// <param name="assetID">
        /// ID of the asset:
        /// - <see cref="AssetType.Package"/>: The packageSKU found in Spatial Studio or Unity
        /// - <see cref="AssetType.EmbeddedAsset"/>: The assetID specified in the <see cref="UnitySDK.Editor.SpaceConfig"/> in Unity Editor
        /// </param>
        /// <param name="position">Initial position of the avatar</param>
        /// <param name="rotation">Initial facing direction of the avatar (this will always be corrected to Y-up).</param>
        /// <param name="displayName">The name in the avatar's nametag</param>
        /// <returns><c>Async</c> Returns the created avatar once it has been registered with the space.</returns>
        SpawnAvatarRequest SpawnAvatar(AssetType assetType, string assetID, Vector3 position, Quaternion rotation, string displayName);

        /// <summary>
        /// Spawns a prefab object from a specific package uploaded thru Spatial Studio.
        /// </summary>
        /// <param name="assetType">Which asset type to set this from. <see cref="AssetType.BackpackItem"/> is not supported here</param>
        /// <param name="assetID">
        /// ID of the asset:
        /// - <see cref="AssetType.Package"/>: The packageSKU found in Spatial Studio or Unity
        /// - <see cref="AssetType.EmbeddedAsset"/>: The assetID specified in the <see cref="UnitySDK.Editor.SpaceConfig"/> in Unity Editor
        /// </param>
        /// <param name="position">Position where an object should be instantiated.</param>
        /// <param name="rotation">Rotation of the object to instantiate.</param>
        SpawnPrefabObjectRequest SpawnPrefabObject(AssetType assetType, string assetID, Vector3 position, Quaternion rotation);

        /// <summary>
        /// Try find the space object ID for a given game object in the scene. Any child game object of a visual representation
        /// of a space object can be used to retrieve the space object ID.
        /// </summary>
        /// <param name="gameObject">A game object that is the visual representation of a space object</param>
        /// <param name="objectID">The resulting space object ID</param>
        /// <returns>True if this game object is a space object</returns>
        bool TryGetSpaceObjectID(GameObject gameObject, out int objectID);

        /// <summary>
        /// Gets the actor ID of the actor that owns the spatial component.
        /// </summary>
        /// <param name="component">Spatial component (eg. SpatialAvatarAttachment)</param>
        /// <returns>Owner actor of spatial component</returns>
        int GetOwner(ISpatialComponentWithOwner component);

        #region Obsolete

        // Synced Objects
        /// <exclude />
        const string OBSOLETE_MESSAGE = "This will be removed soon. Use SpatialNetworkObject instead.";
        /// <exclude />
        [Obsolete(OBSOLETE_MESSAGE)] event OnSyncedObjectInitializedDelegate onSyncedObjectInitialized;
        /// <exclude />
        [Obsolete(OBSOLETE_MESSAGE)] public delegate void OnSyncedObjectInitializedDelegate(SpatialSyncedObject syncedObject);
        /// <exclude />
        [Obsolete(OBSOLETE_MESSAGE)] event OnSyncedObjectOwnerChangedDelegate onSyncedObjectOwnerChanged;
        /// <exclude />
        [Obsolete(OBSOLETE_MESSAGE)] public delegate void OnSyncedObjectOwnerChangedDelegate(SpatialSyncedObject syncedObject, int newOwnerActor);
        /// <exclude />
        [Obsolete(OBSOLETE_MESSAGE)] event OnSyncedObjectVariableChangedDelegate onSyncedObjectVariableChanged;
        /// <exclude />
        [Obsolete(OBSOLETE_MESSAGE)] public delegate void OnSyncedObjectVariableChangedDelegate(SpatialSyncedVariables syncedVariables, string variableName, object newValue);
        /// <exclude />
        [Obsolete(OBSOLETE_MESSAGE)] bool TakeoverSyncedObjectOwnership(SpatialSyncedObject syncedObject);
        /// <exclude />
        [Obsolete(OBSOLETE_MESSAGE)] SpatialSyncedObject GetSyncedObjectByID(int id);
        /// <exclude />
        [Obsolete(OBSOLETE_MESSAGE)] bool GetSyncedObjectIsSynced(SpatialSyncedObject syncedObject);
        /// <exclude />
        [Obsolete(OBSOLETE_MESSAGE)] int GetSyncedObjectID(SpatialSyncedObject syncedObject);
        /// <exclude />
        [Obsolete(OBSOLETE_MESSAGE)] int GetSyncedObjectOwner(SpatialSyncedObject syncedObject);
        /// <exclude />
        [Obsolete(OBSOLETE_MESSAGE)] bool GetSyncedObjectHasControl(SpatialSyncedObject syncedObject);
        /// <exclude />
        [Obsolete(OBSOLETE_MESSAGE)] bool GetSyncedObjectIsLocallyOwned(SpatialSyncedObject syncedObject);

        /// <exclude />
        [Obsolete("Use GetOwner(ISpatialComponentWithOwner component) instead.")]
        int GetOwnerActor(SpatialComponentBase component);

        // SyncedAnimator
        /// <exclude />
        [Obsolete(OBSOLETE_MESSAGE)] void SetSyncedAnimatorParameter(SpatialSyncedAnimator syncedAnimator, string parameterName, object value);
        /// <exclude />
        [Obsolete(OBSOLETE_MESSAGE)] void SetSyncedAnimatorTrigger(SpatialSyncedAnimator syncedAnimator, string triggerName);

        #endregion
    }

    [DocumentationCategory("Services/Space Content Service")]
    public class SpawnSpaceObjectRequest : SpatialAsyncOperation
    {
        public bool succeeded;
        public ISpaceObject spaceObject;
    }

    [DocumentationCategory("Services/Space Content Service")]
    public class DestroySpaceObjectRequest : SpatialAsyncOperation
    {
        public bool succeeded;
        public int spaceObjectID;
    }

    [DocumentationCategory("Services/Space Content Service")]
    public class SpaceObjectOwnershipTransferRequest : SpatialAsyncOperation
    {
        public bool succeeded;
        public int spaceObjectID;
        public int oldOwnerActor;
        public int newOwnerActor;
    }

    [DocumentationCategory("Services/Space Content Service")]
    public class SpawnNetworkObjectRequest : SpawnSpaceObjectRequest
    {
        public SpatialNetworkObject networkObject;
        public GameObject gameObject;
    }

    [DocumentationCategory("Services/Space Content Service")]
    public class SpawnAvatarRequest : SpawnSpaceObjectRequest
    {
        public IAvatar avatar;
    }

    [DocumentationCategory("Services/Space Content Service")]
    public class SpawnPrefabObjectRequest : SpawnSpaceObjectRequest
    {
        public IPrefabObject prefabObject;
    }
}
