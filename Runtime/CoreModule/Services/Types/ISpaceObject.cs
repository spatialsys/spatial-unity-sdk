using UnityEngine;
using System;
using System.Collections.Generic;
namespace SpatialSys.UnitySDK
{
    /// <summary>
    /// The data-model for an object as it exists on the server. Space objects can be created, destroyed, and modified by actors in the space.
    /// </summary>
    [DocumentationCategory("Services/Space Content Service")]
    public interface IReadOnlySpaceObject : IEquatable<IReadOnlySpaceObject>, IEquatable<ISpaceObject>
    {
        /// <summary>
        /// The unique number of the space object in the current server instance.
        /// ObjectID's are allocated when space objects are created and cannot be modified.
        /// </summary>
        int objectID { get; }

        /// <summary>
        /// Space object behavior flags.
        /// </summary>
        SpaceObjectFlags flags { get; }

        /// <summary>
        /// The actorNumber of the actor that created the space object.
        /// </summary>
        int creatorActorNumber { get; }

        /// <summary>
        /// The actorNumber of the actor that currently owns the space object.
        /// </summary>
        int ownerActorNumber { get; }

        /// <summary>
        /// Whether the space object is owned by the local actor (localActorNumber == ownerActorNumber).
        /// </summary>
        bool isMine { get; }

        /// <summary>
        /// Whether the local actor owns the object or has control over it in the case where the object isn't owned
        /// by anyone. (isMine || (hasNoOwner && isMasterClient))
        /// </summary>
        bool hasControl { get; }

        /// <summary>
        /// Whether the space object has been disposed. This will be true if the object has been destroyed.
        /// Disposed space objects can still be read from, but can no longer be edited.
        /// </summary>
        bool isDisposed { get; }

        /// <summary>
        /// Whether the space object can be taken over by another actor.
        /// </summary>
        bool canTakeOwnership { get; }

        /// <summary>
        /// Type of object it is, such as an avatar.
        /// </summary>
        SpaceObjectType objectType { get; }

        /// <summary>
        /// Event fired when the owner of the space object changes.
        /// </summary>
        event SpaceObjectOwnerChangedDelegate onOwnerChanged;
        public delegate void SpaceObjectOwnerChangedDelegate(SpaceObjectOwnerChangedEventArgs args);

        /// <summary>
        /// The position of a space object in world space as it exists on the server.
        /// Note: This may not be the same as the position of the visual representation of the object in the scene because
        /// the visual representation may be smoothed or extrapolated.
        /// </summary>
        Vector3 position { get; }

        /// <summary>
        /// The rotation of a space object in world space as it exists on the server.
        /// Note: This may not be the same as the rotation of the visual representation of the object in the scene because
        /// the visual representation may be smoothed or extrapolated.
        /// </summary>
        Quaternion rotation { get; }

        /// <summary>
        /// The scale of a space object in world space as it exists on the server.
        /// Note: This may not be the same as the scale of the visual representation of the object in the scene because
        /// the visual representation may be smoothed or extrapolated.
        /// </summary>
        Vector3 scale { get; }

        /// <summary>
        /// Whether the space object has variables attached to it.
        /// </summary>
        bool hasVariables { get; }

        /// <summary>
        /// A dictionary of variables that are attached to the space object. These are synchronized across all clients.
        /// This dictionary is never null, but may be empty. To check if object has any variables, use
        /// <see cref="hasVariables"/> for better performance.
        /// </summary>
        IReadOnlyDictionary<byte, object> variables { get; }

        /// <summary>
        /// Event fired when one or more variables on the space object have changed (added, changed, removed).
        /// The event is fired for all connected clients when they receive the change.
        /// </summary>
        event SpaceObjectVariablesChangedDelegate onVariablesChanged;
        public delegate void SpaceObjectVariablesChangedDelegate(SpaceObjectVariablesChangedEventArgs args);

        /// <summary>
        /// Retrieves the value of a variable on the space object.
        /// </summary>
        /// <param name="id">The id of the variable to retrieve. It is recommended to use an enum of type byte to define variables.</param>
        /// <param name="value">The value of the variable if it exists.</param>
        /// <returns>True if the variable exists, false otherwise.</returns>
        bool TryGetVariable(byte id, out object value);

        /// <summary>
        /// Retrieves the value of a variable on the space object.
        /// </summary>
        /// <param name="id">The id of the variable to retrieve. It is recommended to use an enum of type byte to define variables.</param>
        /// <param name="value">The value of the variable if it exists casted to the desired type.</param>
        /// <returns>True if the variable exists, false otherwise.</returns>
        bool TryGetVariable<T>(byte id, out T value);
    }

