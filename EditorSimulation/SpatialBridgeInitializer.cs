using UnityEngine;

namespace SpatialSys.UnitySDK.EditorSimulation
{
    public static class SpatialBridgeInitializer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            SpatialBridge.actorService = new EditorActorService();
            SpatialBridge.cameraService = new EditorCameraService();
            SpatialBridge.coreGUIService = new EditorCoreGUIService();
            SpatialBridge.marketplaceService = new EditorMarketplaceService();
            SpatialBridge.networkingService = new EditorNetworkingService();
        }
    }
}
