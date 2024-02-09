using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial System: On Scene Initialized")]
    [UnitSurtitle("Spatial System")]
    [UnitShortTitle("On Scene Initialized")]
    [UnitSubtitle("Event")]
    [UnitCategory("Events\\Spatial\\System")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class OnSceneInitializedNode : EventUnit<EmptyEventArgs>
    {
        private const string EVENT_HOOK_ID = "OnSceneInitialized";

        protected override bool register => true;

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EVENT_HOOK_ID);
        }

        public static void TriggerEvent()
        {
            EventBus.Trigger(EVENT_HOOK_ID);
        }

        protected override bool ShouldTrigger(Flow flow, EmptyEventArgs args)
        {
            return true;
        }
    }
}
