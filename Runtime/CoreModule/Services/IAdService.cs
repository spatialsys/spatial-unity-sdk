using System;
using UnityEngine;

namespace SpatialSys.UnitySDK
{
    /// <summary>
    /// Type of ad to show.
    /// </summary>
    [DocumentationCategory("Services/Ad Service")]
    public enum SpatialAdType
    {
        /// <summary>
        /// No specific type.
        /// </summary>
        None = 0,
        /// <summary>
        /// Mid game type.
        /// </summary>
        MidGame = 1,
        /// <summary>
        /// Rewarded ad.
        /// </summary>
        Rewarded = 2,
    }

    /// <summary>
    /// Service to handle ads integration
    /// </summary>
    /// <example><code source="Services/AdServiceExamples.cs" region="WatchAd"/></example>
    [DocumentationCategory("Services/Ad Service")]
    public interface IAdService
    {
        /// <summary>
        /// Returns true if the platform supports ads.
        /// </summary>
        /// <example><code source="Services/AdServiceExamples.cs" region="WatchAd"/></example>
        bool isSupported { get; }

        /// <summary>
        /// Request an ad to show up.
        /// </summary>
        /// <example><code source="Services/AdServiceExamples.cs" region="WatchAd"/></example>
        /// <param name="adType">Type of ad to show.</param>
        /// <returns>Ad Request async operation.</returns>
        AdRequest RequestAd(SpatialAdType adType);
    }

    /// <summary>
    /// Operation to request an ad to show up
    /// </summary>
    /// <example><code source="Services/AdServiceExamples.cs" region="WatchAd"/></example>
    [DocumentationCategory("Services/Ad Service")]
    public class AdRequest : SpatialAsyncOperation
    {
        private event Action<SpatialAsyncOperation> _startedCallback;

        /// <summary>
        /// The type of ad that was requested.
        /// </summary>
        public SpatialAdType adType;

        /// <summary>
        /// True if the award request succeeded
        /// </summary>
        public bool succeeded;

        /// <summary>
        /// Returns true if the request has started.
        /// </summary>
        public bool hasStarted { get; private set; }

        /// <summary>
        /// Event that is invoked when the operation has started.
        /// </summary>
        public event Action<SpatialAsyncOperation> started
        {
            add
            {
                if (hasStarted)
                {
                    value(this);
                }
                else
                {
                    _startedCallback = (Action<SpatialAsyncOperation>)Delegate.Combine(_startedCallback, value);
                }
            }
            remove
            {
                _startedCallback = (Action<SpatialAsyncOperation>)Delegate.Remove(_startedCallback, value);
            }
        }

        /// <summary>
        /// Sets the started event, same as setting the event using the started property, but returns
        /// the operation itself.
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public SpatialAsyncOperation SetStartedEvent(Action<SpatialAsyncOperation> callback)
        {
            started += callback;
            return this;
        }

        /// <summary>
        /// Invokes the completion event.
        /// </summary>
        public void InvokeStartedEvent()
        {
            if (hasStarted)
                return;

            hasStarted = true;
            if (_startedCallback == null)
                return;

            try
            {
                _startedCallback(this);
            }
            catch (Exception ex)
            {
                SpatialBridge.loggingService.LogError($"Error invoking started callback for {GetType().Name}; Exception: {ex}");
            }
            _startedCallback = null;
        }
    }
}