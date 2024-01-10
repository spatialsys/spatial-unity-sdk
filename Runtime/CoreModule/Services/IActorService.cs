using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialSys.UnitySDK
{
    public interface IActorService
    {
        /// <summary>
        /// Number of fully joined actors in the current server instance.
        /// </summary>
        int actorCount { get; }

        /// <summary>
        /// Number of actors that have joined the current server instance, but have not yet fully synchronized yet.
        /// </summary>
        int pendingActorCount { get; }

        /// <summary>
        /// The actor number of the local actor (the local user). This changes whenever the local user joins a new
        /// server instance (or disconnects and reconnects to the same server instance).
        /// </summary>
        int localActorNumber { get; }

        /// <summary>
        /// The local actor (the local user)
        /// </summary>
        ILocalActor localActor { get; }

        /// <summary>
        /// A dictionary of all actors in the current server instance, keyed by actor number.
        /// </summary>
        /// <remarks>
        /// Each IActor instance is updated as the actor's state changes, so it is safe to cache references to them,
        /// although you should refresh your cache after switching or reconnecting to a server instance.
        /// 
        /// This collection is modified before the <see cref="onActorJoined"/> event is triggered, but after 
        /// <see cref="onActorLeft"/> is triggered. This allows you to get a reference to the actor before it is
        /// removed from the collection.
        /// 
        /// All actors in this collection are "fully joined" (actors.Count should equal <see cref="actorCount"/>)
        /// 
        /// Note that while <see cref="localActor"/> is never null, the local actor will only be included in this
        /// collection when the local actor has fully joined the server instance (when the actorNumber for the local
        /// actor is known).
        /// </remarks>
        IReadOnlyDictionary<int, IActor> actors { get; }

        /// <summary>
        /// Event that is triggered initially apon connection for all actors who are in the server, and then
        /// when a new actor joins the server.
        /// </summary>
        /// <remarks>
        /// This event is triggered for all actors, even those that had already joined before the local actor joined the
        /// server. You can use <see cref="ActorJoinedEventArgs.wasAlreadyJoined"/> to determine if the actor was
        /// already joined or not.
        /// Note that this is also triggered for the local actor when the local actor joins the server.
        /// </remarks>
        event ActorJoinedDelegate onActorJoined;
        public delegate void ActorJoinedDelegate(ActorJoinedEventArgs args);

        /// <summary>
        /// Event that is triggered when an actor leaves the server (this can be for any reason, including
        /// disconnecting, timing out, or being kicked)
        /// </summary>
        event ActorLeftDelegate onActorLeft;
        public delegate void ActorLeftDelegate(ActorLeftEventArgs args);

        /// <summary>
        /// Return a random actor currently in the space. This will fail if you're not connected to a server instance.
        /// </summary>
        bool TryGetRandomActor(bool includeLocalActor, out IActor actor);
    }

    public interface IActor
    {
        /// <summary>
        /// The unique number of the actor in the current server instance.
        /// If an actor leaves and rejoins the server, they will be assigned a new actor number.
        /// Actor numbers are guaranteed to be unique within a server instance and start at 1.
        /// Actor number 0 is reserved for the server.
        /// </summary>
        int actorNumber { get; }

        /// <summary>
        /// Whether the actor has been disposed. This will be true if the actor has left the server.
        /// </summary>
        bool isDisposed { get; }

        /// <summary>
        /// The user ID of the actor
        /// </summary>
        string userID { get; }

        /// <summary>
        /// The username for the public profile of the user.
        /// </summary>
        string username { get; }

        /// <summary>
        /// The display name for the user. This is displayed in the nametag and on the user's profile page.
        /// </summary>
        string displayName { get; }

        /// <summary>
        /// Does the user for this actor have a fully completed Spatial account?
        /// This will be false if the user is a guest or has not completed their account setup.
        /// </summary>
        bool isRegistered { get; }

        /// <summary>
        /// Is the user for this actor an administrator of the space?
        /// </summary>
        bool isSpaceAdministrator { get; }

        /// <summary>
        /// Is the user for this actor the owner of the space?
        /// </summary>
        bool isSpaceOwner { get; }

        /// <summary>
        /// The platform that the actor is currently using to join this space.
        /// </summary>
        SpatialPlatform platform { get; }

        /// <summary>
        /// Is the user for this actor currently talking with voice chat? This is only true when user is un-muted
        /// and actively talking.
        /// </summary>
        bool isTalking { get; }

        /// <summary>
        /// The avatar for this actor. This will be null if the actor does not have an avatar in the scene
        /// </summary>
        IReadOnlyAvatar avatar { get; }

        /// <summary>
        /// A dictionary of custom properties for the actor. These properties are synchronized across all clients.
        /// Only actors themselves can change their own custom properties.
        /// An actor's properties are only cleared if they leave the space. Teleporting between servers of the same
        /// space will not clear the properties.
        /// </summary>
        IReadOnlyDictionary<string, object> customProperties { get; }

        /// <summary>
        /// Event that is triggered when the avatar for the actor is created.
        /// Because <see cref="avatar"/> can be null, this is provided as convenience to avoid having to check for null
        /// every time you want to access the avatar.
        /// </summary>
        event Action<bool> onAvatarExistsChanged;

        /// <summary>
        /// Event that is triggered when any of the actor's custom properties change.
        /// </summary>
        event ActorCustomPropertiesChangedDelegate onCustomPropertiesChanged;
        public delegate void ActorCustomPropertiesChangedDelegate(ActorCustomPropertiesChangedEventArgs args);

        /// <summary>
        /// Get the profile picture texture for the actor.
        /// </summary>
        /// <returns>The profile picture texture if it can be found. Returns null if the texture is not available</returns>
        ActorProfilePictureRequest GetProfilePicture();
    }

    public interface ILocalActor : IActor
    {
        /// <summary>
        /// The avatar for the local actor (the local user). This will never be null, even if the avatar for the local
        /// user was hidden or disabled.
        /// </summary>
        new IAvatar avatar { get; }

        /// <summary>
        /// Set a custom property for this actor. These properties are synchronized across all clients.
        /// An actor's properties are only cleared if they leave the space. Teleporting between servers of the same
        /// space will not clear the properties.
        /// </summary>
        void SetCustomProperty(string name, object value);
    }

    public struct ActorJoinedEventArgs
    {
        public int actorNumber;

        /// <summary>
        /// This will be true if the actor was already joined when the local actor joined the server.
        /// Since the <see cref="IActorService.onActorJoined"/> event is triggered for all actors, even those that
        /// joined before the local actor, this flag can be used to determine if the actor was already joined or not.
        /// </summary>
        public bool wasAlreadyJoined;
    }

    public struct ActorLeftEventArgs
    {
        public int actorNumber;
    }

    public struct ActorCustomPropertiesChangedEventArgs
    {
        /// <summary>
        /// Properties that were newly added or changed.
        /// This dictionary reference is re-pooled and resused between events, so you should not cache it.
        /// </summary>
        public IReadOnlyDictionary<string, object> changedProperties;

        /// <summary>
        /// Properties that were removed.
        /// This list reference is re-pooled and resused between events, so you should not cache it.
        /// </summary>
        public IReadOnlyList<string> removedProperties;
    }

    public class ActorProfilePictureRequest : SpatialAsyncOperation
    {
        public bool succeeded;
        public int actorNumber;
        public Texture2D texture;
    }
}
