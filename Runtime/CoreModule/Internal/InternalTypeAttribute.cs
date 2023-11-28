using System;

namespace SpatialSys.UnitySDK.Internal
{
    /// <summary>
    /// This attribute is used to mark types that are internal to the SDK and should not be used by external code.
    /// It is used to mark types as internal that cannot be moved into the internal namespace because moving them
    /// would break existing code.
    /// </summary>
    public class InternalTypeAttribute : Attribute
    {
    }
}
