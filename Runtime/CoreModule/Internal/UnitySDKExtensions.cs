using System;

namespace SpatialSys.UnitySDK.Internal
{
    public static class UnitySDKExtensions
    {
        public static bool PublishedWithTargetVersionOrLater(this SavedProjectSettings settings, int major, int minor, int build = 0)
        {
            return PublishedWithTargetVersionOrLater(settings, new Version(major, minor, build));
        }

        public static bool PublishedWithTargetVersionOrLater(this SavedProjectSettings settings, Version targetSDKVersion)
        {
            if (settings == null)
                return false;

            // Published SDK version was introduced in SDK 0.56.0, so anything older cannot be parsed.
            // By default, assume that the published version is always older.
            return Version.TryParse(settings.publishedSDKVersion, out Version parsedVersion) && parsedVersion >= targetSDKVersion;
        }
    }
}
