using UnityEngine;

namespace SpatialSys.UnitySDK
{
    // Sync any changes here with the corresponding enum in the Spatial API
    public enum PackageType
    {
        Space = 4, // Placed at the top so that it's the default value in editor windows
        SpaceTemplate = 0,
        Avatar = 1,
        AvatarAnimation = 2,
        PrefabObject = 3,
        [InspectorName(null)] // Not ready for public use yet
        AvatarAttachment = 5,
    }
}