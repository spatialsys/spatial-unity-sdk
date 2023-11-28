using SpatialSys.UnitySDK.Internal;

namespace SpatialSys.UnitySDK
{
    // Preserved for backwards compatibility; This is what the "Spatial Bridge" was previously called but we need
    // to keep the name for now since the type is serialized by name in Visual Scripting graphs
    [InternalType]
    public static class ClientBridge
    {
        [InternalType]
        public enum DataStoreScope
        {
            UserWorldData = 0,
        }
    }
}