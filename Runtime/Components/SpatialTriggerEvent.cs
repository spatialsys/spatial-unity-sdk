using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SpatialSys.UnitySDK
{
    [RequireComponent(typeof(Collider))]
    public class SpatialTriggerEvent : SpatialComponentBase
    {
        public override string prettyName => "Trigger Event";
        public override string tooltip => "Invokes the UnityEvent when an object enters the trigger.";

        public enum ListenFor
        {
            LocalAvatar,
            Anything,
        }
        public ListenFor listenFor;
        public UnityEvent onEnter;
        public UnityEvent onExit;
    }
}
