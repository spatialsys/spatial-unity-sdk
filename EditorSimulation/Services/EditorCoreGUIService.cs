
namespace SpatialSys.UnitySDK.EditorSimulation
{
    #pragma warning disable 67 // Disable unused event warning

    public class EditorCoreGUIService : ICoreGUIService
    {
        public ICoreGUIShopService shop { get; } = new EditorCoreGUIShopService();

        public event ICoreGUIService.OnCoreGUIOpenStateDelegate onCoreGUIOpenStateChanged;
        public event ICoreGUIService.OnCoreGUIEnabledStateDelegate onCoreGUIEnabledStateChanged;

        public void SetCoreGUIOpen(SpatialCoreGUITypeFlags guis, bool open) { }
        public void SetCoreGUIEnabled(SpatialCoreGUITypeFlags guis, bool enabled) { }
        public SpatialCoreGUIState GetCoreGUIState(SpatialCoreGUIType guiType) => SpatialCoreGUIState.None;
        public void CloseAllCoreGUI() { }
        public void DisplayToastMessage(string message, float duration = 4f) { }
    }
    
    public class EditorCoreGUIShopService : ICoreGUIShopService
    {
        public bool isGUIOpen { get; }
        public bool isGUIEnabled { get; }

        public void SelectItem(string itemID) { }
        public void SetItemEnabled(string itemID, bool enabled, string disabledMessage = null) { }
        public void SetItemVisibility(string itemID, bool visible) { }
    }
}