    /// <summary>
    /// A space object interface that allows for read and write access to the object.
    /// Setting values on the object requires the local client to have ownership of the object.
    /// </summary>
    [DocumentationCategory("Services/Space Content Service")]
    public interface ISpaceObject : IReadOnlySpaceObject
    {
        /// <summary>
        /// The position of a space object in world space as it exists on the server.
        /// Note: This may not be the same as the position of the visual representation of the object in the scene because
        /// the visual representation may be smoothed or extrapolated.
        /// </summary>
        new Vector3 position { get; set; }

        /// <summary>
        /// The rotation of a space object in world space as it exists on the server.
        /// Note: This may not be the same as the rotation of the visual representation of the object in the scene because
        /// the visual representation may be smoothed or extrapolated.
        /// </summary>
        new Quaternion rotation { get; set; }

        /// <summary>
        /// The scale of a space object in world space as it exists on the server.
        /// Note: This may not be the same as the scale of the visual representation of the object in the scene because
        /// the visual representation may be smoothed or extrapolated.
        /// </summary>
        new Vector3 scale { get; set; }

        /// <summary>
        /// Same as <see cref="SetVariable{T}(byte, T)"/> but with an object type. Provided for convenience.
        /// </summary>
        void SetVariable(byte id, object value);

        /// <summary>
        /// Set the value of, or create a variable on the space object. Requires the local actor to have ownership of the object.
        /// Note that depending on the context, setting a variable may not be instantaneous but it will always be performed in the same frame.
        /// </summary>
        /// <param name="id">The id of the variable to set or create. It is recommended to use an enum of type byte to define variables.</param>
        /// <param name="value">
        /// The value of the variable (supported types: int, bool, float, Vector2, Vector3, string, Color32, byte, double, long, int[])
        /// </param>
        void SetVariable<T>(byte id, T value);

        /// <summary>
        /// Remove a variable from the space object. Requires the local actor to have ownership of the object.
        /// Note that depending on the context, removing a variable may not be instantaneous but it will always be performed in the same frame.
        /// </summary>
        /// <param name="id">The id of the variable to remove</param>
        void RemoveVariable(byte id);
    }

    [System.Flags]
    public enum SpaceObjectFlags
    {
        None = 0,
        /// <summary>
        /// Flag indicating that the object should always be owned by the master client.
        /// </summary>
        MasterClientObject = 1 << 0,
        /// <summary>
        /// Flag indicating that the object should be destroyed when the owner leaves the space.
        /// </summary>
        DestroyWhenOwnerLeaves = 1 << 1,
        /// <summary>
        /// Flag indicating that the object should be destroyed when the creator of the object leaves the space.
        /// </summary>
        DestroyWhenCreatorLeaves = 1 << 2,
        /// <summary>
        /// Flag indicating that the object can be transferred to another client.
        /// </summary>
        AllowOwnershipTransfer = 1 << 3,
    }

    /// <summary>
    /// An enum used to describe what type of visual component a space object has.
    /// </summary>
    [DocumentationCategory("Services/Space Content Service")]
    public enum SpaceObjectType : byte
    {
        /// <summary>
        /// The type of component is unknown. This is likely an internal spatial object not yet exposed via the SDK.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The space object has no visual component. For example: an empty object with variables.
        /// </summary>
        Empty = 1,

        /// <summary>
        /// The space object is an avatar.
        /// </summary>
        Avatar = 2,

        /// <summary>
        /// The space object is a Spatial prefab.
        /// </summary>
        PrefabObject = 3,

        /// <summary>
        /// The space object represents an network object embedded in a scene or spawned at runtime.
        /// </summary>
        NetworkObject = 4,
    }

    /// <summary>
    /// Arguments for the <see cref="ISpaceObject.onOwnerChanged"/> event.
    /// </summary>
    [DocumentationCategory("Services/Space Content Service")]
    public struct SpaceObjectOwnerChangedEventArgs
    {
        public int previousOwnerActorNumber;
        public int newOwnerActorNumber;
    }

    /// <summary>
    /// Arguments for the <see cref="ISpaceObject.onVariablesChanged"/> event.
    /// </summary>
    [DocumentationCategory("Services/Space Content Service")]
    public struct SpaceObjectVariablesChangedEventArgs
    {
        /// <summary>
        /// Variables that were newly added or changed.
        /// This dictionary reference is re-pooled and re-used between events, so you should not cache it.
        /// </summary>
        public IReadOnlyDictionary<byte, object> changedVariables;

        /// <summary>
        /// Variables that were removed.
        /// This list reference is re-pooled and re-used between events, so you should not cache it.
        /// </summary>
        public IReadOnlyList<byte> removedVariables;
    }
}
