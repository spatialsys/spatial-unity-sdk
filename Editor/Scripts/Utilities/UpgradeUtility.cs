using System.Linq;
using System.Collections;
using System.Collections.Generic;
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
        private const int FETCH_INTERVAL_HOURS = 2;

        public static string currentVersion => UnityEditor.PackageManager.PackageInfo.GetAllRegisteredPackages().FirstOrDefault(p => p.name == UNITY_SDK_PACKAGE_NAME)?.version;

        // Check for upgrade
        private static ListRequest _listRequest;
        private static Promise<bool> _upgradeCheckPromise;
        private static UnityEditor.PackageManager.PackageInfo _unitySdkPackageInfo;
        public enum UpgradeCheckType
        {
            Default,
            ForceFetch, // Always fetch latest package info from the server
            SoftCheck   // Check without fetching new data
        }

        // Upgrade
        private static AddRequest _upgradeRequest;
        private static Promise<bool> _upgradePromise;

        static UpgradeUtility()
        {
#if !SPATIAL_UNITYSDK_DISABLE_UPGRADE_CHECK
            UpgradeUtility.CheckForUpgrade(UpgradeUtility.UpgradeCheckType.ForceFetch)
                .Then(upgradeRequired => {
                    if (upgradeRequired)
                        ShowUpgradeDialog();
                });
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
                    fetchPackageInfo = true;

                    // Only fetch package info if it's been a while since the last fetch
                    string lastCheckDateTicks = EditorPrefs.GetString(LAST_FETCH_DATE_PREFS_KEY, null);
                    if (lastCheckDateTicks != null)
                    {
                        long lastCheckDateTicksLong = long.Parse(lastCheckDateTicks);
                        System.DateTime lastCheckDate = new System.DateTime(lastCheckDateTicksLong);
                        if ((System.DateTime.Now - lastCheckDate).TotalHours > FETCH_INTERVAL_HOURS)
                            fetchPackageInfo = true;
                    }
                    break;

                case UpgradeCheckType.ForceFetch:
                    fetchPackageInfo = true;
                    break;

                case UpgradeCheckType.SoftCheck:
                    fetchPackageInfo = false;
                    break;
            }

            // Check for an upgrade.
            _upgradeCheckPromise = new Promise<bool>();
            _listRequest = Client.List(offlineMode: !fetchPackageInfo);
            EditorApplication.update += ListRequestProgressUpdate;
            if (fetchPackageInfo)
                _upgradeCheckPromise.Then(upgradeRequired => EditorPrefs.SetString(LAST_FETCH_DATE_PREFS_KEY, System.DateTime.Now.Ticks.ToString()));

            return _upgradeCheckPromise;
#endif
        }

        private static void ListRequestProgressUpdate()
        {
            if (_listRequest.IsCompleted)
            {
                if (_listRequest.Status == StatusCode.Success)
                {
                    // Find spatial sdk package
                    _unitySdkPackageInfo = _listRequest.Result.FirstOrDefault(p => p.name == UNITY_SDK_PACKAGE_NAME);
                    if (_unitySdkPackageInfo != null)
                    {
                        _upgradeCheckPromise.Resolve(_unitySdkPackageInfo.version != _unitySdkPackageInfo.versions.latest);
                    }
                    else
                    {
                        _upgradeCheckPromise.Reject(new System.Exception($"{nameof(UpgradeUtility)}: Spatial SDK package not found"));
                    }
                }
                else
                {
                    _upgradeCheckPromise.Reject(new System.Exception($"{nameof(UpgradeUtility)}: Error checking for upgrade: " + _listRequest.Error.message));
                }

                EditorApplication.update -= ListRequestProgressUpdate;
            }
        }

        public static IPromise<bool> UpgradeToLatest()
        {
#if SPATIAL_UNITYSDK_DISABLE_UPGRADE_CHECK
            return Promise<bool>.Resolved(false);
#else
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
                if (_upgradeRequest.Status == StatusCode.Success)
                {
                    _upgradePromise.Resolve(true);
                }
                else
                {
                    _upgradePromise.Reject(new System.Exception($"{nameof(UpgradeUtility)}: Error upgrading package to latest: " + _upgradeRequest.Error.message));
                }

                EditorApplication.update -= AddRequestProgressUpdate;
            }
        }

        public static void ShowUpgradeDialog()
        {
            if (UnityEditor.EditorUtility.DisplayDialog("Upgrade to latest version?", "A new version of the Spatial SDK is available. Would you like to upgrade?", "Yes", "No"))
            {
                UpgradeUtility.UpgradeToLatest()
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