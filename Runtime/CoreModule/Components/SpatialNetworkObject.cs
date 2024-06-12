using SpatialSys.UnitySDK.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Experimental.SceneManagement;
#endif

namespace SpatialSys.UnitySDK
{
    /// <summary>
    /// This component enables a GameObject to be synchronized across the network. A network object handles synchronizing
    /// the transform, rigidbody, and custom variables defined through visual scripting or <see cref="NetworkVariable"/>s
    /// within <see cref="SpatialNetworkBehaviour"/>.
    /// 
    /// Network objects can be either embedded in a scene or instantiated from a prefab.
    /// 
    /// To spawn a network object for all clients, use <see cref="SpatialBridge.spaceContentService.Spawn"/>. Note
    /// that GameObject.Instantiate will only create a network object for the local client.
    /// 
    /// To despawn a network object you can simply Object.Destroy the GameObject, or use
    /// <see cref="SpatialBridge.spaceContentService.DestroySpaceObject"/>. This will despawn the object for all clients.
    /// </summary>
    [DocumentationCategory("Core/Components")]
    [DisallowMultipleComponent]
    public sealed class SpatialNetworkObject : SpatialComponentBase, ISpatialComponentWithOwner
#if UNITY_EDITOR
        , ISerializationCallbackReceiver
#endif
    {
        public override string prettyName => "Spatial Network Object";
        public override string tooltip => "Represents a single synchronized entity in the session";
        public override string documentationURL => "https://toolkit.spatial.io/docs/multiplayer/network-object";

        public SpaceObjectFlags objectFlags = SpaceObjectFlags.AllowOwnershipTransfer | SpaceObjectFlags.DestroyWhenOwnerLeaves;
        public NetworkObjectSyncFlags syncFlags;

        /// <summary>
        /// Unique ID for the prefab that this NetworkObject is attached to. This is used to know which prefab
        /// to instantiate on other clients when this object is spawned.
        /// This is only assigned to the root NetworkObject in a prefab.
        /// </summary>
        public int networkPrefabGuid;

        /// <summary>
        /// Unique ID for NetworkObjects that are embedded in a scene. Guaranteed to be unique within the scene.
        /// </summary>
        public int sceneObjectGuid;

        // NetworkObject properties
        public SpatialNetworkObject rootObject;
        public SpatialNetworkObject parentObject;
        public SpatialNetworkObject[] nestedObjects;
        public SpatialNetworkBehaviour[] behaviours;
        public event Action onDestroy;

        // SpaceObject properties exposed for convenience
        public ISpaceObject spaceObject { get; set; }
        public int objectID => spaceObject?.objectID ?? 0;
        public bool isMine => spaceObject?.isMine ?? true;
        public bool hasControl => spaceObject?.hasControl ?? true;
        public int ownerActorNumber => spaceObject?.ownerActorNumber ?? 0;

        private void OnDestroy()
        {
            onDestroy?.Invoke();
        }

        public void RequestOwnership()
        {
            SpatialBridge.spaceContentService.TakeOwnership(objectID);
        }

        public void ReleaseOwnership()
        {
            SpatialBridge.spaceContentService.ReleaseOwnership(objectID);
        }

        public void TransferOwnership(int actorNumber)
        {
            SpatialBridge.spaceContentService.TransferOwnership(objectID, actorNumber);
        }

        public static bool TryFindObject(int objectID, out SpatialNetworkObject obj)
        {
            return SpatialBridge.spaceContentService.TryFindNetworkObject(objectID, out obj);
        }

#if UNITY_EDITOR
        private void ValidateObject()
        {
            if (Application.isPlaying)
                return;

            bool isOnPrefab = (gameObject.scene.name == null || !gameObject.scene.name.Equals(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name));
            bool destroyOnDisconnectAllowed = isOnPrefab;
            if (!destroyOnDisconnectAllowed && (objectFlags.HasFlag(SpaceObjectFlags.DestroyWhenCreatorLeaves) || objectFlags.HasFlag(SpaceObjectFlags.DestroyWhenOwnerLeaves)))
            {
                objectFlags &= ~SpaceObjectFlags.DestroyWhenCreatorLeaves;
                objectFlags &= ~SpaceObjectFlags.DestroyWhenOwnerLeaves;
                UnityEditor.EditorUtility.SetDirty(this);
            }

            if (objectFlags.HasFlag(SpaceObjectFlags.MasterClientObject))
            {
                objectFlags &= ~SpaceObjectFlags.DestroyWhenOwnerLeaves;
                objectFlags &= ~SpaceObjectFlags.DestroyWhenCreatorLeaves;
                objectFlags &= ~SpaceObjectFlags.AllowOwnershipTransfer;
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }

        [ContextMenu("Remove Visual Scripting Network Variables")]
        private void RemoveVisualScriptingNetworkVariables()
        {
            SpatialNetworkVariables networkVariables = GetComponent<SpatialNetworkVariables>();
            if (networkVariables != null)
                DestroyImmediate(networkVariables);
        }

        #region GUID Handling
        /// <summary>
        /// Assignes a new guid to this network object
        /// </summary>
        public void RefreshGuid()
        {
            sceneObjectGuid = 0;
            CreateGuid();
        }

        // We cannot allow a GUID to be saved into a prefab, and we need to convert to byte[]
        public void OnBeforeSerialize()
        {
            // This lets us detect if we are a prefab instance or a prefab asset.
            // A prefab asset cannot contain a GUID since it would then be duplicated when instanced.
            if (IsInPrefabAsset())
                sceneObjectGuid = 0;
        }
        public void OnAfterDeserialize() { }

        protected override void OnValidate()
        {
            base.OnValidate();

            ValidateObject();

            // Clear the guid if we are duplicating or pasting
            if (Event.current != null && Event.current.type == EventType.ExecuteCommand && (Event.current.commandName == "Duplicate" || Event.current.commandName == "Paste"))
                sceneObjectGuid = 0;

            // similar to on Serialize, but gets called on Copying a Component or Applying a Prefab
            // at a time that lets us detect what we are
            if (IsInPrefabAsset())
            {
                sceneObjectGuid = 0;
            }
            else
            {
                CreateGuid();
            }
        }

        private void CreateGuid()
        {
            // We already have a valid guid
            if (sceneObjectGuid != 0)
                return;

            // Irrelevant for prefabs
            if (IsInPrefabAsset())
                return;

            // Generate new guid then convert to long using first 4 bytes
            // This may conflict, but we ensure that the sceneObjectGuid is unique in the 
            // scene using <see cref="SpatialNetworkedObjectProcessor"/>
            var guid = Guid.NewGuid();
            sceneObjectGuid = BitConverter.ToInt32(guid.ToByteArray(), 0);
            UnityEditor.EditorUtility.SetDirty(this);

            // If we are creating a new GUID for a prefab instance of a prefab, but we have somehow lost our prefab connection
            // force a save of the modified prefab instance properties
            if (PrefabUtility.IsPartOfNonAssetPrefabInstance(this))
                PrefabUtility.RecordPrefabInstancePropertyModifications(this);
        }

        private bool IsInPrefabAsset()
        {
            return PrefabUtility.IsPartOfPrefabAsset(this) || IsEditingInPrefabMode();
        }

        private bool IsEditingInPrefabMode()
        {
            if (UnityEditor.EditorUtility.IsPersistent(this))
            {
                // if the game object is stored on disk, it is a prefab of some kind, despite not returning true for IsPartOfPrefabAsset =/
                return true;
            }
            else
            {
                // If the GameObject is not persistent let's determine which stage we are in first because getting Prefab info depends on it
                var mainStage = StageUtility.GetMainStageHandle();
                var currentStage = StageUtility.GetStageHandle(gameObject);
                if (currentStage != mainStage)
                {
                    var prefabStage = PrefabStageUtility.GetPrefabStage(gameObject);
                    if (prefabStage != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion
#endif
    }

    [System.Flags]
    [InternalType]
    public enum NetworkObjectSyncFlags
    {
        None = 0,
        Position = 1 << 0,
        Rotation = 1 << 1,
        Scale = 1 << 2,
        Rigidbody = 1 << 3,
    }

    /// <summary>
    /// This component allows you to define custom behaviour for a network object. It must always be accompanied by a
    /// <see cref="SpatialNetworkObject"/> component on the same GameObject or on one of its parents.
    /// </summary>
    public abstract class SpatialNetworkBehaviour : MonoBehaviour
    {
        public int objectID => networkObject?.objectID ?? 0;
        public SpatialNetworkObject networkObject { get; set; }
        public bool isMine => networkObject?.isMine ?? false;
        public bool hasControl => networkObject?.hasControl ?? false;
        public int ownerActorNumber => networkObject?.ownerActorNumber ?? 0;

        /// <summary>
        /// Called when the network object is spawned. Use this method instead of Start.
        /// Accessing and modifying the network object's state is only allowed after this method is called.
        /// </summary>
        public virtual void Spawned() { }

        /// <summary>
        /// Called before the network object is despawned. Use this method instead of OnDestroy.
        /// After this method is called, the network object's state can no longer be modified, and the underlying
        /// space object will be marked as disposed.
        /// </summary>
        public virtual void Despawned() { }
    }

    [InternalType]
    public interface INetworkVariable
    {
        public static HashSet<Type> CURRENTLY_SUPPORTED_TYPES = new HashSet<Type> {
            typeof(int),
            typeof(bool),
            typeof(float),
            typeof(Vector2),
            typeof(Vector3),
            typeof(string),
            typeof(Color32),
            typeof(byte),
            typeof(double),
            typeof(long),
            typeof(int[]),
        };

        ISpaceObject spaceObject { get; }
        byte id { get; }
        bool idAssigned { get; }
        object initialValue { get; }

        void Bind(ISpaceObject spaceObject, byte id, string variableName);
    }

    /// <summary>
    /// To be used in conjunction with <see cref="SpatialNetworkBehaviour"/>. This allows you to define a variable
    /// that will be synchronized across the network.
    /// </summary>
    public class NetworkVariable<T> : INetworkVariable
    {
        private T _initialValue;
        private string _variableName;

        public ISpaceObject spaceObject { get; private set; }
        public byte id { get; private set; }
        public bool idAssigned { get; private set; }
        public T value
        {
            get
            {
                if (spaceObject.TryGetVariable<T>(id, out T value))
                    return value;
                return default;
            }
            set
            {
                spaceObject.SetVariable(id, value);
            }
        }
        object INetworkVariable.initialValue => _initialValue;

        public NetworkVariable() : this(default)
        {
        }

        public NetworkVariable(T initialValue)
        {
            _initialValue = initialValue;
        }

        /// <summary>
        /// Creates a new network variable with a default value.
        /// </summary>
        /// <param name="id">Manually assign an ID. This ID is used to identify variables for a give <see cref="SpatialNetworkObject"/></param>
        /// <param name="initialValue">The default value when the network object is created</param>
        public NetworkVariable(byte id, T initialValue) : this(initialValue)
        {
            this.id = id;
            idAssigned = true;
        }

        public static implicit operator T(NetworkVariable<T> var)
        {
            return var.value;
        }

        /// <summary>
        /// Called internally before <see cref="SpatialNetworkBehaviour.Spawned"/> is called.
        /// </summary>
        public void Bind(ISpaceObject spaceObject, byte id, string variableName)
        {
            this.spaceObject = spaceObject;
            if (!idAssigned)
            {
                this.id = id;
                idAssigned = true;
            }
            _variableName = variableName;
        }

        public override string ToString()
        {
            return $"NetworkVariable<{typeof(T).Name}>[name={_variableName}, value={value}, id={id}]";
        }
    }

    /// <summary>
    /// To be used in conjunction with <see cref="SpatialNetworkBehaviour"/>.
    /// Called right after <see cref="SpatialNetworkBehaviour.Spawned"/> and when the ownership of the network object changes.
    /// </summary>
    public interface IOwnershipChanged
    {
        void OnOwnershipChanged(NetworkObjectOwnershipChangedEventArgs args);
    }

    /// <summary>
    /// Arguments for the <see cref="IOwnershipChanged.OnOwnershipChanged"/>.
    /// </summary>
    public struct NetworkObjectOwnershipChangedEventArgs
    {
        public int previousOwnerActorNumber;
        public int newOwnerActorNumber;
    }

    /// <summary>
    /// To be used in conjunction with <see cref="SpatialNetworkBehaviour"/>.
    /// Called right after <see cref="SpatialNetworkBehaviour.Spawned"/> with variable initial value changes, and called
    /// when any of the variables of the network object change. This includes variables defined in visual scripting and
    /// marked as networked with the <see cref="SpatialNetworkVariables"/> component, and also any <see cref="NetworkVariable<T>"/>
    /// defined in other behaviours attached to the same network object.
    /// </summary>
    public interface IVariablesChanged
    {
        void OnVariablesChanged(NetworkObjectVariablesChangedEventArgs args);
    }

    /// <summary>
    /// Arguments for the <see cref="IVariablesChanged.OnVariablesChanged"/>.
    /// </summary>
    public struct NetworkObjectVariablesChangedEventArgs
    {
        /// <summary>
        /// Variables that were changed, with the key being the variable ID and the value being the new value.
        /// This dictionary reference is re-pooled and re-used between events, so you should not cache it.
        /// </summary>
        public IReadOnlyDictionary<byte, object> changedVariables;
    }
}