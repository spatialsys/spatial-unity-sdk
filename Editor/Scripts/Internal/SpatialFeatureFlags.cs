using System;
using UnityEditor;
using UnityEngine;

namespace SpatialSys.UnitySDK.Editor
{
    [InitializeOnLoad]
    public static class SpatialFeatureFlags
    {
        private const double POLL_INTERVAL_SECONDS = 60 * 10; // every 10 minutes
        private const string SESSION_FEATURE_FLAGS_KEY = "SpatialSDK_FeatureFlags";
        private const string SESSION_LAST_POLL_TIME_KEY = "SpatialSDK_LastPollTime";

        public static bool enabled => !Application.isBatchMode;
        public static bool initialized => !string.IsNullOrEmpty(SessionState.GetString(SESSION_FEATURE_FLAGS_KEY, null));

        public static SpatialAPI.FeatureFlags currentFlags;
        public static Action onFetchComplete;

        private static double _lastPollTime;
        private static ulong _requestID = 0;

        static SpatialFeatureFlags()
        {
#if !SPATIAL_UNITYSDK_INTERNAL
            if (!enabled)
                return;

            EditorApplication.update += PollUpdate;
            AuthUtility.onAuthStatusChanged += OnAuthStatusChanged;

            // Load from session state cache if it's available to avoid redundant fetches.
            if (initialized)
            {
                try
                {
                    currentFlags = JsonUtility.FromJson<SpatialAPI.FeatureFlags>(SessionState.GetString(SESSION_FEATURE_FLAGS_KEY, null));
                    // Persist poll timer to avoid delaying fetch via repeatedly recompiling.
                    _lastPollTime = (double)SessionState.GetFloat(SESSION_LAST_POLL_TIME_KEY, defaultValue: (float)-POLL_INTERVAL_SECONDS);
                }
                catch (Exception exc)
                {
                    Debug.LogError($"Failed to load feature flags state from local cache: {exc}");
                }
            }
            else
            {
                // Refetch immediately on next update.
                _lastPollTime = -POLL_INTERVAL_SECONDS;
            }
#endif
        }

        private static void OnAuthStatusChanged()
        {
            // Refetch immediately on next update when logged in to a potentially new account.
            if (AuthUtility.isAuthenticated)
                _lastPollTime = -POLL_INTERVAL_SECONDS;
        }

        private static void PollUpdate()
        {
            double t = EditorApplication.timeSinceStartup;
            if (t - _lastPollTime >= POLL_INTERVAL_SECONDS)
            {
                _lastPollTime = t;
                SessionState.SetFloat(SESSION_LAST_POLL_TIME_KEY, (float)_lastPollTime);
                Refetch();
            }
        }

        public static void Refetch()
        {
            if (!enabled)
            {
                Debug.LogError("Cannot fetch feature flags because the service is disabled");
                return;
            }

            ulong currentReqID = ++_requestID;
            SpatialAPI.GetFeatureFlags()
                .Then(resp => {
                    if (_requestID != currentReqID)
                        return; // Stale; ignore request

                    currentFlags = resp.featureFlags;
                    SessionState.SetString(SESSION_FEATURE_FLAGS_KEY, JsonUtility.ToJson(currentFlags));
                    onFetchComplete?.Invoke();
                })
                .Catch(exc => {
                    Debug.LogError($"Failed to fetch feature flags: {exc}");
                });
        }
    }
}
