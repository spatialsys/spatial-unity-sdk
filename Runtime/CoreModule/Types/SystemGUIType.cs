using System;

namespace SpatialSys.UnitySDK
{
    [Flags]
    [Obsolete("Use SpatialCoreGUITypeFlags instead")]
    [DocumentationCategory("Services/Core GUI Service")]
    public enum SpatialSystemGUIType
    {
        None = 0,
        QuestSystem = 1 << 0,
    }
}
