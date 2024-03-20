using System;
using UnityEngine;

namespace SpatialSys.UnitySDK
{
    /// <summary>
    /// Service for interacting with Spatial scene objects (synced objects, prefab objcets, anvatar attachments and synced animators).
    /// </summary>
    [DocumentationCategory("Services/Space Content Service")]
    public interface ISpaceContentService
    {
        #region Scene
        /// <summary>
        /// True if the scene has been fully initialized.
        /// </summary>
        bool isSceneInitialized { get; }

        /// <summary>
        /// Event fired when the main space package has been fully initialized.
        /// </summary>
        event Action onSceneInitialized;

        #endregion

        #region Synced Objects

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

        #region SyncedAnimator

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

        #region General Spatial Components

        /// <summary>
        /// Gets the actor ID of the actor that owns the spatial component.
        /// </summary>
        /// <param name="component">Spatial component (eg. SpatialAvatarAttachment)</param>
        /// <returns>Owner actor of spatial component</returns>
        public int GetOwnerActor(SpatialComponentBase component);

        #endregion

        #region Prefab Objects

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
        void SpawnPrefabObject(AssetType assetType, string assetID, Vector3 position, Quaternion rotation);

        #endregion
    }
}
