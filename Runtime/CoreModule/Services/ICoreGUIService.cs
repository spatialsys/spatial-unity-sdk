using System;

namespace SpatialSys.UnitySDK
{
    /// <summary>
    /// Service for handling all UI related functionality.
    /// </summary>
    [DocumentationCategory("Services/Core GUI Service")]
    public interface ICoreGUIService
    {
        /// <summary>
        /// Interface for shop-specific functionality
        /// </summary>
        ICoreGUIShopService shop { get; }

        /// <summary>
        /// Triggered when a core GUI is opened or closed.
        /// </summary>
        event OnCoreGUIOpenStateDelegate onCoreGUIOpenStateChanged;
        public delegate void OnCoreGUIOpenStateDelegate(SpatialCoreGUIType guiType, bool open);

        /// <summary>
        /// Triggered when a core GUI is enabled or disabled.
        /// </summary>
        event OnCoreGUIEnabledStateDelegate onCoreGUIEnabledStateChanged;
        public delegate void OnCoreGUIEnabledStateDelegate(SpatialCoreGUIType guiType, bool enabled);

        /// <summary>
        /// Opens/Maximize or Close/Minimize a core GUI. When closed, this simply hides the GUI from the user, but
        /// does not disable it, which means the GUI can still be opened by the user via hotkeys.
        /// 
        /// If the GUI is currently disabled and an attempt is made to open the UI, it will not be opened, nor will it be opened if it is eventually enabled.
        /// However, closing the GUI even when it is disabled will still mark it as closed.
        /// </summary>
        /// <example><code source="Services/CoreGUIServiceExamples.cs" region="SetCoreGUIOpen"/></example>
        void SetCoreGUIOpen(SpatialCoreGUITypeFlags guis, bool open);

        /// <summary>
        /// Enables or Disables a core GUI.
        /// If enabled, this allows the user or script to open the GUI. But if it was not open before, it will not be opened.
        /// If disabled, this will not only close or minimize or hide the given GUI, but also prevent it from being
        /// opened by the user until it is re-enabled. When re-enabled, the GUI will be restored to its previous open state,
        /// either minimized or open/maximized.
        /// </summary>
        /// <example><code source="Services/CoreGUIServiceExamples.cs" region="SetCoreGUIEnabled"/></example>
        void SetCoreGUIEnabled(SpatialCoreGUITypeFlags guis, bool enabled);

        /// <summary>
        /// Returns the current state of a core GUI.
        /// </summary>
        SpatialCoreGUIState GetCoreGUIState(SpatialCoreGUIType guiType);

        /// <summary>
        /// Closes or minimizes all core GUIs. This simply hides the GUIs from the user, but does not disable them.
        /// </summary>
        void CloseAllCoreGUI();

        /// <summary>
        /// Enables or Disables mobile controls GUI.
        /// If disabled, this will hide the default controls GUI on mobile devices.
        /// </summary>
        /// <example><code source="Services/CoreGUIServiceExamples.cs" region="SetMobileControlsGUIEnabled"/></example>
        void SetMobileControlsGUIEnabled(SpatialMobileControlsGUITypeFlags guis, bool enabled);

        /// <summary>
        /// Display a toast message to the user. This is a basic text-based notification.
        /// This can be called every 500ms. If called more frequently, the messages will be ignored.
        /// </summary>
        /// <param name="message">The message to display</param>
        /// <param name="duration">The duration the message should display for</param>
        void DisplayToastMessage(string message, float duration = 4f);
    }

    /// <summary>
    /// Interface for world-shop specific functionality.
    /// This only controls the world shop GUI, not the universal shop GUI.
    /// </summary>
    [DocumentationCategory("Services/Core GUI Service")]
    public interface ICoreGUIShopService
    {
        /// <summary>
        /// Is the shop GUI currently open?
        /// </summary>
        bool isGUIOpen { get; }

        /// <summary>
        /// Can the user currently open the shop GUI?
        /// </summary>
        bool isGUIEnabled { get; }

        /// <summary>
        /// Select an item in the Shop GUI. This only works if the shop GUI is open.
        /// </summary>
        void SelectItem(string itemID);

        /// <summary>
        /// Set the enabled state for an item in the shop GUI. This can be useful to prevent users from purchasing items
        /// until they have attained a certain XP or Level.
        /// All items in the shop are enabled by default.
        /// </summary>
        /// <param name="itemID">The ID of the item</param>
        /// <param name="enabled">Enabled or disabled</param>
        /// <param name="disabledMessage">The tooltip or explanation message to show to the user explaining why the item is disabled</param>
        void SetItemEnabled(string itemID, bool enabled, string disabledMessage = null);

        /// <summary>
        /// Set the visibility state of an item in the shop GUI. This can be useful to only show items in the shop GUI that
        /// the user can currently purchase.
        /// </summary>
        void SetItemVisibility(string itemID, bool visible);
    }

    [DocumentationCategory("Services/Core GUI Service")]
    public enum SpatialCoreGUIType
    {
        None = 0,

        /// <summary>
        /// The GUI showing the list of participants in the space.
        /// </summary>
        ParticipantsList = 1,

        /// <summary>
        /// The GUI showing the chat window.
        /// </summary>
        Chat = 2,

        /// <summary>
        /// The GUI showing the backpack (the user's inventory).
        /// </summary>
        Backpack = 3,

        /// <exclude />
        [Obsolete("Use WorldShop instead")]
        Shop = 4,

        /// <summary>
        /// The in-space shop GUI
        /// </summary>
        WorldShop = 4,

        /// <summary>
        /// The GUI for the quest system, showing the current quest and progress
        /// </summary>
        QuestSystem = 5,

        /// <summary>
        /// The GUI for the emote system, showing the list of available emotes.
        /// </summary>
        Emote = 6,

        /// <summary>
        /// The universal shop GUI. This shop is for items that can be used in all spaces on the platform.
        /// </summary>
        UniversalShop = 7,
    }

    [Flags]
    [DocumentationCategory("Services/Core GUI Service")]
    public enum SpatialCoreGUITypeFlags
    {
        None = 0,

        ParticipantsList = 1 << 0,
        Chat = 1 << 1,
        Backpack = 1 << 2,

        [Obsolete("Use WorldShop instead")]
        /// <exclude />
        Shop = 1 << 3,
        WorldShop = 1 << 3,

        QuestSystem = 1 << 4,
        Emote = 1 << 5,
        UniversalShop = 1 << 6,

        All = ~None,
    }

    [Flags]
    [DocumentationCategory("Services/Core GUI Service")]
    public enum SpatialCoreGUIState
    {
        None = 0,

        /// <summary>
        /// Whether the GUI can be opened by the user or by script.
        /// </summary>
        Enabled = 1 << 0,

        /// <summary>
        /// Whether the GUI is currently open or closed. If a GUI is marked as open but also disabled, while the GUI
        /// is disabled it will be temporarily marked as closed. When the GUI is re-enabled, it will be restored to
        /// its previous open state.
        /// </summary>
        Open = 1 << 1,
    }

    [Flags]
    [DocumentationCategory("Services/Core GUI Service")]
    public enum SpatialMobileControlsGUITypeFlags
    {
        None = 0,

        AvatarMoveControls = 1 << 0,
        AvatarJumpButton = 1 << 1,

        All = ~None,
    }

    [DocumentationCategory("Services/Core GUI Service")]
    public static class CoreGUIUtility
    {
        public static SpatialCoreGUITypeFlags ToFlag(this SpatialCoreGUIType guiType)
        {
            return (SpatialCoreGUITypeFlags)(1 << (int)(guiType - 1));
        }
    }
}
