
namespace SpatialSys.UnitySDK
{
    /// <summary>
    /// A component of a space object.
    /// </summary>
    [DocumentationCategory("Services/Space Content Service")]
    public interface IReadOnlySpaceObjectComponent
    {
        /// <summary>
        /// Returns true when the component or its parent space object has been destroyed.
        /// </summary>
        bool isDisposed { get; }

        /// <summary>
        /// The id of the spaceObject the component is attached to.
        /// </summary>
        int spaceObjectID { get; }

        /// <summary>
        /// The space object the component is attached to.
        /// </summary>
        IReadOnlySpaceObject spaceObject { get; }
    }

    /// <summary>
    /// A component of a space object.
    /// </summary>
    [DocumentationCategory("Services/Space Content Service")]
    public interface ISpaceObjectComponent : IReadOnlySpaceObjectComponent
    {
        /// <summary>
        /// The space object the component is attached to.
        /// </summary>
        new ISpaceObject spaceObject { get; }
    }
}
