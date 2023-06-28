using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("On Actor Left")]
    [UnitCategory("Events\\Spatial\\Actor")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class OnActorLeftNode : EventUnit<int>
    {
        public static string eventName = "OnActorLeft";
        protected override bool register => true;

        [DoNotSerialize]
        public ValueOutput actor { get; private set; }

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(eventName);
        }

        protected override void Definition()
        {
            base.Definition();
            actor = ValueOutput<int>(nameof(actor));
        }

        protected override bool ShouldTrigger(Flow flow, int args)
        {
            return true;
        }

        protected override void AssignArguments(Flow flow, int arg)
        {
            flow.SetValue(actor, arg);
        }
    }
}
