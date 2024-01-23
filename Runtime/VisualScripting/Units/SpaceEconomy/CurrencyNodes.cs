using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial: Get World Currency Balance")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Get World Currency Balance")]
    [UnitCategory("Spatial\\Space Economy")]
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
                f.SetValue(balance, SpatialBridge.inventoryService.worldCurrencyBalance);
                return outputTrigger;
            });
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitTitle("Spatial: Award World Currency")]
    [UnitSurtitle("Spatial")]
    [UnitShortTitle("Award World Currency")]
    [UnitCategory("Spatial\\Space Economy")]
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
            AwardWorldCurrencyRequest request = SpatialBridge.inventoryService.AwardWorldCurrency(flow.GetValue<ulong>(amount));
            yield return request;
            flow.SetValue(succeeded, request.succeeded);

            yield return outputTrigger;
        }
    }
}
