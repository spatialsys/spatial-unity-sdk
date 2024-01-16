using UnityEngine;

namespace SpatialSys.UnitySDK.EditorSimulation
{
    public static class SpatialBridgeInitializer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            SpatialBridge.actorService = new EditorActorService();
            SpatialBridge.audioService = new EditorAudioService();
            SpatialBridge.badgeService = new EditorBadgeService();
            SpatialBridge.cameraService = new EditorCameraService();
            SpatialBridge.coreGUIService = new EditorCoreGUIService();
            SpatialBridge.loggingService = new EditorLoggingService();
            SpatialBridge.marketplaceService = new EditorMarketplaceService();
            SpatialBridge.networkingService = new EditorNetworkingService();
            SpatialBridge.spatialComponentService = new EditorSpatialComponentService();
            SpatialBridge.userWorldDataStoreService = new EditorUserWorldDataStoreService();
            SpatialBridge.vfxService = new EditorVFXService();
        }
    }
}
