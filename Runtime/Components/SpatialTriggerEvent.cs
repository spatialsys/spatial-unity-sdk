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
        public override string documentationURL => "https://www.notion.so/spatialxr/Trigger-Event-6165695d8f1c423881c9d75935c7d934";
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

        [FormerlySerializedAs("onEnter"), Obsolete("Use onEnterEvent instead.")]
        public UnityEvent deprecated_onEnter;
        [FormerlySerializedAs("onEnter"), Obsolete("Use onExitEvent instead.")]
        public UnityEvent deprecated_onExit;
    }
}
