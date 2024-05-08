using UnityEngine;
using SpatialSys.UnitySDK.VisualScripting;

namespace SpatialSys.UnitySDK.EditorSimulation
{
    public static class SpatialBridgeInitializer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
#if !SPATIAL_UNITYSDK_INTERNAL
            SpatialBridge.actorService = new EditorActorService();
            SpatialBridge.adService = new EditorAdService();
            SpatialBridge.audioService = new EditorAudioService();
            SpatialBridge.badgeService = new EditorBadgeService();
            SpatialBridge.cameraService = new EditorCameraService();
            SpatialBridge.coreGUIService = new EditorCoreGUIService();
            SpatialBridge.inputService = new EditorInputService();
            SpatialBridge.inventoryService = new EditorInventoryService();
            SpatialBridge.loggingService = new EditorLoggingService();
            SpatialBridge.marketplaceService = new EditorMarketplaceService();
            SpatialBridge.networkingService = new EditorNetworkingService();
            SpatialBridge.questService = new EditorQuestService();
            SpatialBridge.spaceContentService = new EditorSpaceContentService();
            SpatialBridge.spaceService = new EditorSpaceService();
            SpatialBridge.spatialComponentService = new EditorSpatialComponentService();
            SpatialBridge.userWorldDataStoreService = new EditorUserWorldDataStoreService();
            SpatialBridge.vfxService = new EditorVFXService();
            SpatialBridge.graphicsService = new EditorGraphicsService();
            SpatialBridge.eventService = new EditorEventService();


            new GameObject("[Spatial SDK] Visual Scripting Manager").AddComponent<UnitySDKVisualScriptingManager>();
#endif
        }
    }
}
