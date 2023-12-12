using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial: On Connected Changed")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("On Connected Changed")]
    [UnitCategory("Events\\Spatial\\Server")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class OnConnectedChangedNode : EventUnit<bool>
    {
        private const string EVENT_HOOK_ID = "SpatialOnConnectedChanged";

        protected override bool register => true;

        [DoNotSerialize]
        public ValueOutput connected { get; private set; }

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EVENT_HOOK_ID);
        }

        public static void TriggerEvent(ServerConnectionStatus status)
        {
            bool isConnected = status == ServerConnectionStatus.Connected;
            EventBus.Trigger(EVENT_HOOK_ID, isConnected);
        }

        protected override void Definition()
        {
            base.Definition();
            connected = ValueOutput<bool>(nameof(connected));
        }

        protected override void AssignArguments(Flow flow, bool connected)
        {
            flow.SetValue(this.connected, connected);
        }
    }

    [UnitTitle("Spatial: On Space Participant Count Changed")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("On Space Participant Count Changed")]
    [UnitCategory("Events\\Spatial\\Server")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class OnSpaceParticipantCountChangedNode : EventUnit<int>
    {
        private const string EVENT_HOOK_ID = "SpatialOnSpaceParticipantCountChanged";

        protected override bool register => true;

        [DoNotSerialize]
        public ValueOutput count { get; private set; }

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EVENT_HOOK_ID);
        }

        public static void TriggerEvent(int count)
        {
            EventBus.Trigger(EVENT_HOOK_ID, count);
        }

        protected override void Definition()
        {
            base.Definition();
            count = ValueOutput<int>(nameof(count));
        }

        protected override void AssignArguments(Flow flow, int count)
        {
            flow.SetValue(this.count, count);
        }
    }

    [UnitTitle("Spatial: On Server Participant Count Changed")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("On Server Participant Count Changed")]
    [UnitCategory("Events\\Spatial\\Server")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class OnServerParticipantCountChangedNode : EventUnit<int>
    {
        private const string EVENT_HOOK_ID = "SpatialOnServerParticipantCountChanged";

        protected override bool register => true;

        [DoNotSerialize]
        public ValueOutput count { get; private set; }

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EVENT_HOOK_ID);
        }

        public static void TriggerEvent(int count)
        {
            EventBus.Trigger(EVENT_HOOK_ID, count);
        }

        protected override void Definition()
        {
            base.Definition();
            count = ValueOutput<int>(nameof(count));
        }

        protected override void AssignArguments(Flow flow, int count)
        {
            flow.SetValue(this.count, count);
        }
    }
}