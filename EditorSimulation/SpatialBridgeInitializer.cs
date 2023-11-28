using UnityEngine;

namespace SpatialSys.UnitySDK.EditorSimulation
{
    public static class SpatialBridgeInitializer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            SpatialBridge.cameraService = new EditorCameraService();
            SpatialBridge.marketplaceService = new EditorMarketplaceService();
        }
    }
}
