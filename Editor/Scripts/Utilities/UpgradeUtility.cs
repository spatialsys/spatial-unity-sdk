using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;
using RSG;

namespace SpatialSys.UnitySDK.Editor
{
    [InitializeOnLoad]
    public static class UpgradeUtility
    {
        private const string UNITY_SDK_PACKAGE_NAME = "io.spatial.unitysdk";
        private const string LAST_FETCH_DATE_PREFS_KEY = "SpatialSDK_UpgradeUtility_LastFetchDate";
        private const int FETCH_INTERVAL_MINUTES = 120;
        private const string LAST_AUTO_UPDATE_DATE_PREFS_KEY = "SpatialSDK_UpgradeUtility_LastAutoUpdateDate";
        private const int AUTO_UPDATE_INTERVAL_MINUTES = 120;

        public static UnityEditor.PackageManager.PackageInfo packageInfo => UnityEditor.PackageManager.PackageInfo.FindForAssetPath($"Packages/{UNITY_SDK_PACKAGE_NAME}");
        public static bool isOfficialVersion => packageInfo.source == PackageSource.Registry;
        public static string currentVersion => packageInfo?.version;
        public static string latestVersion => packageInfo?.versions.latest;
        public static bool upgradeAvailable => currentVersion != latestVersion;

        // Check for upgrade
        private static SearchRequest _searchRequest;
        private static Promise<bool> _upgradeCheckPromise;
        private static UnityEditor.PackageManager.PackageInfo _unitySdkPackageInfo;
        public enum UpgradeCheckType
        {
            Default,    // Only do a full fetch if the last fetch was more than FETCH_INTERVAL_MINUTES ago
            ForceFetch, // Always fetch latest package info from the server
            SoftCheck   // Check without fetching new data
        }

        // Upgrade
        private static AddRequest _upgradeRequest;
        private static Promise<bool> _upgradePromise;

        static UpgradeUtility()
        {
#if !SPATIAL_UNITYSDK_DISABLE_UPGRADE_CHECK
            // Do we need to do a suggest update check?
            bool doSuggestUpdateCheck = true;
            string lastCheckDateTicks = EditorPrefs.GetString(LAST_AUTO_UPDATE_DATE_PREFS_KEY, null);
            if (long.TryParse(lastCheckDateTicks, out long lastCheckDateTicksLong))
            {
                System.DateTime lastCheckDate = new System.DateTime(lastCheckDateTicksLong);
                if ((System.DateTime.Now - lastCheckDate).TotalMinutes < AUTO_UPDATE_INTERVAL_MINUTES)
                    doSuggestUpdateCheck = false;
            }

            // Only perform upgrade check if we opened the project just now, or if the last check was more than AUTO_UPDATE_INTERVAL_MINUTES ago
            bool editorWasJustOpened = Time.realtimeSinceStartup < 20;
            if (!EditorApplication.isPlayingOrWillChangePlaymode && (editorWasJustOpened || doSuggestUpdateCheck))
            {
                CheckForUpgrade(UpgradeCheckType.ForceFetch)
                    .Then(upgradeRequired => {
                        EditorPrefs.SetString(LAST_AUTO_UPDATE_DATE_PREFS_KEY, System.DateTime.Now.Ticks.ToString());
                        if (upgradeRequired)
                            ShowUpgradeDialog();
                    });
            }
#endif
        }

        /// <summary>
        /// Check if Spatial UnitySDK package has an upgrade available
        /// Resolves to true if an upgrade is available
        /// </summary>
        public static IPromise<bool> CheckForUpgrade(UpgradeCheckType checkType = UpgradeCheckType.Default)
        {
#if SPATIAL_UNITYSDK_DISABLE_UPGRADE_CHECK
            return Promise<bool>.Resolved(false);
#else
            bool fetchPackageInfo = true;
            switch (checkType)
            {
                case UpgradeCheckType.Default:
                    // Only fetch package info if it's been a while since the last fetch
                    string lastCheckDateTicks = EditorPrefs.GetString(LAST_FETCH_DATE_PREFS_KEY, null);
                    if (long.TryParse(lastCheckDateTicks, out long lastCheckDateTicksLong))
                    {
                        System.DateTime lastCheckDate = new System.DateTime(lastCheckDateTicksLong);
                        if ((System.DateTime.Now - lastCheckDate).TotalMinutes < FETCH_INTERVAL_MINUTES)
                            fetchPackageInfo = false;
                    }
                    break;

                case UpgradeCheckType.ForceFetch:
                    fetchPackageInfo = true;
                    break;

                case UpgradeCheckType.SoftCheck:
                    fetchPackageInfo = false;
                    break;
            }

            // Fast check from cache
            if (!fetchPackageInfo)
                return Promise<bool>.Resolved(upgradeAvailable);

            // We're already doing a check, wait for it to complete
            if (_upgradeCheckPromise != null && _upgradeCheckPromise.CurState == PromiseState.Pending)
                return _upgradeCheckPromise;

            // Check for an upgrade, but fetch latest data
            _upgradeCheckPromise = new Promise<bool>();
            _upgradeCheckPromise.Then(upgradeRequired => EditorPrefs.SetString(LAST_FETCH_DATE_PREFS_KEY, System.DateTime.Now.Ticks.ToString()));
            _searchRequest = Client.Search(UNITY_SDK_PACKAGE_NAME);
            EditorApplication.update += SearchRequestProgressUpdate;
            return _upgradeCheckPromise;
#endif
        }

