namespace SpatialSys.UnitySDK
{

    /// <summary>
    /// The data-model for a prefab objects as it exists on the server. Prefab object's are a component of SpaceObjects.
    /// </summary>
    [DocumentationCategory("Services/Space Content Service")]
    public interface IReadOnlyPrefabObject : IReadOnlySpaceObjectComponent
    {
        /// <summary>
        /// The type of Spatial asset currently used by the prefab object.
        /// </summary>
        AssetType assetType { get; }

        /// <summary>
        /// ID of the Spatial asset currently used by the prefab object.
        /// </summary>
        /// <remarks>
        /// The ID can refer to different things depending on the <see cref="assetType"/>:
        /// - <see cref="AssetType.BackpackItem"/>: The itemID found in Spatial Studio
        /// - <see cref="AssetType.Package"/>: The packageSKU found in Spatial Studio or Unity
        /// - <see cref="AssetType.EmbeddedAsset"/>: The assetID specified in the <see cref="UnitySDK.Editor.SpaceConfig"/> in Unity Editor
        /// </remarks>
        string assetID { get; }
    }

    /// <summary>
    /// A prefab object interface that allows for read and write access to the object.
    /// Setting values on the object requires the local client to have ownership of the object.
    /// </summary>
    [DocumentationCategory("Services/Space Content Service")]
    public interface IPrefabObject : IReadOnlyPrefabObject, ISpaceObjectComponent
    {
        /// <summary>
        /// Sets the asset used by the prefab object. Successfully changing the asset will cause the gameObjects representing this prefab
        /// in the scene to be re-instantiated for all clients. The IPrefabObject instances will remain the same.
        /// </summary>
        /// <param name="assetType">
        /// The type of Spatial asset the assetID refers to.
        /// - <see cref="AssetType.BackpackItem"/>: The itemID found in Spatial Studio
        /// - <see cref="AssetType.Package"/>: The packageSKU found in Spatial Studio or Unity
        /// - <see cref="AssetType.EmbeddedAsset"/>: The assetID specified in the <see cref="UnitySDK.Editor.SpaceConfig"/> in Unity Editor
        /// </param>
        /// <param name="assetID">
        /// ID of the Spatial asset currently used by the prefab object.
        /// </param>
        void SetAsset(AssetType assetType, string assetID);
    }
}
