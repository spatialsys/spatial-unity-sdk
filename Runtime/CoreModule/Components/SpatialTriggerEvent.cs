using System;
using SpatialSys.UnitySDK.Internal;
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
        public override string tooltip =>
        @"Invokes the Spatial Event when an object enters the trigger. The SpatialEvent contains Syncable UnityEvents, AnimatorEvents, and QuestEvents.
        
    • <b>UnityEvents</b> can be used to invoke single function on components like gameObject.SetActive(false) to disable a target gameObject.
        
    • <b>AnimatorEvents</b> can be used to set parameters on an Animator component. If you want the Animator parameters to be synced across clients, you must use a SyncedAnimator component.
        
    • <b>QuestEvents</b> can be used to update the state of a quest. Quests are created by adding the Quest component to any gameObject in the scene. Quest events are not synced.";

        public override string documentationURL => "https://docs.spatial.io/components/trigger-event";
        public override bool isExperimental => false;

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
            SpatialBridge.spatialComponentService.InitializeTriggerEvent(this);
        }

        private void OnEnable()
        {
            SpatialBridge.spatialComponentService.TriggerEventEnabledChanged(this, true);
        }

        private void OnDisable()
        {
            SpatialBridge.spatialComponentService.TriggerEventEnabledChanged(this, false);
        }

        private void Reset()
        {
            version = LATEST_VERSION;
        }
    }
}
