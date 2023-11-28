using System.Collections;
using UnityEngine;
using Unity.VisualScripting;
using SpatialSys.UnitySDK.Internal;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial Data Store: Get Variable")]
    [UnitSurtitle("Spatial Data Store")]
    [UnitShortTitle("Get Variable")]
    [UnitCategory("Spatial\\DataStore")]
    [TypeIcon(typeof(DataStoreIcon))]
    public class GetDataStoreVariableValueNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput scope { get; private set; }
        [DoNotSerialize]
        public ValueInput key { get; private set; }
        [DoNotSerialize]
        public ValueInput defaultValue { get; private set; }

        [DoNotSerialize]
        public ValueOutput value { get; private set; }
        [DoNotSerialize]
        public ValueOutput succeeded { get; private set; }
        [DoNotSerialize]
        public ValueOutput responseCode { get; private set; }

        protected override void Definition()
        {
            scope = ValueInput<ClientBridge.DataStoreScope>(nameof(scope), ClientBridge.DataStoreScope.UserWorldData);
            key = ValueInput<string>(nameof(key), "");
            defaultValue = ValueInput<object>(nameof(defaultValue), null);

            value = ValueOutput<object>(nameof(value));
            succeeded = ValueOutput<bool>(nameof(succeeded));
            responseCode = ValueOutput<int>(nameof(responseCode));

            inputTrigger = ControlInputCoroutine(nameof(inputTrigger), ExecuteAsync);
            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }

        private IEnumerator ExecuteAsync(Flow flow)
        {
            bool completed = false;
            SpatialBridge.GetDataStoreVariableValue?.Invoke(
                flow.GetValue<ClientBridge.DataStoreScope>(scope),
                flow.GetValue<string>(key),
                flow.GetValue<object>(defaultValue),
                result => {
                    completed = true;
                    flow.SetValue(value, result.value);
                    flow.SetValue(succeeded, result.succeeded);
                    flow.SetValue(responseCode, result.responseCode);
                }
            );
            yield return new WaitUntil(() => completed);
            yield return outputTrigger;
        }
    }

    [UnitTitle("Spatial Data Store: Set Variable")]
    [UnitSurtitle("Spatial Data Store")]
    [UnitShortTitle("Set Variable")]
    [UnitCategory("Spatial\\DataStore")]
    [TypeIcon(typeof(DataStoreIcon))]
    public class SetDataStoreVariableValueNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput scope { get; private set; }
        [DoNotSerialize]
        public ValueInput key { get; private set; }
        [DoNotSerialize]
        public ValueInput value { get; private set; }

        [DoNotSerialize]
        public ValueOutput succeeded { get; private set; }
        [DoNotSerialize]
        public ValueOutput responseCode { get; private set; }

        protected override void Definition()
        {
            scope = ValueInput<ClientBridge.DataStoreScope>(nameof(scope), ClientBridge.DataStoreScope.UserWorldData);
            key = ValueInput<string>(nameof(key), "");
            value = ValueInput<object>(nameof(value));

            succeeded = ValueOutput<bool>(nameof(succeeded));
            responseCode = ValueOutput<int>(nameof(responseCode));

            inputTrigger = ControlInputCoroutine(nameof(inputTrigger), ExecuteAsync);
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }

        private IEnumerator ExecuteAsync(Flow flow)
        {
            bool completed = false;
            SpatialBridge.SetDataStoreVariableValue?.Invoke(
                scope: flow.GetValue<ClientBridge.DataStoreScope>(scope),
                key: flow.GetValue<string>(key),
                value: flow.GetValue<object>(value),
                result => {
                    completed = true;
                    flow.SetValue(succeeded, result.succeeded);
                    flow.SetValue(responseCode, result.responseCode);
                }
            );
            yield return new WaitUntil(() => completed);
            yield return outputTrigger;
        }
    }

    [UnitTitle("Spatial Data Store: Delete Variable")]
    [UnitSurtitle("Spatial Data Store")]
    [UnitShortTitle("Delete Variable")]
    [UnitCategory("Spatial\\DataStore")]
    [TypeIcon(typeof(DataStoreIcon))]
    public class DeleteDataStoreVariableNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput scope { get; private set; }
        [DoNotSerialize]
        public ValueInput key { get; private set; }

        [DoNotSerialize]
        public ValueOutput succeeded { get; private set; }
        [DoNotSerialize]
        public ValueOutput responseCode { get; private set; }

        protected override void Definition()
        {
            scope = ValueInput<ClientBridge.DataStoreScope>(nameof(scope), ClientBridge.DataStoreScope.UserWorldData);
            key = ValueInput<string>(nameof(key), "");

            succeeded = ValueOutput<bool>(nameof(succeeded));
            responseCode = ValueOutput<int>(nameof(responseCode));

            inputTrigger = ControlInputCoroutine(nameof(inputTrigger), ExecuteAsync);
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }

        private IEnumerator ExecuteAsync(Flow flow)
        {
            bool completed = false;
            SpatialBridge.DeleteDataStoreVariable?.Invoke(
                scope: flow.GetValue<ClientBridge.DataStoreScope>(scope),
                key: flow.GetValue<string>(key),
                result => {
                    completed = true;
                    flow.SetValue(succeeded, result.succeeded);
                    flow.SetValue(responseCode, result.responseCode);
                }
            );
            yield return new WaitUntil(() => completed);
            yield return outputTrigger;
        }
    }

    [UnitTitle("Spatial Data Store: Clear All Variables")]
    [UnitSurtitle("Spatial Data Store")]
    [UnitShortTitle("Clear All Variables")]
    [UnitCategory("Spatial\\DataStore")]
    [TypeIcon(typeof(DataStoreIcon))]
    public class ClearDataStoreNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput scope { get; private set; }

        [DoNotSerialize]
        public ValueOutput succeeded { get; private set; }
        [DoNotSerialize]
        public ValueOutput responseCode { get; private set; }

        protected override void Definition()
        {
            scope = ValueInput<ClientBridge.DataStoreScope>(nameof(scope), ClientBridge.DataStoreScope.UserWorldData);

            succeeded = ValueOutput<bool>(nameof(succeeded));
            responseCode = ValueOutput<int>(nameof(responseCode));

            inputTrigger = ControlInputCoroutine(nameof(inputTrigger), ExecuteAsync);
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }

        private IEnumerator ExecuteAsync(Flow flow)
        {
            bool completed = false;
            SpatialBridge.ClearDataStore?.Invoke(
                scope: flow.GetValue<ClientBridge.DataStoreScope>(scope),
                result => {
                    completed = true;
                    flow.SetValue(succeeded, result.succeeded);
                    flow.SetValue(responseCode, result.responseCode);
                }
            );
            yield return new WaitUntil(() => completed);
            yield return outputTrigger;
        }
    }

    [UnitTitle("Spatial Data Store: Has Variable")]
    [UnitSurtitle("Spatial Data Store")]
    [UnitShortTitle("Has Variable")]
    [UnitCategory("Spatial\\DataStore")]
    [TypeIcon(typeof(DataStoreIcon))]
    public class DataStoreVariableExistsNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput scope { get; private set; }
        [DoNotSerialize]
        public ValueInput key { get; private set; }

        [DoNotSerialize]
        public ValueOutput exists { get; private set; }
        [DoNotSerialize]
        public ValueOutput succeeded { get; private set; }
        [DoNotSerialize]
        public ValueOutput responseCode { get; private set; }

        protected override void Definition()
        {
            scope = ValueInput<ClientBridge.DataStoreScope>(nameof(scope), ClientBridge.DataStoreScope.UserWorldData);
            key = ValueInput<string>(nameof(key), "");

            exists = ValueOutput<bool>(nameof(exists));
            succeeded = ValueOutput<bool>(nameof(succeeded));
            responseCode = ValueOutput<int>(nameof(responseCode));

            inputTrigger = ControlInputCoroutine(nameof(inputTrigger), ExecuteAsync);
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }

        private IEnumerator ExecuteAsync(Flow flow)
        {
            bool completed = false;
            SpatialBridge.HasDataStoreVariable?.Invoke(
                scope: flow.GetValue<ClientBridge.DataStoreScope>(scope),
                key: flow.GetValue<string>(key),
                result => {
                    completed = true;
                    flow.SetValue(exists, (bool)result.value == true);
                    flow.SetValue(succeeded, result.succeeded);
                    flow.SetValue(responseCode, result.responseCode);
                }
            );
            yield return new WaitUntil(() => completed);
            yield return outputTrigger;
        }
    }

    [UnitTitle("Spatial Data Store: Has Any Variable")]
    [UnitSurtitle("Spatial Data Store")]
    [UnitShortTitle("Has Any Variable")]
    [UnitCategory("Spatial\\DataStore")]
    [TypeIcon(typeof(DataStoreIcon))]
    public class DataStoreHasAnyVariablesNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput scope { get; private set; }

        [DoNotSerialize]
        public ValueOutput hasAny { get; private set; }
        [DoNotSerialize]
        public ValueOutput succeeded { get; private set; }
        [DoNotSerialize]
        public ValueOutput responseCode { get; private set; }

        protected override void Definition()
        {
            scope = ValueInput<ClientBridge.DataStoreScope>(nameof(scope), ClientBridge.DataStoreScope.UserWorldData);

            hasAny = ValueOutput<bool>(nameof(hasAny));
            succeeded = ValueOutput<bool>(nameof(succeeded));
            responseCode = ValueOutput<int>(nameof(responseCode));

            inputTrigger = ControlInputCoroutine(nameof(inputTrigger), ExecuteAsync);
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }

        private IEnumerator ExecuteAsync(Flow flow)
        {
            bool completed = false;
            SpatialBridge.DataStoreHasAnyVariable?.Invoke(
                scope: flow.GetValue<ClientBridge.DataStoreScope>(scope),
                result => {
                    completed = true;
                    flow.SetValue(hasAny, (bool)result.value == true);
                    flow.SetValue(succeeded, result.succeeded);
                    flow.SetValue(responseCode, result.responseCode);
                }
            );
            yield return new WaitUntil(() => completed);
            yield return outputTrigger;
        }
    }

    [UnitTitle("Spatial Data Store: Dump as JSON String")]
    [UnitSurtitle("Spatial Data Store")]
    [UnitShortTitle("Dump as JSON String")]
    [UnitCategory("Spatial\\DataStore")]
    [TypeIcon(typeof(DataStoreIcon))]
    public class DumpDataStoreVariablesNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput scope { get; private set; }

        [DoNotSerialize]
        public ValueOutput json { get; private set; }
        [DoNotSerialize]
        public ValueOutput succeeded { get; private set; }
        [DoNotSerialize]
        public ValueOutput responseCode { get; private set; }

        protected override void Definition()
        {
            scope = ValueInput<ClientBridge.DataStoreScope>(nameof(scope), ClientBridge.DataStoreScope.UserWorldData);

            json = ValueOutput<string>(nameof(json));
            succeeded = ValueOutput<bool>(nameof(succeeded));
            responseCode = ValueOutput<int>(nameof(responseCode));

            inputTrigger = ControlInputCoroutine(nameof(inputTrigger), ExecuteAsync);
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }

        private IEnumerator ExecuteAsync(Flow flow)
        {
            bool completed = false;
            SpatialBridge.DumpDataStoreVariables?.Invoke(
                scope: flow.GetValue<ClientBridge.DataStoreScope>(scope),
                result => {
                    completed = true;
                    flow.SetValue(json, (string)result.value);
                    flow.SetValue(succeeded, result.succeeded);
                    flow.SetValue(responseCode, result.responseCode);
                }
            );
            yield return new WaitUntil(() => completed);
            yield return outputTrigger;
        }
    }
}