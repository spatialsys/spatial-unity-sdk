using System;
using System.Collections.Generic;

namespace SpatialSys.UnitySDK
{
    /// <summary>
    /// This service provides access to all the networking functionality in Spatial: connectivity, server management,
    /// matchmaking, remove events (RPCs/Messaging), etc.
    /// </summary>
    public interface INetworkingService
    {
        /// <summary>
        /// Interface for sending and receiving custom network messages
        /// </summary>
        INetworkingRemoteEventsService remoteEvents { get; }

        /// <summary>
        /// The connection status to the current server. Most networking service functionality can only be performed
        /// if fully connected. This state is useful to be able to communicate connection status to the user.
        /// </summary>
        ServerConnectionStatus connectionStatus { get; }

        /// <summary>
        /// Is connected to server (this means the client is ready to receive and send messages).
        /// Shortcut for `connectionStatus == ServerConnectionStatus.Connected`
        /// </summary>
        bool isConnected { get; }

        /// <summary>
        /// Time in seconds that is synced with the server and all actors connected to the current server instance.
        /// The value is guaranteed to be the same across all actors and the server.
        /// Returns 0 if not connected.
        /// </summary>
        /// <remarks>
        /// This is not a DateTime! Use this value with care:
        /// - It can start with any positive value
        /// - It will "wrap around" from 4294967.295 to 0
        /// </remarks>
        double networkTime { get; }

        /// <summary>
        /// Total number of participants for the current space, totalling all participants from all servers for this space.
        /// This is roughly accurate and is updated every few seconds.
        /// </summary>
        int spaceParticipantCount { get; }

        /// <summary>
        /// Total number of participants in the current server
        /// Returns 0 if not connected.
        /// </summary>
        int serverParticipantCount { get; }

        /// <summary>
        /// Total number of unique participants in the current server
        /// Returns 0 if not connected.
        /// </summary>
        int serverParticipantCountUnique { get; }

        /// <summary>
        /// Total number of participants (actors) allowed to be in this server
        /// Returns 0 if not connected.
        /// </summary>
        int serverMaxParticipantCount { get; }

        /// <summary>
        /// Total number of active servers for the current space
        /// Returns 0 if not connected.
        /// </summary>
        int serverCount { get; }

        /// <summary>
        /// Is the current server open? Open means that others can join the server. When closed others will be prevented
        /// from joining.
        /// Returns false if not connected.
        /// </summary>
        bool isServerOpen { get; }

        /// <summary>
        /// Is the current server visible to others? Invisible servers don't show up in the UI and are "hidden" to the
        /// matchmaking service.
        /// Returns false if not connected.
        /// </summary>
        bool isServerVisible { get; }

        /// <summary>
        /// Is the server instancing feature enabled for this space? If this is true, then multiple servers can exist
        /// for the space, if false, only one single server instance will ever exist (all participants in single server).
        /// This can be configured in the Space Package Config settings in Unity.
        /// </summary>
        bool isServerInstancingEnabled { get; }

        /// <summary>
        /// Is the local client the master client? The master client is in charge of various server simulation tasks and is a good candidate 
        /// for running code that should only run once per server instance.
        /// </summary>
        bool isMasterClient { get; }

        /// <summary>
        /// The actor number of the master client. The master client is in charge of various server simulation tasks and is a good candidate 
        /// for running code that should only run once per server instance.
        /// </summary>
        int masterClientActorNumber { get; }

        /// <summary>
        /// Event that triggers when the server connection status changes
        /// </summary>
        event OnServerConnectionStatusChangedDelegate onConnectionStatusChanged;
        public delegate void OnServerConnectionStatusChangedDelegate(ServerConnectionStatus status);

        /// <summary>
        /// Event that triggers when the participant count (actor count) for the current space changes.
        /// This is the total number of participants for the current space, totalling all participants from all servers
        /// </summary>
        event OnSpaceParticipantCountChangedDelegate onSpaceParticipantCountChanged;
        public delegate void OnSpaceParticipantCountChangedDelegate(int count);

        /// <summary>
        /// Event that triggers when the participant count (actor count) for the current server changes.
        /// This is the total number of participants for the current server only, not for the space globally.
        /// This is also triggered when not connected and will have a value of 0.
        /// </summary>
        event OnServerParticipantCountChangedDelegate onServerParticipantCountChanged;
        public delegate void OnServerParticipantCountChangedDelegate(int count);

        /// <summary>
        /// Set the current server open state. This is only allowed if connection state is "Connected".
        /// This option is only available if the server instancing feature is enabled for the current space.
        /// </summary>
        void SetServerOpen(bool open);

        /// <summary>
        /// Set the current server visibility state. This is only allowed if connection state is "Connected".
        /// This option is only available if the server instancing feature is enabled for the current space.
        /// </summary>
        void SetServerVisible(bool visible);

        /// <summary>
        /// Set the max number of participants for the current server.
        /// You can only set this to a value that is less than or equal to the current participant (actor) count.
        /// </summary>
        void SetServerMaxParticipantCount(int maxParticipants);

        /// <summary>
        /// Get the current server properties. Server properties persist with the server instance until the server is
        /// closed, which happens when the last actor disconnects from the server.
        /// This only works if actively connected to the server. Check for `isConnected` before calling this.
        /// </summary>
        /// <remarks>
        /// Supported types: string, int, bool
        /// </remarks>
        IReadOnlyDictionary<string, object> GetServerProperties();

