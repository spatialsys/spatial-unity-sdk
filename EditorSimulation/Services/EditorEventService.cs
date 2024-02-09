using System;
using System.Reflection;
using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.EditorSimulation
{
    public class EditorEventService : IEventService
    {
        public void AddEventHandler(object target, string eventName, Delegate eventHandler)
        {
            EventInfo eventInfo = target.GetType().GetEvent(eventName);
            ValidateEvent(eventInfo, target.GetType(), eventName, eventHandler);
            eventInfo.AddEventHandler(target, eventHandler);
        }

        public void RemoveEventHandler(object target, string eventName, Delegate eventHandler)
        {
            EventInfo eventInfo = target.GetType().GetEvent(eventName);
            ValidateEvent(eventInfo, target.GetType(), eventName, eventHandler);
            eventInfo.RemoveEventHandler(target, eventHandler);
        }

        public void AddStaticEventHandler(Type targetType, string eventName, Delegate eventHandler)
        {
            EventInfo eventInfo = targetType.GetEvent(eventName);
            ValidateEvent(eventInfo, targetType, eventName, eventHandler);
            eventInfo.AddEventHandler(null, eventHandler);
        }

        public void RemoveStaticEventHandler(Type targetType, string eventName, Delegate eventHandler)
        {
            EventInfo eventInfo = targetType.GetEvent(eventName);
            ValidateEvent(eventInfo, targetType, eventName, eventHandler);
            eventInfo.RemoveEventHandler(null, eventHandler);
        }

        public void AddVisualScriptEventHandler<TArgs>(EventHook hook, Action<TArgs> handler)
        {
            EventBus.Register<TArgs>(hook, handler);
        }

        public void RemoveVisualScriptEventHandler(EventHook hook, Delegate handler)
        {
            EventBus.Unregister(hook, handler);
        }

        private void ValidateEvent(EventInfo eventInfo, Type targetType, string eventName, Delegate eventHandler)
        {
            if (eventInfo == null)
            {
                throw new ArgumentException($"Event '{eventName}' not found on type '{targetType}'.");
            }
            if (!eventInfo.EventHandlerType.IsAssignableFrom(eventHandler.GetType()))
            {
                throw new ArgumentException("The provided delegate does not match the event's delegate type.");
            }
        }
    }
}
