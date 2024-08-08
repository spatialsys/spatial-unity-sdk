using System;
using SpatialSys.UnitySDK.Internal;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace SpatialSys.UnitySDK
{
    [DocumentationCategory("Core/Components")]
    [RequireComponent(typeof(Collider))]
    public class SpatialTriggerEvent : SpatialComponentBase
    {
        public const int LATEST_VERSION = 2;

        public override string prettyName => "Trigger Event";
        public override string tooltip =>
        @"Invokes the Spatial Event when an object enters the trigger. The SpatialEvent contains Syncable UnityEvents, AnimatorEvents, and QuestEvents.
        
    • <b>UnityEvents</b> can be used to invoke single function on components like gameObject.SetActive(false) to disable a target gameObject.
        
    • <b>AnimatorEvents</b> can be used to set parameters on an Animator component. If you want the Animator parameters to be synced across clients, you must use a SyncedAnimator component.
        
    • <b>QuestEvents</b> can be used to update the state of a quest. Quests are created by adding the Quest component to any gameObject in the scene. Quest events are not synced.";

        public override string documentationURL => "https://toolkit.spatial.io/docs/components/trigger-event";
        public override bool isExperimental => false;

        [Flags]
        public enum ListenFor
        {
            None = 0,
            LocalAvatar = 1 << 0,
            LocalNPC = 1 << 1,
        }

        [HideInInspector]
        public int version;
        public ListenFor listenFor = ListenFor.LocalAvatar;
        public SpatialEvent onEnterEvent;
        public SpatialEvent onExitEvent;

        [HideInInspector, FormerlySerializedAs("onEnter"), Obsolete("Use onEnterEvent instead.")]
        public UnityEvent deprecated_onEnter;
        [HideInInspector, FormerlySerializedAs("onEnter"), Obsolete("Use onExitEvent instead.")]
        public UnityEvent deprecated_onExit;

        private void Start()
        {
            if (version == 1)
                listenFor = ListenFor.LocalAvatar;
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

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            UpgradeDataIfNecessary();
        }

        public void UpgradeDataIfNecessary()
        {
            if (version == LATEST_VERSION)
                return;

            if (version == 1)
            {
                listenFor = ListenFor.LocalAvatar; // for any v1 triggers, default value 0 was LocalAvatar. Set explicitly.
                version = 2;
            }
        }
#endif

    }
}
