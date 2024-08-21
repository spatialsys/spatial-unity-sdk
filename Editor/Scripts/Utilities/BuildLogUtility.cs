using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SpatialSys.UnitySDK.Editor
{
    /// <summary>
    /// Utility class for handling build logs
    /// </summary>
    public static class BuildLogUtility
    {
        private const string PREFS_LAST_BUILD_ERRORS = "SpatialSDK_LastBuildErrors";
        private const string PREFS_LAST_BUILD_WARNINGS = "SpatialSDK_LastBuildWarnings";
        private const string SPLIT_STR = "\n\n";

        private static List<string> _errors = new List<string>();
        private static List<string> _warnings = new List<string>();

        private static void HandleLogMessageReceived(string logString, string stackTrace, LogType type)
        {
            // Only include error logs on the Assets folder scripts
            if (!logString.StartsWith("Assets"))
                return;

            // Only store logstrings, stack is unnecesary as it's a compilation error and there's no relevant stack.
            if (type == LogType.Error || type == LogType.Exception)
            {
                _errors.Add(logString);
            }
            else if (type == LogType.Warning)
            {
                _warnings.Add(logString);
            }
        }

        /// <summary>
        /// Begin capturing build logs
        /// </summary>
        public static void BeginCapturingBuildLogs()
        {
            // Handle log messages from compiling process
            Application.logMessageReceived += HandleLogMessageReceived;
            _errors.Clear();
            _warnings.Clear();
        }

        /// <summary>
        /// End capturing build logs and store them in SessionState
        /// </summary>
        public static void EndCapturingBuildLogs()
        {
            // Handle log messages from compiling process
            Application.logMessageReceived -= HandleLogMessageReceived;
            if (_errors.Count > 0)
            {
                SessionState.SetString(PREFS_LAST_BUILD_ERRORS, string.Join(SPLIT_STR, _errors));
            }
            if (_warnings.Count > 0)
            {
                SessionState.SetString(PREFS_LAST_BUILD_WARNINGS, string.Join(SPLIT_STR, _warnings));
            }
        }

        /// <summary>
        /// Print previous build logs if there are any
        /// </summary>
        public static void PrintPreviousBuildLogs()
        {
            bool GetAndDeleteKey(string key, out string[] values)
            {
                string value = SessionState.GetString(key, "");
                SessionState.EraseString(key);

                if (string.IsNullOrEmpty(value))
                {
                    values = null;
                    return false;
                }
                values = value.Split(SPLIT_STR);
                return true;
            }

            if (GetAndDeleteKey(PREFS_LAST_BUILD_ERRORS, out string[] errors))
            {
                foreach (var error in errors)
                {
                    Debug.LogError(error);
                }
            }
            if (GetAndDeleteKey(PREFS_LAST_BUILD_WARNINGS, out string[] warnings))
            {
                foreach (var warning in warnings)
                {
                    Debug.LogWarning(warning);
                }
            }
        }
    }
}