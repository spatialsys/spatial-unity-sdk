using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpatialSys.UnitySDK.EditorSimulation
{
#pragma warning disable 67 // Disable unused event warning

    public class EditorNetworkingService : INetworkingService
    {
        private Dictionary<string, object> _serverProperties = new();

        public INetworkingRemoteEventsService remoteEvents { get; } = new EditorNetworkingRemoteEventsService();
        public ServerConnectionStatus connectionStatus => ServerConnectionStatus.Disconnected;
        public bool isConnected => false;
        public double networkTime => Time.realtimeSinceStartup;
        public int spaceParticipantCount => 1;
        public int serverParticipantCount => 1;
        public int serverParticipantCountUnique => 1;
        public int serverMaxParticipantCount { get; private set; } = 1;
        public int serverCount => 1;
        public bool isServerOpen { get; private set; } = true;
        public bool isServerVisible { get; private set; } = false;
        public bool isServerInstancingEnabled => false;
        public bool isMasterClient => true;
        public int masterClientActorNumber => SpatialBridge.actorService.localActorNumber;

        public event INetworkingService.OnServerConnectionStatusChangedDelegate onConnectionStatusChanged;
        public event INetworkingService.OnSpaceParticipantCountChangedDelegate onSpaceParticipantCountChanged;
        public event INetworkingService.OnServerParticipantCountChangedDelegate onServerParticipantCountChanged;

        public void SetServerOpen(bool open) => isServerOpen = open;
        public void SetServerVisible(bool visible) => isServerVisible = visible;
        public void SetServerMaxParticipantCount(int maxParticipants) => serverMaxParticipantCount = maxParticipants;
        public IReadOnlyDictionary<string, object> GetServerProperties() => _serverProperties;
        public void SetServerProperties(IReadOnlyCollection<KeyValuePair<string, object>> properties)
        {
            foreach (KeyValuePair<string, object> kvp in properties)
                _serverProperties[kvp.Key] = kvp.Value;
        }
        public void TeleportToNewServer(int maxParticipants = 0, bool isOpen = true, bool isVisible = true, IReadOnlyCollection<KeyValuePair<string, object>> serverProperties = null, IReadOnlyCollection<string> matchProperties = null) { }
        public void TeleportActorsToNewServer(IReadOnlyCollection<int> actorNumbers, int maxParticipants = 0, bool isVisible = true, IReadOnlyCollection<KeyValuePair<string, object>> serverProperties = null, IReadOnlyCollection<string> matchProperties = null) { }
        public void TeleportToBestMatchServer(int maxParticipants = 0, IReadOnlyCollection<KeyValuePair<string, object>> serverProperties = null, IReadOnlyCollection<string> serverPropertiesToMatch = null) { }
    }

    public class EditorNetworkingRemoteEventsService : INetworkingRemoteEventsService
    {
        public event INetworkingRemoteEventsService.OnEventDelegate onEvent;

        public void RaiseEventAll(byte eventID, params object[] args)
        {
            // Raise event locally
            onEvent?.Invoke(new NetworkingRemoteEventArgs {
                senderActor = 1,
                eventID = eventID,
                eventArgs = args,
            });
        }

        public void RaiseEventOthers(byte eventID, params object[] args) { }

        public void RaiseEvent(IReadOnlyCollection<int> targetActors, byte eventID, params object[] args)
        {
            // Raise event locally
            if (targetActors.Any(a => a == 1))
            {
                onEvent?.Invoke(new NetworkingRemoteEventArgs {
                    senderActor = 1,
                    eventID = eventID,
                    eventArgs = args,
                });
            }
        }
    }
}
