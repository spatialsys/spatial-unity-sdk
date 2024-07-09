
namespace SpatialSys.UnitySDK
{
    /// <summary>
    /// Service for interacting with the current space.
    /// </summary>
    /// <example><code source="Services/SpaceServiceExamples.cs" region="SandboxLog" lang="csharp"/></example>
    [DocumentationCategory("Services/Space Service")]
    public interface ISpaceService
    {
        /// <summary>
        /// Returns the package version of the Space package that is loaded. Otherwise, returns 0 if
        /// the version failed to parse or fetch.
        /// </summary>
        int spacePackageVersion { get; }

        /// <summary>
        /// Are we currently in a sandbox space?
        /// </summary>
        /// <example><code source="Services/SpaceServiceExamples.cs" region="SandboxLog" lang="csharp"/></example>
        bool isSandbox { get; }

        /// <summary>
        /// Has the local user loved this space?
        /// </summary>
        /// <example><code source="Services/SpaceServiceExamples.cs" region="SpaceLiked" lang="csharp"/></example>
        bool hasLikedSpace { get; }

        /// <summary>
        /// Triggered when the user likes the space.
        /// </summary>
        /// <example><code source="Services/SpaceServiceExamples.cs" region="SpaceLiked" lang="csharp"/></example>
        event OnSpaceLikedDelegate onSpaceLiked;
        public delegate void OnSpaceLikedDelegate();

        /// <summary>
        /// Open a URL in the user's default browser.
        /// On web, this will open the URL in a new tab.
        /// On mobile, this will open the URL in the default browser app.
        /// In VR, this will open the URL in the default browser app (popover on Oculus Quest)
        /// </summary>
        /// <example><code source="Services/SpaceServiceExamples.cs" region="OpenURL" lang="csharp"/></example>
        void OpenURL(string url);

        /// <summary>
        /// Enable or disable avatar to avatar collisions
        /// </summary>
        void EnableAvatarToAvatarCollisions(bool enabled);
    }
}
