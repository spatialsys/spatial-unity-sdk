using System;
using UnityEngine;

namespace SpatialSys.UnitySDK
{
    public class SpatialAsyncOperation : CustomYieldInstruction
    {
        private event Action<SpatialAsyncOperation> _completionCallback;

        public override bool keepWaiting => !isDone;
        public bool isDone { get; private set; }
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

        public void InvokeCompletionEvent()
        {
            if (_completionCallback != null)
            {
                isDone = true;
                try
                {
                    _completionCallback(this);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error invoking completion callback for {GetType().Name}; Exception: {ex}");
                }
                _completionCallback = null;
            }
        }
    }
}