        /// <summary>
        /// Set properties for the current server. Server properties persist with the server instance until the server is
        /// closed, which happens when the last actor disconnects from the server.
        /// This only works if actively connected to the server. Check for `isConnected` before calling this.
        /// </summary>
        /// <remarks>
        /// Supported types: string, int, bool
        /// Property names validation rule: a-z, A-Z, 0-9, _ and and only starting with letters, and must be maximum 10
        /// characters long.
        /// Additionally, it is recommended to keep property names as short as possible, as they are broadcasted as bare
        /// strings, which can increase network traffic.
        /// </remarks>
        /// <param name="properties">Dictionary of properties to set (string, int, bool)</param>
        void SetServerProperties(IReadOnlyCollection<KeyValuePair<string, object>> properties);

        /// <summary>
        /// Teleport local actor to a new server instance. This will disconnect the actor from the current server and
        /// connect to a new server instance, which will have the specified properties.
        /// To check if the teleport was successful, listen to the <see cref="onConnectionStatusChanged"/> event.
        /// </summary>
        /// <param name="maxParticipants">Max number of participants (actors) allowed to join this server concurrently</param>
        /// <param name="isOpen">Can others join this server?</param>
        /// <param name="isVisible">Is this server visible to others, and should it be taken account for matchmaking?</param>
        /// <param name="serverProperties">Properties to set for the new server</param>
        /// <param name="matchProperties">Which of the "serverProperties" should be made available for matchmaking check?</param>
        void TeleportToNewServer(int maxParticipants = 0, bool isOpen = true, bool isVisible = true, IReadOnlyCollection<KeyValuePair<string, object>> serverProperties = null, IReadOnlyCollection<string> matchProperties = null);

        /// <summary>
        /// Similar to <see cref="TeleportToNewServer"/>, but allows specifying a list of actors to teleport to
        /// the new server.
        /// To check if the teleport was successful, listen to the <see cref="onConnectionStatusChanged"/> event.
        /// </summary>
        /// <param name="actorNumbers">Which actors should be teleported? This can include the local actor too</param>
        /// <param name="maxParticipants">Max number of participants (actors) allowed to join this server concurrently</param>
        /// <param name="isVisible">Is this server visible to others, and should it be taken account for matchmaking?</param>
        /// <param name="serverProperties">Properties to set for the new server</param>
        /// <param name="matchProperties">Which of the "serverProperties" should be made available for matchmaking check?</param>
        void TeleportActorsToNewServer(IReadOnlyCollection<int> actorNumbers, int maxParticipants = 0, bool isVisible = true, IReadOnlyCollection<KeyValuePair<string, object>> serverProperties = null, IReadOnlyCollection<string> matchProperties = null);

        /// <summary>
        /// Teleport local actor to the best match server instance based on the specified properties. If no match can be
        /// found, a new server instance will be created with the specified properties.
        /// 
        /// For example:
        /// - A server exists that has a property "gameMode" with value "deathmatch"
        /// - You can call this method with serverProperties={"gameMode": "deathmatch"} and serverPropertiesToMatch={"gameMode"}
        /// - This will find the existing server and teleport the actor to that server
        /// - If no server exists with the specified properties, a new server will be created with the specified properties
        /// </summary>
        /// <param name="maxParticipants">Find a server that matches this maxParticipants count; 0 means "any" and will default to the settings set for the space if no match was found</param>
        /// <param name="serverProperties">Specify the properties that you want to match for</param>
        /// <param name="serverPropertiesToMatch">Specify which propeties from "serverProperties" that should be used for matchmaking.</param>
        void TeleportToBestMatchServer(int maxParticipants = 0, IReadOnlyCollection<KeyValuePair<string, object>> serverProperties = null, IReadOnlyCollection<string> serverPropertiesToMatch = null);
    }

    public enum ServerConnectionStatus
    {
        Disconnected = 0,
        Connecting = 1,
        Connected = 2,
        Disconnecting = 3,
    }

    /// <summary>
    /// This interface allows sending and receiving custom network messages.
    /// 
    /// Supported event argument types: byte, bool, short, int, long, float, double, string, Vector2, Vector3,
    /// Quaternion, Color, Color32, Vector4, DateTime
    /// </summary>
    /// <example><code source="Services/NetworkingServiceExamples.cs" region="RPCExample"/></example>
    public interface INetworkingRemoteEventsService
    {
        /// <summary>
        /// Event that triggers when a remote event is received
        /// </summary>
        /// <example><code source="Services/NetworkingServiceExamples.cs" region="RPCExample"/></example>
        event OnEventDelegate onEvent;
        public delegate void OnEventDelegate(NetworkingRemoteEventArgs args);

        /// <summary>
        /// Raise a remote event to all actors connected to the current server.
        /// Note that this will also raise the event on the current actor, which triggers the `onEvent` callback.
        /// </summary>
        /// <example><code source="Services/NetworkingServiceExamples.cs" region="RPCExample"/></example>
        void RaiseEventAll(byte eventID, params object[] args);

        /// <summary>
        /// Raise a remote event to all actors connected to the current server, except the current actor.
        /// </summary>
        void RaiseEventOthers(byte eventID, params object[] args);

        /// <summary>
        /// Raise a remote event to specific set of actors
        /// </summary>
        /// <param name="targetActors">Which actors to raise this event for. For GC efficiency, passing in a int[] type is recommended</param>
        void RaiseEvent(IReadOnlyCollection<int> targetActors, byte eventID, params object[] args);
    }

    public struct NetworkingRemoteEventArgs
    {
        public int senderActor;
        public byte eventID;
        public object[] eventArgs;
    }
}