        private static void SearchRequestProgressUpdate()
        {
            if (_searchRequest.IsCompleted)
            {
                try
                {
                    if (_searchRequest.Status == StatusCode.Success)
                    {
                        // Find spatial sdk package
                        _unitySdkPackageInfo = _searchRequest.Result.Length > 0 ? _searchRequest.Result[0] : null;
                        if (_unitySdkPackageInfo != null)
                        {
                            _upgradeCheckPromise.Resolve(currentVersion != _unitySdkPackageInfo.versions.latest);
                        }
                        else
                        {
                            _upgradeCheckPromise.Reject(new System.Exception($"{nameof(UpgradeUtility)}: Spatial SDK package not found"));
                        }
                    }
                    else
                    {
                        _upgradeCheckPromise.Reject(new System.Exception($"{nameof(UpgradeUtility)}: Error checking for upgrade: " + _searchRequest.Error.message));
                    }
                }
                catch (System.Exception exc)
                {
                    _upgradeCheckPromise.Reject(exc);
                }

                EditorApplication.update -= SearchRequestProgressUpdate;
            }
        }

        public static IPromise<bool> UpgradeToLatest()
        {
#if SPATIAL_UNITYSDK_DISABLE_UPGRADE_CHECK
            return Promise<bool>.Resolved(false);
#else
            // We're already doing an upgrade, wait for it to complete
            if (_upgradePromise != null && _upgradePromise.CurState == PromiseState.Pending)
                return _upgradePromise;

            _upgradePromise = new Promise<bool>();
            CheckForUpgrade(UpgradeCheckType.ForceFetch)
                .Then(upgradeRequired => {
                    if (upgradeRequired)
                    {
                        // Upgrade to latest version
                        _upgradeRequest = Client.Add($"{UNITY_SDK_PACKAGE_NAME}@{_unitySdkPackageInfo.versions.latest}");
                        EditorApplication.update += AddRequestProgressUpdate;
                    }
                    else
                    {
                        // No upgrade required
                        _upgradePromise.Resolve(false);
                    }
                })
                .Catch(err => _upgradePromise.Reject(err));
            return _upgradePromise;
#endif
        }

        private static void AddRequestProgressUpdate()
        {
            if (_upgradeRequest.IsCompleted)
            {
                try
                {
                    if (_upgradeRequest.Status == StatusCode.Success)
                    {
                        _upgradePromise.Resolve(true);
                    }
                    else
                    {
                        _upgradePromise.Reject(new System.Exception($"{nameof(UpgradeUtility)}: Error upgrading package to latest: " + _upgradeRequest.Error.message));
                    }
                }
                catch (System.Exception exc)
                {
                    _upgradePromise.Reject(exc);
                }

                EditorApplication.update -= AddRequestProgressUpdate;
            }
        }

        public static void ShowUpgradeDialog()
        {
            if (UnityEditor.EditorUtility.DisplayDialog("Upgrade to latest version?", "A new version of the Spatial SDK is available. Would you like to upgrade?", "Yes", "No"))
            {
                UpgradeToLatest()
                    .Then(upgradePerformed => {
                        UnityEditor.EditorUtility.DisplayDialog("Upgrade successful", "The Spatial SDK has been upgraded to the latest version.", "OK");
                    })
                    .Catch(err => {
                        Debug.LogException(err);
                        UnityEditor.EditorUtility.DisplayDialog("Upgrade failed", "Failed to upgrade to latest version of the Spatial SDK. Please try again later.", "OK");
                    });
            }
        }
    }
}