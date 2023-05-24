using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial: Get World Currency Balance")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Get World Currency Balance")]
    [UnitCategory("Spatial\\Actions")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class GetWorldCurrencyBalanceNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueOutput balance { get; private set; }

        protected override void Definition()
        {
            balance = ValueOutput<ulong>(nameof(balance));

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                f.SetValue(balance, ClientBridge.GetWorldCurrencyBalance.Invoke());
                return outputTrigger;
            });
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitTitle("Spatial: Award World Currency")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Award World Currency")]
    [UnitCategory("Spatial\\Actions")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class AwardWorldCurrencyNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput amount { get; private set; }

        [DoNotSerialize]
        public ValueOutput succeeded { get; private set; }

        protected override void Definition()
        {
            amount = ValueInput<ulong>(nameof(amount), 1);
            succeeded = ValueOutput<bool>(nameof(succeeded));

            inputTrigger = ControlInputCoroutine(nameof(inputTrigger), ExecuteAsync);
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }

        private IEnumerator ExecuteAsync(Flow flow)
        {
            bool completed = false;
            ClientBridge.AwardWorldCurrency.Invoke(flow.GetValue<ulong>(amount), success => {
                completed = true;
                flow.SetValue(succeeded, success);
            });
            yield return new WaitUntil(() => completed);

            yield return outputTrigger;
        }
    }
}
