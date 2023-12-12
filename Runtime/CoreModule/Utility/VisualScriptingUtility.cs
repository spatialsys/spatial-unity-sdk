using SpatialSys.UnitySDK.VisualScripting;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace SpatialSys.UnitySDK
{
    public static class VisualScriptingUtility
    {
        public static Delegate AddCustomEventListener(GameObject gameObject, Action<string, object[]> handler)
        {
            Action<CustomEventArgs> intermediateHandler = (ev) => handler(ev.name, ev.arguments);
            EventBus.Register<CustomEventArgs>(new EventHook(EventHooks.Custom, gameObject), intermediateHandler);
            return intermediateHandler;
        }

        public static void RemoveCustomEventListener(GameObject gameObject, Delegate handler)
        {
            EventBus.Unregister(new EventHook(EventHooks.Custom, gameObject), handler);
        }

        public static void TriggerCustomEvent(GameObject target, string message, params object[] args)
        {
            EventBus.Trigger(new EventHook(EventHooks.Custom, target), new CustomEventArgs(message, args));
        }

        /// <summary>
        /// Convert the Spatial client platform to the scripting platform enum type.
        /// </summary>
        public static SpatialPlatform ConvertClientPlatformToScriptingPlatform(int clientPlatform)
        {
            switch (clientPlatform)
            {
                case 0:
                    return SpatialPlatform.Web;
                case 2:
                case 3:
                    return SpatialPlatform.Mobile;
                case 4:
                    return SpatialPlatform.MetaQuest;
                default:
                    return SpatialPlatform.Unknown;
            }
        }

        public static List<T> ToList<T>(this AotList aotList)
        {
            if (aotList == null)
                return null;

            List<T> result = new(capacity: aotList.Count);
            string expectedValueTypeName = typeof(T).Name;
            foreach (object value in aotList)
            {
                if (value is T convertedValue)
                {
                    result.Add(convertedValue);
                }
                else
                {
                    throw new System.Exception($"All list entries must be of type {expectedValueTypeName}, but found {value?.GetType().Name ?? "null"} (value={value?.ToString() ?? "null"})");
                }
            }

            return result;
        }

        public static Dictionary<TKey, object> ToDictionary<TKey>(this AotDictionary aotDict)
        {
            if (aotDict == null)
                return null;

            Dictionary<TKey, object> result = new(capacity: aotDict.Count);
            string expectedKeyTypeName = typeof(TKey).Name;
            foreach (DictionaryEntry entry in aotDict)
            {
                if (entry.Key is TKey convertedKey)
                {
                    result[convertedKey] = entry.Value;
                }
                else
                {
                    throw new System.Exception($"All dictionary keys must be of type {expectedKeyTypeName}, but found {entry.Key?.GetType().Name ?? "null"} (value={entry.Key?.ToString() ?? "null"})");
                }
            }

            return result;
        }
    }
}