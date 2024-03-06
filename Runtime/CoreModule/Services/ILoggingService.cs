
using System;
using System.Collections.Generic;

namespace SpatialSys.UnitySDK
{
    /// <summary>
    /// This currently simply logs errors to the console, but in the future we may provide a service that logs to a
    /// remote server which can be viewed by the developer.
    /// </summary>
    [DocumentationCategory("Logging Service")]
    public interface ILoggingService
    {
        // Future
        // void LogDebug(string message, Dictionary<string, object> data = null);
        // void LogInfo(string message, Dictionary<string, object> data = null);
        // void LogWarning(string message, Dictionary<string, object> data = null);

        /// <summary>
        /// Logs an error to the console, nicely formatted.
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="exception">Optional exception to log</param>
        /// <param name="data">Optional additional data</param>
        void LogError(string message, Exception exception = null, Dictionary<string, object> data = null);

        /// <summary>
        /// Logs an error to the console, nicely formatted.
        /// </summary>
        /// <param name="exception">The exception to log</param>
        /// <param name="data">Optional additional data</param>
        void LogError(Exception exception, Dictionary<string, object> data = null);
    }
}
