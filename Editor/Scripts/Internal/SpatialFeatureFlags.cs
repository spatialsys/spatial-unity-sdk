using RSG;
using System;
using UnityEditor;
using UnityEngine;

namespace SpatialSys.UnitySDK.Editor
{
    [InitializeOnLoad]
    public static class SpatialFeatureFlags
    {
        private const double FETCH_COOLDOWN_SECONDS = 5.0;
        private const double POLL_INTERVAL_SECONDS = 60 * 10; // every 10 minutes
        private const string SESSION_FEATURE_FLAGS_KEY = "SpatialSDK_FeatureFlags";
        private const string SESSION_LAST_FETCH_TIME_KEY = "SpatialSDK_LastFetchTime";

#if SPATIAL_UNITYSDK_INTERNAL
        public static bool enabled => false; // don't change -- force disabled
#else
        public static bool enabled => !Application.isBatchMode;
#endif

        public static bool initialized => !string.IsNullOrEmpty(SessionState.GetString(SESSION_FEATURE_FLAGS_KEY, defaultValue: null));

        public static SpatialAPI.FeatureFlags currentFlags;
        public static Action onFetchComplete;

        private static double _lastFetchTime;
        private static ulong _requestID = 0;

        static SpatialFeatureFlags()
        {
            if (!enabled)
                return;

            // Attempt to reload from cache on recompiles to avoid redundant fetches.
            if (initialized)
            {
                try
                {
                    string flagsJson = SessionState.GetString(SESSION_FEATURE_FLAGS_KEY, defaultValue: null);
                    currentFlags = !string.IsNullOrEmpty(flagsJson) ? JsonUtility.FromJson<SpatialAPI.FeatureFlags>(flagsJson) : default;
                    _lastFetchTime = (double)SessionState.GetFloat(SESSION_LAST_FETCH_TIME_KEY, defaultValue: (float)-POLL_INTERVAL_SECONDS);
                }
                catch (Exception exc)
                {
                    Debug.LogError($"Failed to load feature flags state from local cache: {exc}");
                }
            }

            AuthUtility.onAuthStatusChanged += OnAuthStatusChanged;
            OnAuthStatusChanged();
        }

        private static void OnAuthStatusChanged()
        {
            EditorApplication.update -= PollUpdate;

            if (AuthUtility.isAuthenticated)
            {
                // Reset timer to refetch immediately on next update.
                _lastFetchTime = -POLL_INTERVAL_SECONDS;
                EditorApplication.update += PollUpdate;
            }
        }

        private static void PollUpdate()
        {
            if (!enabled)
                return;

            if (EditorApplication.timeSinceStartup - _lastFetchTime >= POLL_INTERVAL_SECONDS)
                Refetch();
        }

        public static IPromise Refetch()
        {
            if (!enabled)
                throw new System.Exception("Cannot fetch feature flags because the service is disabled");

            double t = EditorApplication.timeSinceStartup;
            double timeSinceLastFetch = t - _lastFetchTime;
            _lastFetchTime = t;
            SessionState.SetFloat(SESSION_LAST_FETCH_TIME_KEY, (float)_lastFetchTime);

            if (timeSinceLastFetch < FETCH_COOLDOWN_SECONDS)
                return Promise.Resolved();

            ulong currentReqID = ++_requestID;
            return SpatialAPI.GetFeatureFlags()
                .Then(resp => {
                    if (_requestID != currentReqID)
                        return; // Stale; ignore request

                    currentFlags = resp.featureFlags;
                    SessionState.SetString(SESSION_FEATURE_FLAGS_KEY, JsonUtility.ToJson(currentFlags));
                    onFetchComplete?.Invoke();
                })
                .Catch(exc => {
                    if (Application.internetReachability != NetworkReachability.NotReachable)
                        Debug.LogError($"Failed to fetch feature flags: {exc}");
                });
        }
    }
}
