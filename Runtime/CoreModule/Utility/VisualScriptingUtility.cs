using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace SpatialSys.UnitySDK
{
    /// <summary>
    /// A utility class that assists with communicating between Visual Scripting and C#.
    /// </summary>
    /// <example>
    /// <code source="Services/VisualScriptingUtilityExamples.cs" region="VSUtilTriggerCustomEvent" lang="csharp"></code>
    /// </example>
    public static class VisualScriptingUtility
    {
        /// <summary>
        /// Add a listener for a custom visual scripting event.
        /// </summary>
        /// <param name="gameObject">The gameobject that we are listening on. This should be the object we expect to trigger the event.</param>
        /// <param name="handler">The callback that will be handling any events raised.</param>
        /// <returns>Returns a delegate we can use to later unsubscribe the listener.</returns>
        public static Delegate AddCustomEventListener(GameObject gameObject, Action<string, object[]> handler)
        {
            Action<CustomEventArgs> intermediateHandler = (ev) => handler(ev.name, ev.arguments);
            SpatialBridge.eventService.AddVisualScriptEventHandler(new EventHook(EventHooks.Custom, gameObject), intermediateHandler);
            return intermediateHandler;
        }

        /// <summary>
        /// Remove a listener for a custom visual scripting event.
        /// </summary>
        /// <param name="gameObject">The gameobject that we are listening on. This should be the same gameobject used to add the custom event listener.</param>
        /// <param name="handler">The delegate we are unsubscribing. This should be the delegate we received from <see cref="AddCustomEventListener"/>.</param>
        public static void RemoveCustomEventListener(GameObject gameObject, Delegate handler)
        {
            SpatialBridge.eventService.RemoveVisualScriptEventHandler(new EventHook(EventHooks.Custom, gameObject), handler);
        }

        /// <summary>
        /// Trigger a custom visual scripting event.
        /// </summary>
        /// <example>
        /// When called, all ScriptMachines on the target gameobject will trigger the <see href="https://docs.unity3d.com/Packages/com.unity.visualscripting@1.7/manual/vs-custom-events.html">custom events</see> that have the matching event name.
        /// <code source="Services/VisualScriptingUtilityExamples.cs" region="VSUtilTriggerCustomEvent" lang="csharp"></code>
        /// </example>
        /// <param name="target">The target gameobject we want to trigger the event on. All script machines on this gameobject will receive the event.</param>
        /// <param name="message">The name of the event.</param>
        /// <param name="args">The args for the event.</param>
        public static void TriggerCustomEvent(GameObject target, string message, params object[] args)
        {
            EventBus.Trigger(new EventHook(EventHooks.Custom, target), new CustomEventArgs(message, args));
        }

        /// <summary>
        /// Convert an AotList to a List of a given type. 
        /// </summary>
        /// <remarks>
        /// Visual scripting uses special AOT types for collections that are not necessary in C#. This method is used to convert these AOT types to standard C# types.
        /// </remarks>
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

        /// <summary>
        /// Convert an AOTDiction to a List of a given type. 
        /// </summary>
        /// <remarks>
        /// Visual scripting uses special AOT types for collections that are not necessary in C#. This method is used to convert these AOT types to standard C# types.
        /// </remarks>
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