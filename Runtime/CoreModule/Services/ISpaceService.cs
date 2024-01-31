
namespace SpatialSys.UnitySDK
{
    /// <summary>
    /// Service for interacting with the current space.
    /// </summary>
    public interface ISpaceService
    {
        /// <summary>
        /// Returns the version of the space package that is currently running. Only makes sense when the current space
        /// is a unity-based space.
        /// Does not currently work properly with SpaceTemplate based spaces.
        /// </summary>
        int spacePackageVersion { get; }

        /// <summary>
        /// Are we currently in a sandbox space?
        /// </summary>
        bool isSandbox { get; }

        /// <summary>
        /// Has the local user loved this space?
        /// </summary>
        bool hasLikedSpace { get; }

        /// <summary>
        /// Triggered when the user likes the space.
        /// </summary>
        event OnSpaceLikedDelegate onSpaceLiked;
        public delegate void OnSpaceLikedDelegate();

        /// <summary>
        /// Open a URL in the user's default browser.
        /// On web, this will open the URL in a new tab.
        /// On mobile, this will open the URL in the default browser app.
        /// In VR, this will open the URL in the default browser app (popover on Oculus Quest)
        /// </summary>
        void OpenURL(string url);

        /// <summary>
        /// Enable or disable avatar to avatar collisions
        /// </summary>
        void EnableAvatarToAvatarCollisions(bool enabled);
    }
}
