using System;

namespace SpatialSys.UnitySDK
{
    [Flags]
    [Obsolete("Use SpatialCoreGUITypeFlags instead")]
    public enum SpatialSystemGUIType
    {
        None = 0,
        QuestSystem = 1 << 0,
    }
}
