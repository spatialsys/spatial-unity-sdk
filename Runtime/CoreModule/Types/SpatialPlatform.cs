using System;

namespace SpatialSys.UnitySDK
{
    // Note: Keep both enum names and values in sync until the old enum is removed
    public enum SpatialPlatform
    {
        Unknown = 0,
        Web = 1,
        Mobile = 2,
        MetaQuest = 3,
    }
}

namespace SpatialSys.UnitySDK.VisualScripting
{
    // In order to avoid breaking changes, we are keeping the old enum name and values
    // This should be removed in the future
    public enum SpatialPlatform
    {
        Unknown = 0,
        Web = 1,
        Mobile = 2,
        MetaQuest = 3,
    }
}
