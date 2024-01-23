using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial: On World Currency Balance Changed")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("On World Currency Balance Changed")]
    [UnitCategory("Events\\Spatial\\Space Economy")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class OnWorldCurrencyBalanceChangedNode : EventUnit<ulong>
    {
        public const string EVENT_HOOK_ID = "OnWorldCurrencyBalanceChanged";
        protected override bool register => true;

        [DoNotSerialize]
        public ValueOutput balance { get; private set; }


        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EVENT_HOOK_ID);
        }

        public static void TriggerEvent(ulong balance)
        {
            EventBus.Trigger(EVENT_HOOK_ID, balance);
        }

        protected override void Definition()
        {
            base.Definition();
            balance = ValueOutput<ulong>(nameof(balance));
        }

        protected override bool ShouldTrigger(Flow flow, ulong balance)
        {
            return true;
        }

        protected override void AssignArguments(Flow flow, ulong balance)
        {
            flow.SetValue(this.balance, balance);
        }
    }
}
