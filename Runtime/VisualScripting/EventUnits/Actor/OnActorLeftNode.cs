using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("On Actor Left")]
    [UnitCategory("Events\\Spatial\\Actor")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class OnActorLeftNode : EventUnit<int>
    {
        private const string EVENT_HOOK_ID = "OnActorLeft";

        protected override bool register => true;

        [DoNotSerialize]
        public ValueOutput actor { get; private set; }

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EVENT_HOOK_ID);
        }

        public static void TriggerEvent(int actorNumber)
        {
            EventBus.Trigger(EVENT_HOOK_ID, actorNumber);
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
