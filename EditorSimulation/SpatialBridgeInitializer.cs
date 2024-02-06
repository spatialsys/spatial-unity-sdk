using UnityEngine;

namespace SpatialSys.UnitySDK.EditorSimulation
{
    // Helper class used to run coroutines in the editor. By placing this class on a shared file, Unity
    // doesn't detect this file as an 'Editor' file and lets us attach it to a GameObject.
    public class CoroutineRunner : MonoBehaviour
    {
    }
    public static class SpatialBridgeInitializer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
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
            SpatialBridge.spaceService = new EditorSpaceService();
            SpatialBridge.spatialComponentService = new EditorSpatialComponentService();
            SpatialBridge.userWorldDataStoreService = new EditorUserWorldDataStoreService();
            SpatialBridge.vfxService = new EditorVFXService();
        }
    }
}
