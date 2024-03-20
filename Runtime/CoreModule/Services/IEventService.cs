using System;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK
{
    /// <summary>
    /// This service provides alternative methods to subscribe to events, since the Creator Toolkit
    /// prohibits direct access events outside of the SpatialSDK namespace.
    /// </summary>
    /// <remarks>
    /// Spatial limits the ability to directly subscribe to C# events, UnityEvents, and to register callback functions.
    /// However, this is still possible using the utility methods in this service.
    /// </remarks>
    [DocumentationCategory("Services/Event Service")]
    public interface IEventService
    {
        /// <summary>
        /// Adds `eventHandler` to the event named `eventName` on the `target` object.
        /// Equivalent to `target.eventName += eventHandler;`
        /// </summary>
        void AddEventHandler(object target, string eventName, Delegate eventHandler);

        /// <summary>
        /// Removes `eventHandler` from the event named `eventName` on the `target` object.
        /// Equivalent to `target.eventName -= eventHandler;`
        /// </summary>
        void RemoveEventHandler(object target, string eventName, Delegate eventHandler);

        /// <summary>
        /// Adds `eventHandler` to the static event named `eventName` on the `targetType` type.
        /// Equivalent to `targetType.eventName += eventHandler;`
        /// </summary>
        void AddStaticEventHandler(Type targetType, string eventName, Delegate eventHandler);

        /// <summary>
        /// Removes `eventHandler` from the static event named `eventName` on the `targetType` type.
        /// Equivalent to `targetType.eventName -= eventHandler;`
        /// </summary>
        void RemoveStaticEventHandler(Type targetType, string eventName, Delegate eventHandler);

        /// <summary>
        /// Adds `eventHandler` to the EventHook 'hook'
        /// Equivalent to `EventBus.Register<TAargs>(hook, eventHandler);`
        /// </summary>
        void AddVisualScriptEventHandler<TArgs>(EventHook hook, Action<TArgs> eventHandler);

        /// <summary>
        /// Adds `eventHandler` from the EventHook 'hook'
        /// Equivalent to `EventBus.Unregister(hook, eventHandler);`
        /// </summary>
        void RemoveVisualScriptEventHandler(EventHook hook, Delegate eventHandler);
    }
}
