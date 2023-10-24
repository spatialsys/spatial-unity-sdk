using UnityEditor;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;
using RSG;

using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace SpatialSys.UnitySDK.Editor
{
    [InitializeOnLoad]
    public static class PackageManagerUtility
    {
        public const string PACKAGE_NAME = "io.spatial.unitysdk";
        public const string PACKAGE_DIRECTORY_PATH = "Packages/" + PACKAGE_NAME;

        public static PackageInfo localPackageInfo => PackageInfo.FindForAssetPath(PACKAGE_DIRECTORY_PATH);
        public static bool isOfficialVersion => localPackageInfo.source == PackageSource.Registry;
        public static string currentVersion => localPackageInfo?.version;
        public static string latestVersion => localPackageInfo?.versions.latest;
        public static bool updateAvailable => currentVersion != latestVersion;
        public static string documentationUrl => localPackageInfo?.documentationUrl;

        private static SearchRequest _searchRequest;
        private static Promise<bool> _updateCheckPromise;
        private static PackageInfo _fetchedPackageInfo;
        private static AddRequest _updateRequest;
        private static Promise<bool> _updatePromise;

        /// <summary>
        /// Returns a promise that resolves to true if a package update is available, otherwise false.
        /// The promise is rejected if there's a request error or other internal exception.
        /// </summary>
        public static IPromise<bool> CheckForUpdate()
        {
            // We're already doing a check, wait for it to complete
            if (_updateCheckPromise?.CurState == PromiseState.Pending)
                return _updateCheckPromise;

            _updateCheckPromise = new Promise<bool>();
            _searchRequest = Client.Search(PACKAGE_NAME);
            EditorApplication.update += HandleSearchRequestCompletion;
            return _updateCheckPromise;
        }

        private static void HandleSearchRequestCompletion()
        {
            // Wait until completed.
            if (!_searchRequest.IsCompleted)
                return;

            try
            {
                if (_searchRequest.Status == StatusCode.Success)
                {
                    _fetchedPackageInfo = _searchRequest.Result.Length > 0 ? _searchRequest.Result[0] : null;
                    if (_fetchedPackageInfo != null)
                    {
                        _updateCheckPromise.Resolve(currentVersion != _fetchedPackageInfo.versions.latest);
                    }
                    else
                    {
                        _updateCheckPromise.Reject(new System.Exception($"{nameof(PackageManagerUtility)}: {PACKAGE_NAME} not found"));
                    }
                }
                else
                {
                    _updateCheckPromise.Reject(new System.Exception($"{nameof(PackageManagerUtility)}: Error checking for updates. " + _searchRequest.Error.message));
                }
            }
            catch (System.Exception exc)
            {
                _updateCheckPromise.Reject(exc);
            }

            EditorApplication.update -= HandleSearchRequestCompletion;
        }

        /// <summary>
        /// Returns a promise that resolves to true if the package updated successfully, otherwise false if it's already on the latest version.
        /// The promise is rejected if there's a request error or other internal exception.
        /// </summary>
        public static IPromise<bool> UpdateToLatest()
        {
            // We're already doing an upgrade, wait for it to complete
            if (_updatePromise?.CurState == PromiseState.Pending)
                return _updatePromise;

            _updatePromise = new Promise<bool>();
            CheckForUpdate()
                .Then(updateAvailable => {
                    if (updateAvailable)
                    {
                        // Upgrade to latest version
                        _updateRequest = Client.Add($"{PACKAGE_NAME}@{_fetchedPackageInfo.versions.latest}");
                        EditorApplication.update += HandleUpdateRequestCompletion;
                    }
                    else
                    {
                        _updatePromise.Resolve(false);
                    }
                })
                .Catch(exc => _updatePromise.Reject(exc));
            return _updatePromise;
        }

        private static void HandleUpdateRequestCompletion()
        {
            if (!_updateRequest.IsCompleted)
                return;

            try
            {
                if (_updateRequest.Status == StatusCode.Success)
                {
                    _updatePromise.Resolve(true);
                }
                else
                {
                    _updatePromise.Reject(new System.Exception($"{nameof(PackageManagerUtility)}: Error updating package to latest. " + _updateRequest.Error.message));
                }
            }
            catch (System.Exception exc)
            {
                _updatePromise.Reject(exc);
            }

            EditorApplication.update -= HandleUpdateRequestCompletion;
        }
    }
}
