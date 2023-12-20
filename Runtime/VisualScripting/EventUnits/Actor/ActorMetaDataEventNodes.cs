using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Actor: On Custom Property Changed")]
    [UnitSurtitle("Actor")]
    [UnitShortTitle("On Custom Property Changed")]
    [UnitSubtitle("Event")]
    [UnitCategory("Events\\Spatial\\Actor")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class OnActorCustomVariableChanged : EventUnit<(int, string, object)>
    {
        private const string EVENT_HOOK_ID = "OnActorCustomVariableChanged";
        protected override bool register => true;

        [DoNotSerialize]
        public ValueOutput actor { get; private set; }
        [DoNotSerialize]
        [PortLabel("Property Name")]
        public ValueOutput variableName { get; private set; }
        [DoNotSerialize]
        [PortLabel("Property Value")]
        public ValueOutput variableValue { get; private set; }

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EVENT_HOOK_ID);
        }

        public static void TriggerEvent(int actor, string variableName, object variableValue)
        {
            EventBus.Trigger(EVENT_HOOK_ID, (actor, variableName, variableValue));
        }

        protected override void Definition()
        {
            base.Definition();
            actor = ValueOutput<int>(nameof(actor));
            variableName = ValueOutput<string>(nameof(variableName));
            variableValue = ValueOutput<object>(nameof(variableValue));
        }

        protected override bool ShouldTrigger(Flow flow, (int, string, object) args)
        {
            return true;
        }

        protected override void AssignArguments(Flow flow, (int, string, object) args)
        {
            flow.SetValue(actor, args.Item1);
            flow.SetValue(variableName, args.Item2);
            flow.SetValue(variableValue, args.Item3);
        }
    }
}
