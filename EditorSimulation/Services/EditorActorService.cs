using System;
using System.Collections.Generic;

namespace SpatialSys.UnitySDK.EditorSimulation
{
    public class EditorActorService : IActorService
    {
        public int actorCount => 1;
        public int pendingActorCount => 0;
        public int localActorNumber => 1;
        public ILocalActor localActor { get; } = new EditorLocalActor();
        public IReadOnlyDictionary<int, IActor> actors { get; }

#pragma warning disable 0067 // Disable "event is never used" warning
        public event IActorService.ActorJoinedDelegate onActorJoined;
        public event IActorService.ActorLeftDelegate onActorLeft;
#pragma warning restore 0067

        public EditorActorService()
        {
            actors = new Dictionary<int, IActor>() {
                { 1, localActor }
            };
        }

        public bool TryGetRandomActor(bool includeLocalActor, out IActor actor)
        {
            if (includeLocalActor)
            {
                actor = localActor;
                return true;
            }

            actor = null;
            return false;
        }
    }

    public class EditorActor : IActor
    {
        public int actorNumber => 0;
        public bool isDisposed => false;
        public string userID => string.Empty;
        public string username => string.Empty;
        public string displayName => string.Empty;
        public bool isRegistered => true;
        public bool isSpaceAdministrator => false;
        public SpatialPlatform platform => SpatialPlatform.Unknown;
        public bool isTalking => false;
        public IReadOnlyAvatar avatar => null;
        public IReadOnlyDictionary<string, object> customProperties => new Dictionary<string, object>();

#pragma warning disable 0067 // Disable "event is never used" warning
        public event Action<bool> onAvatarExistsChanged;
        public event IActor.ActorCustomPropertiesChangedDelegate onCustomPropertiesChanged;
#pragma warning restore 0067

        public ActorProfilePictureRequest GetProfilePicture()
        {
            ActorProfilePictureRequest request = new() {
                succeeded = false,
            };
            request.InvokeCompletionEvent();
            return request;
        }
    }

    public class EditorLocalActor : ILocalActor
    {
        public int actorNumber => 1;
        public bool isDisposed => false;
        public string userID => string.Empty;
        public string username => string.Empty;
        public string displayName => "Editor Local Actor";
        public bool isRegistered => true;
        public bool isSpaceAdministrator => true;
        public SpatialPlatform platform => SpatialPlatform.Web;
        public bool isTalking => false;
        public IReadOnlyDictionary<string, object> customProperties => _customProperties;

        public IAvatar avatar => new EditorLocalAvatar();
        IReadOnlyAvatar IActor.avatar => avatar;

#pragma warning disable 0067 // Disable "event is never used" warning
        public event Action<bool> onAvatarExistsChanged;
        public event IActor.ActorCustomPropertiesChangedDelegate onCustomPropertiesChanged;
#pragma warning restore 0067

        private Dictionary<string, object> _customProperties = new();

        public ActorProfilePictureRequest GetProfilePicture()
        {
            ActorProfilePictureRequest request = new() {
                succeeded = false,
            };
            request.InvokeCompletionEvent();
            return request;
        }

        public void SetCustomProperty(string name, object value)
        {
            _customProperties[name] = value;
            onCustomPropertiesChanged?.Invoke(new ActorCustomPropertiesChangedEventArgs() {
                changedProperties = new Dictionary<string, object>() {
                    { name, value }
                },
                removedProperties = new List<string>(),
            });
        }
    }
}
