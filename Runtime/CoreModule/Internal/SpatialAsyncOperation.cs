using System;
using UnityEngine;

namespace SpatialSys.UnitySDK
{
    /// <summary>
    /// Base class for all Spatial's async operations. This class is yieldable and can be used in coroutines.
    /// </summary>
    public class SpatialAsyncOperation : CustomYieldInstruction
    {
        private event Action<SpatialAsyncOperation> _completionCallback;

        /// <summary>
        /// Returns true if the operation is not done.
        /// </summary>
        public override bool keepWaiting => !isDone;

        /// <summary>
        /// Returns true if the operation is done.
        /// </summary>
        public bool isDone { get; private set; }

        /// <summary>
        /// Event that is invoked when the operation is completed.
        /// </summary>
        public event Action<SpatialAsyncOperation> completed
        {
            add
            {
                if (isDone)
                {
                    value(this);
                }
                else
                {
                    _completionCallback = (Action<SpatialAsyncOperation>)Delegate.Combine(_completionCallback, value);
                }
            }
            remove
            {
                _completionCallback = (Action<SpatialAsyncOperation>)Delegate.Remove(_completionCallback, value);
            }
        }

        /// <summary>
        /// Invokes the completion event.
        /// </summary>
        public void InvokeCompletionEvent()
        {
            if (isDone)
                return;

            isDone = true;
            if (_completionCallback == null)
                return;

            try
            {
                _completionCallback(this);
            }
            catch (Exception ex)
            {
                SpatialBridge.loggingService.LogError($"Error invoking completion callback for {GetType().Name}; Exception: {ex}");
            }
            _completionCallback = null;
        }
    }

    /// <summary>
    /// Extension methods for SpatialAsyncOperation.
    /// </summary>
    public static class SpatialAsyncOperationExtensions
    {
        /// <summary>
        /// Sets the completion event, same as setting the event using the completed property, but returns
        /// the operation itself.
        /// </summary>
        /// <typeparam name="T">class deriving from SpatialAsyncOperation</typeparam>
        /// <param name="operation">SpatialAsyncOperation type</param>
        /// <param name="callback">Callback function</param>
        /// <returns>Returns the same operation</returns>
        public static T SetCompletedEvent<T>(this T operation, Action<T> callback) where T : SpatialAsyncOperation
        {
            // Ideally this would be a cast, but Action<T:SpatialAsyncOperation> can't be cast to Action<SpatialAsyncOperation>
            operation.completed += _ => callback(operation);
            return operation;
        }
    }
}