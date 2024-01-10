using UnityEngine;
using Unity.VisualScripting;
using System.Collections.Generic;
using System.Collections;
using System;
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
            ClientBridge.DataStoreScope scopeValue = flow.GetValue<ClientBridge.DataStoreScope>(scope);
            if (scopeValue == ClientBridge.DataStoreScope.UserWorldData)
            {
                DataStoreGetVariableRequest request = SpatialBridge.userWorldDataStoreService.GetVariable(flow.GetValue<string>(key), flow.GetValue<object>(defaultValue));
                yield return request;

                // Try to convert from internal types to VS types
                if (request.responseCode == DataStoreResponseCode.Ok)
                {
                    request.responseCode = DataStoreVisualScriptingTypeSupport.TryConvertFromInternalTypes(request.value, out request.value);
                    request.succeeded = request.responseCode == DataStoreResponseCode.Ok;
                }

                flow.SetValue(value, request.value);
                flow.SetValue(succeeded, request.succeeded);
                flow.SetValue(responseCode, (int)request.responseCode);
            }
            else // Unsupported scope
            {
                Debug.LogError($"{nameof(GetDataStoreVariableValueNode)}: Unsupported DataStore scope {scopeValue}");
                flow.SetValue(value, null);
                flow.SetValue(succeeded, false);
                flow.SetValue(responseCode, (int)DataStoreResponseCode.InternalError);
            }
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
            // Convert from VS types to internal types
            object variableValue = flow.GetValue<object>(value);
            DataStoreResponseCode conversionResponseCode = DataStoreVisualScriptingTypeSupport.TryConvertToInternalTypes(variableValue, out variableValue);

            // Fail early if conversion failed
            if (conversionResponseCode != DataStoreResponseCode.Ok)
            {
                flow.SetValue(succeeded, false);
                flow.SetValue(responseCode, (int)conversionResponseCode);
                yield return outputTrigger;
                yield break;
            }

            ClientBridge.DataStoreScope scopeValue = flow.GetValue<ClientBridge.DataStoreScope>(scope);
            if (scopeValue == ClientBridge.DataStoreScope.UserWorldData)
            {
                DataStoreOperationRequest request = SpatialBridge.userWorldDataStoreService.SetVariable(flow.GetValue<string>(key), variableValue);
                yield return request;
                flow.SetValue(succeeded, request.succeeded);
                flow.SetValue(responseCode, (int)request.responseCode);
            }
            else // Unsupported scope
            {
                Debug.LogError($"{nameof(SetDataStoreVariableValueNode)}: Unsupported DataStore scope {scopeValue}");
                flow.SetValue(succeeded, false);
                flow.SetValue(responseCode, (int)DataStoreResponseCode.InternalError);
            }
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
            ClientBridge.DataStoreScope scopeValue = flow.GetValue<ClientBridge.DataStoreScope>(scope);
            if (scopeValue == ClientBridge.DataStoreScope.UserWorldData)
            {
                DataStoreOperationRequest request = SpatialBridge.userWorldDataStoreService.DeleteVariable(flow.GetValue<string>(key));
                yield return request;
                flow.SetValue(succeeded, request.succeeded);
                flow.SetValue(responseCode, (int)request.responseCode);
            }
            else // Unsupported scope
            {
                Debug.LogError($"{nameof(DeleteDataStoreVariableNode)}: Unsupported DataStore scope {scopeValue}");
                flow.SetValue(succeeded, false);
                flow.SetValue(responseCode, (int)DataStoreResponseCode.InternalError);
            }
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
            ClientBridge.DataStoreScope scopeValue = flow.GetValue<ClientBridge.DataStoreScope>(scope);
            if (scopeValue == ClientBridge.DataStoreScope.UserWorldData)
            {
                DataStoreOperationRequest request = SpatialBridge.userWorldDataStoreService.ClearAllVariables();
                yield return request;
                flow.SetValue(succeeded, request.succeeded);
                flow.SetValue(responseCode, (int)request.responseCode);
            }
            else // Unsupported scope
            {
                Debug.LogError($"{nameof(ClearDataStoreNode)}: Unsupported DataStore scope {scopeValue}");
                flow.SetValue(succeeded, false);
                flow.SetValue(responseCode, (int)DataStoreResponseCode.InternalError);
            }
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
            ClientBridge.DataStoreScope scopeValue = flow.GetValue<ClientBridge.DataStoreScope>(scope);
            if (scopeValue == ClientBridge.DataStoreScope.UserWorldData)
            {
                DataStoreHasVariableRequest request = SpatialBridge.userWorldDataStoreService.HasVariable(flow.GetValue<string>(key));
                yield return request;
                flow.SetValue(exists, request.hasVariable);
                flow.SetValue(succeeded, request.succeeded);
                flow.SetValue(responseCode, (int)request.responseCode);
            }
            else // Unsupported scope
            {
                Debug.LogError($"{nameof(DataStoreVariableExistsNode)}: Unsupported DataStore scope {scopeValue}");
                flow.SetValue(exists, false);
                flow.SetValue(succeeded, false);
                flow.SetValue(responseCode, (int)DataStoreResponseCode.InternalError);
            }
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
            ClientBridge.DataStoreScope scopeValue = flow.GetValue<ClientBridge.DataStoreScope>(scope);
            if (scopeValue == ClientBridge.DataStoreScope.UserWorldData)
            {
                DataStoreHasAnyVariableRequest request = SpatialBridge.userWorldDataStoreService.HasAnyVariable();
                yield return request;
                flow.SetValue(hasAny, request.hasAnyVariable);
                flow.SetValue(succeeded, request.succeeded);
                flow.SetValue(responseCode, (int)request.responseCode);
            }
            else // Unsupported scope
            {
                Debug.LogError($"{nameof(DataStoreHasAnyVariablesNode)}: Unsupported DataStore scope {scopeValue}");
                flow.SetValue(hasAny, false);
                flow.SetValue(succeeded, false);
                flow.SetValue(responseCode, (int)DataStoreResponseCode.InternalError);
            }
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
            ClientBridge.DataStoreScope scopeValue = flow.GetValue<ClientBridge.DataStoreScope>(scope);
            if (scopeValue == ClientBridge.DataStoreScope.UserWorldData)
            {
                DataStoreDumpVariablesRequest request = SpatialBridge.userWorldDataStoreService.DumpVariablesAsJSON();
                yield return request;
                flow.SetValue(json, request.json);
                flow.SetValue(succeeded, request.succeeded);
                flow.SetValue(responseCode, (int)request.responseCode);
            }
            else // Unsupported scope
            {
                Debug.LogError($"{nameof(DumpDataStoreVariablesNode)}: Unsupported DataStore scope {scopeValue}");
                flow.SetValue(json, null);
                flow.SetValue(succeeded, false);
                flow.SetValue(responseCode, (int)DataStoreResponseCode.InternalError);
            }
            yield return outputTrigger;
        }
    }

    public static class DataStoreVisualScriptingTypeSupport
    {
        private static readonly HashSet<Type> SUPPORTED_PRIMITIVE_ARRAY_TYPES = new() {
            typeof(string),
            typeof(bool),
            typeof(int),
            typeof(float),
        };

        /// <summary>
        /// Converts arrays and dictionary internal type representations to AotList and AotDictionary types
        /// </summary>
        public static DataStoreResponseCode TryConvertFromInternalTypes(object value, out object result)
        {
            try
            {
                if (value is string[] || value is bool[] || value is int[] || value is float[])
                {
                    Array arrayValue = (Array)value;
                    AotList aotList = new(arrayValue.Length);
                    for (int i = 0; i < arrayValue.Length; i++)
                        aotList.Add(arrayValue.GetValue(i));
                    result = aotList;
                    return DataStoreResponseCode.Ok;
                }
                else if (value is Dictionary<string, object> dict)
                {
                    AotDictionary aotDict = new(dict.Count);
                    foreach (KeyValuePair<string, object> kvp in dict)
                    {
                        DataStoreResponseCode responseCode = TryConvertFromInternalTypes(kvp.Value, out object convertedValue);
                        if (responseCode != DataStoreResponseCode.Ok)
                        {
                            result = null;
                            return responseCode;
                        }

                        aotDict.Add(kvp.Key, convertedValue);
                    }
                    result = aotDict;
                    return DataStoreResponseCode.Ok;
                }
                else
                {
                    result = value;
                    return DataStoreResponseCode.Ok;
                }
            }
            catch
            {
                result = null;
                return DataStoreResponseCode.UnknownError;
            }
        }

        /// <summary>
        /// Converts AotList and AotDictionary types to primitive array type like int[] and Dictionary<string, object>
        /// </summary>
        public static DataStoreResponseCode TryConvertToInternalTypes(object value, out object result)
        {
            if (value is AotList aotList)
            {
                // If list is empty, we default to most common
                // This may convert existing type to string but this is unavoidable since we can't know what the type is
                if (aotList.Count == 0)
                {
                    result = new string[0];
                    return DataStoreResponseCode.Ok;
                }

                // Try to get the type for the list
                object firstNonNullElementValue = null;
                for (int i = 0; i < aotList.Count; i++)
                {
                    if (aotList[i] != null)
                    {
                        firstNonNullElementValue = aotList[i];
                        break;
                    }
                }
                Type typeOfFirstElement;
                if (firstNonNullElementValue == null)
                {
                    typeOfFirstElement = typeof(string);
                }
                else
                {
                    typeOfFirstElement = firstNonNullElementValue.GetType();
                }

                if (!SUPPORTED_PRIMITIVE_ARRAY_TYPES.Contains(typeOfFirstElement))
                {
                    result = null;
                    return DataStoreResponseCode.UnsupportedValueType;
                }

                Array valueArray;
                if (firstNonNullElementValue is string || firstNonNullElementValue == null)
                {
                    valueArray = new string[aotList.Count];
                }
                else if (firstNonNullElementValue is bool)
                {
                    valueArray = new bool[aotList.Count];
                }
                else if (firstNonNullElementValue is int)
                {
                    valueArray = new int[aotList.Count];
                }
                else if (firstNonNullElementValue is float)
                {
                    valueArray = new float[aotList.Count];
                }
                else
                {
                    result = null;
                    return DataStoreResponseCode.UnsupportedValueType;
                }

                for (int i = 0; i < aotList.Count; i++)
                    valueArray.SetValue(aotList[i], i); // Let it fail with cast exceptions here; Try/catch would be too slow

                result = valueArray;
                return DataStoreResponseCode.Ok;
            }
            else if (value is AotDictionary aotDict)
            {
                Dictionary<string, object> dict = new(aotDict.Count);
                foreach (object key in aotDict.Keys)
                {
                    if (key is not string)
                    {
                        result = null;
                        return DataStoreResponseCode.UnsupportedDictionaryKeyType;
                    }

                    DataStoreResponseCode responseCode = TryConvertToInternalTypes(aotDict[key], out object convertedValue);
                    if (responseCode != DataStoreResponseCode.Ok)
                    {
                        result = null;
                        return responseCode;
                    }

                    dict.Add((string)key, convertedValue);
                }

                result = dict;
                return DataStoreResponseCode.Ok;
            }
            else
            {
                result = value;
                return DataStoreResponseCode.Ok;
            }
        }
    }
}