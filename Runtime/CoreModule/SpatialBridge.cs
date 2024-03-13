using UnityEngine;

namespace SpatialSys.UnitySDK
{
    /// <summary>
    /// The main interface that provides access to Spatial services.
    /// This acts like a bridge between user code and Spatial core functionality.
    /// <code source="Services/SpatialBridgeExamples.cs" region="BridgeHelloWorld"/>
    /// </summary>
    [DocumentationCategory("Spatial Bridge")]
    public static class SpatialBridge
    {
        /// <summary>
        /// Service for interacting with actors and users in the space.
        /// </summary>
        public static IActorService actorService;

        /// <summary>
        /// Service to handle ads integration
        /// </summary>
        public static IAdService adService;

        /// <summary>
        /// Service for handling audio and sound effects.
        /// </summary>
        public static IAudioService audioService;

        /// <summary>
        /// Service for handling badges.
        /// </summary>
        public static IBadgeService badgeService;

        /// <summary>
        /// Provides access to all camera related functionality: Main camera state, player camera settings,
        /// camera shake, and target overrides.
        /// </summary>
        public static ICameraService cameraService;

        /// <summary>
        /// Service for handling all UI related functionality.
        /// </summary>
        public static ICoreGUIService coreGUIService;

        /// <summary>
        /// A service with helper methods for subscribing to events.
        /// </summary>
        public static IEventService eventService;

        /// <summary>
        /// Service for handling all input related functionality.
        /// </summary>
        public static IInputService inputService;

        /// <summary>
        /// Service to handle inventory and currency.
        /// </summary>
        public static IInventoryService inventoryService;

        /// <summary>
        /// Service for logging errors and messages to the console.
        /// </summary>
        public static ILoggingService loggingService;

        /// <summary>
        /// Service to handle item purchases on the store.
        /// </summary>
        public static IMarketplaceService marketplaceService;

        /// <summary>
        /// This service provides access to all the networking functionality in Spatial: connectivity, server management,
        /// matchmaking, remove events (RPCs/Messaging), etc.
        /// </summary>
        public static INetworkingService networkingService;

        /// <summary>
        /// Service for managing Quests on the space.
        /// </summary>
        public static IQuestService questService;

        /// <summary>
        /// Service for interacting with the current scene's synced objects
        /// </summary>
        public static ISpaceContentService spaceContentService;

        /// <summary>
        /// Service for interacting with the current space.
        /// </summary>
        public static ISpaceService spaceService;

        /// <summary>
        /// This service provides access to the <c>Users</c> datastore for the current <c>world</c>. Spaces that belong to
        /// the same <c>world</c> share the same user world datastore.
        /// </summary>
        public static IUserWorldDataStoreService userWorldDataStoreService;

        /// <summary>
        /// A service for handling visual effects.
        /// </summary>
        public static IVFXService vfxService;

        /// <summary>
        /// A service for handling Graphics settings.
        /// </summary>
        public static IGraphicsService graphicsService;

        #region Internal services

        /// <summary>
        /// Used to initialize Spatial components in the Unity SDK. This is an internal type and should not be used by developers,
        /// only on spatial components themselves.
        /// </summary>
        public static Internal.ISpatialComponentService spatialComponentService;

        #endregion
    }
}
