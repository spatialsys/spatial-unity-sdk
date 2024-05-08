using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialSys.UnitySDK
{
    /// <summary>
    /// Service for interacting with a space's synchronized content.
    /// </summary>
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
        /// <param name="sku">Unique ID (sku) of the package to instantiate.</param>
        /// <param name="position">Position where an object should be instantiated.</param>
        /// <param name="rotation">Rotation of the object to instantiate.</param>
        SpawnPrefabObjectRequest SpawnPrefabObject(AssetType assetType, string assetID, Vector3 position, Quaternion rotation);

        /// <summary>
        /// Try find the space object ID for a given game object in the scene. Any child game object of a visual representation
        /// of a space object can be used to retrieve the space object ID.
        /// </summary>
        /// <param name="gameObject">A game object that is the visual representation of a space object</param>
        /// <param name="objectID">The resulting space onbject id</param>
        /// <returns>True if this game object is a space object</returns>
        bool TryGetSpaceObjectID(GameObject gameObject, out int objectID);

        #region Synced Objects -- Will be deprecated in future versions

        /// <summary>
        /// Event fired when a synced object is initialized.
        /// </summary>
        event OnSyncedObjectInitializedDelegate onSyncedObjectInitialized;
        public delegate void OnSyncedObjectInitializedDelegate(SpatialSyncedObject syncedObject);

        /// <summary>
        /// Event fired when a synced object's owner changes.
        /// </summary>
        event OnSyncedObjectOwnerChangedDelegate onSyncedObjectOwnerChanged;
        public delegate void OnSyncedObjectOwnerChangedDelegate(SpatialSyncedObject syncedObject, int newOwnerActor);

        /// <summary>
        /// Event fired when a synced object's variable changes.
        /// </summary>
        event OnSyncedObjectVariableChangedDelegate onSyncedObjectVariableChanged;
        public delegate void OnSyncedObjectVariableChangedDelegate(SpatialSyncedVariables syncedVariables, string variableName, object newValue);

        /// <summary>
        /// Tries to take ownership of a synced object.
        /// </summary>
        /// <param name="syncedObject">Synced object to try to take ownership,</param>
        /// <returns>True if synced object is now locally owned, otherwise false.</returns>
        bool TakeoverSyncedObjectOwnership(SpatialSyncedObject syncedObject);

        /// <summary>
        /// Gets a synced object by ID.
        /// </summary>
        /// <param name="id">ID of the synced object.</param>
        /// <returns>Synced object. Null if synced object was not found.</returns>
        SpatialSyncedObject GetSyncedObjectByID(int id);

        /// <summary>
        /// Gets whether or not a synced object is currently being synced.
        /// </summary>
        /// <param name="syncedObject">Synced object.</param>
        /// <returns>True if object is being synced, otherwise false.</returns>
        bool GetSyncedObjectIsSynced(SpatialSyncedObject syncedObject);

        /// <summary>
        /// Gets the ID of a synced object.
        /// </summary>
        /// <param name="syncedObject">Synced object.</param>
        /// <returns>ID of synced object.</returns>
        int GetSyncedObjectID(SpatialSyncedObject syncedObject);

        /// <summary>
        /// Gets the owner of a synced object.
        /// </summary>
        /// <param name="syncedObject">Synced object.</param>
        /// <returns>Actor ID of the synced object owner.</returns>
        int GetSyncedObjectOwner(SpatialSyncedObject syncedObject);

        /// <summary>
        /// Gets whether or not a synced object can be modified by the local actor. This can happen when
        /// either the local actor is the owner of the synced object, or if the synced object's owner is
        /// no longer in the space and the local actor is the master client.
        /// </summary>
        /// <param name="syncedObject">Synced object.</param>
        /// <returns>True if object can be controlled locally, otherwise false.</returns>
        bool GetSyncedObjectHasControl(SpatialSyncedObject syncedObject);

        /// <summary>
        /// Gets whether or not a synced object is locally owned.
        /// </summary>
        /// <param name="syncedObject">Synced object.</param>
        /// <returns>True if object is locally owned, otherwise false.</returns>
        bool GetSyncedObjectIsLocallyOwned(SpatialSyncedObject syncedObject);

        #endregion

        #region SyncedAnimator -- Will be deprecated in future versions

        /// <summary>
        /// Sets a synced animator parameter.
        /// </summary>
        /// <param name="syncedAnimator">Synced animator.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">Parameter value.</param>
        void SetSyncedAnimatorParameter(SpatialSyncedAnimator syncedAnimator, string parameterName, object value);

        /// <summary>
        /// Sets a synced animator trigger.
        /// </summary>
        /// <param name="syncedAnimator">Synced animator.</param>
        /// <param name="triggerName">Trigger name.</param>
        void SetSyncedAnimatorTrigger(SpatialSyncedAnimator syncedAnimator, string triggerName);

        #endregion

        #region General Spatial Components -- Will be deprecated in future versions

        /// <summary>
        /// Gets the actor ID of the actor that owns the spatial component.
        /// </summary>
        /// <param name="component">Spatial component (eg. SpatialAvatarAttachment)</param>
        /// <returns>Owner actor of spatial component</returns>
        public int GetOwnerActor(SpatialComponentBase component);

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
    public class SpawnAvatarRequest : SpatialAsyncOperation
    {
        public bool succeeded;
        public IAvatar avatar;
    }

    [DocumentationCategory("Services/Space Content Service")]
    public class SpawnPrefabObjectRequest : SpatialAsyncOperation
    {
        public bool succeeded;
        public IPrefabObject prefabObject;
    }
}
