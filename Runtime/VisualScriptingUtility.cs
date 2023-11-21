using SpatialSys.UnitySDK.VisualScripting;
using System;
using UnityEngine;
using Unity.VisualScripting;

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
    }
}