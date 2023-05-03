using System;

namespace SpatialSys.UnitySDK
{
    public static class UnitySDKExtensions
    {
        public static bool PublishedWithTargetVersionOrLater(this SavedProjectSettings settings, Version targetSDKVersion)
        {
            // Requires the published SDK version (introduced in SDK 0.56.0) to be defined in order to properly compare.
            // In this case, always assume that the current SDK version is always older.
            if (settings == null || string.IsNullOrEmpty(settings.publishedSDKVersion))
                return false;

            return new Version(settings.publishedSDKVersion) >= targetSDKVersion;
        }
    }
}
