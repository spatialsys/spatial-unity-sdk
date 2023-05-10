using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace SpatialSys.UnitySDK
{
    [RequireComponent(typeof(Collider))]
    public class SpatialTriggerEvent : SpatialComponentBase
    {
        public const int LATEST_VERSION = 1;

        public override string prettyName => "Trigger Event";
        public override string tooltip => "Invokes the UnityEvent when an object enters the trigger.";
        public override string documentationURL => "https://docs.spatial.io/components/trigger-event";
        public override bool isExperimental => true;

        public enum ListenFor
        {
            LocalAvatar,
        }

        [HideInInspector]
        public int version;
        public ListenFor listenFor;
        public SpatialEvent onEnterEvent;
        public SpatialEvent onExitEvent;

        [HideInInspector, FormerlySerializedAs("onEnter"), Obsolete("Use onEnterEvent instead.")]
        public UnityEvent deprecated_onEnter;
        [HideInInspector, FormerlySerializedAs("onEnter"), Obsolete("Use onExitEvent instead.")]
        public UnityEvent deprecated_onExit;

        private void Start()
        {
            ClientBridge.InitializeSpatialTriggerEvent?.Invoke(this);
        }

        private void OnEnable()
        {
            ClientBridge.TriggerEventEnabledChanged?.Invoke(this, true);
        }

        private void OnDisable()
        {
            ClientBridge.TriggerEventEnabledChanged?.Invoke(this, false);
        }

        private void Reset()
        {
            version = LATEST_VERSION;
        }
    }
}
