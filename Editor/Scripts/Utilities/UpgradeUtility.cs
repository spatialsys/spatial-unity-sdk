using System.Linq;
using UnityEngine;
using UnityEditor;
using RSG;

namespace SpatialSys.UnitySDK.Editor
{
    [InitializeOnLoad]
    public static class UpgradeUtility
    {
        private const string LAST_FETCH_DATE_PREFS_KEY = "SpatialSDK_UpgradeUtility_LastFetchDate";
        private const int FETCH_INTERVAL_MINUTES = 120;
        private const string LAST_AUTO_UPDATE_DATE_PREFS_KEY = "SpatialSDK_UpgradeUtility_LastAutoUpdateDate";
        private const int AUTO_UPDATE_INTERVAL_MINUTES = 120;

        public enum UpgradeCheckType
        {
            Default,    // Only do a full fetch if the last fetch was more than FETCH_INTERVAL_MINUTES ago
            ForceFetch, // Always fetch latest package info from the server
            SoftCheck   // Check without fetching new data
        }

        static UpgradeUtility()
        {
#if !SPATIAL_UNITYSDK_DISABLE_UPGRADE_CHECK && !SPATIAL_UNITYSDK_INTERNAL
            // Check if it has been enough time since we asked user to update
            bool doSuggestUpdateCheck = !EditorUtility.TryGetDateTimeFromEditorPrefs(LAST_AUTO_UPDATE_DATE_PREFS_KEY, out System.DateTime lastCheckDate) ||
                (System.DateTime.Now - lastCheckDate).TotalMinutes >= AUTO_UPDATE_INTERVAL_MINUTES;

            // But also perform an update check anyway if the user just opened the editor.
            bool editorWasJustOpened = Time.realtimeSinceStartup < 20;
            if (!EditorApplication.isPlayingOrWillChangePlaymode && (editorWasJustOpened || doSuggestUpdateCheck))
            {
                CheckForUpgrade(UpgradeCheckType.ForceFetch)
                    .Then(upgradeRequired => {
                        EditorUtility.SetDateTimeToEditorPrefs(LAST_AUTO_UPDATE_DATE_PREFS_KEY, System.DateTime.Now);
                        if (upgradeRequired)
                            ShowUpgradeDialog();
                    });
            }
#endif
        }

        /// <summary>
        /// Returns a promise that resolves to true if a package update is available, otherwise false.
        /// The promise is rejected if there's a request error or other internal exception.
        /// </summary>
        public static IPromise<bool> CheckForUpgrade(UpgradeCheckType checkType = UpgradeCheckType.Default)
        {
#if SPATIAL_UNITYSDK_DISABLE_UPGRADE_CHECK || SPATIAL_UNITYSDK_INTERNAL
            return Promise<bool>.Resolved(false);
#else
            bool performNetworkRequest = true;
            switch (checkType)
            {
                case UpgradeCheckType.Default:
                    // Only fetch package info if it's been a while since the last fetch
                    performNetworkRequest = !EditorUtility.TryGetDateTimeFromEditorPrefs(LAST_FETCH_DATE_PREFS_KEY, out System.DateTime lastCheckDate) ||
                        (System.DateTime.Now - lastCheckDate).TotalMinutes >= FETCH_INTERVAL_MINUTES;
                    break;

                case UpgradeCheckType.ForceFetch:
                    performNetworkRequest = true;
                    break;

                case UpgradeCheckType.SoftCheck:
                    performNetworkRequest = false;
                    break;
            }

            // Fast check from cache
            if (!performNetworkRequest)
                return Promise<bool>.Resolved(PackageManagerUtility.updateAvailable);

            return PackageManagerUtility.CheckForUpdate();
#endif
        }

        public static void ShowUpgradeDialog()
        {
#if !SPATIAL_UNITYSDK_DISABLE_UPGRADE_CHECK && !SPATIAL_UNITYSDK_INTERNAL
            if (UnityEditor.EditorUtility.DisplayDialog("Upgrade to latest version?", "A new version of the Spatial SDK is available. Would you like to upgrade now?", "Yes", "No"))
            {
                PackageManagerUtility.UpdateToLatest()
                    .Then(updatePerformed => {
                        if (updatePerformed)
                            UnityEditor.EditorUtility.DisplayDialog("Upgrade successful", "The Spatial SDK has been upgraded to the latest version.", "OK");
                    })
                    .Catch(err => {
                        Debug.LogException(err);
                        UnityEditor.EditorUtility.DisplayDialog("Upgrade failed", "Failed to upgrade to latest version of the Spatial SDK. Please try again later.", "OK");
                    });
            }
#endif
        }

        /// <summary>
        /// UpgradeToLatest, but specific messaging for when the upgrade is required for test or publish
        /// </summary>
        public static IPromise PerformUpgradeIfNecessaryForTestOrPublish()
        {
#if SPATIAL_UNITYSDK_DISABLE_UPGRADE_CHECK || SPATIAL_UNITYSDK_INTERNAL
            return Promise.Resolved();
#else
            return CheckForUpgrade()
                .Then(updateAvailable => {
                    if (updateAvailable)
                    {
                        if (UnityEditor.EditorUtility.DisplayDialog(
                            "Upgrade to latest version?",
                            "A new version of the Spatial SDK is available. You will need to upgrade to the latest version to publish or test your package. Would you like to upgrade now?",
                            "Yes",
                            "No"))
                        {
                            return PackageManagerUtility.UpdateToLatest();
                        }
                        else
                        {
                            UnityEditor.EditorUtility.DisplayDialog("Upgrade required to continue", "You will need to upgrade to the latest Spatial SDK to continue. You can do this by pressing the menu item under \"Spatial SDK/Check for updates...\".", "OK");
                            throw new RSG.PromiseCancelledException();
                        }
                    }

                    return Promise<bool>.Resolved(false);
                })
                .Then(updatePerformed => {
                    if (updatePerformed)
                        UnityEditor.EditorUtility.DisplayDialog("Upgrade successful", "The Spatial SDK has been upgraded to the latest version.", "OK");
                })
                .Catch(err => {
                    if (err is RSG.PromiseCancelledException)
                        throw err; // Cancel downstream promises

                    Debug.LogException(err);
                    UnityEditor.EditorUtility.DisplayDialog("Upgrade failed", "Failed to upgrade to latest version of the Spatial SDK. Check out the console panel for details.", "OK");
                    throw new System.Exception("Failed to perform necessary upgrade");
                });
#endif
        }
    }
}