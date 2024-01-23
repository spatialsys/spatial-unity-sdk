using System.Collections.Generic;
using SpatialSys.UnitySDK;

namespace SpatialSys.UnitySDK.EditorSimulation
{
    #pragma warning disable 67 // Disable unused event warning

    public class EditorInventoryService : IInventoryService
    {
        public IReadOnlyDictionary<string, IInventoryItem> items { get; } = new Dictionary<string, IInventoryItem>();

        public ulong worldCurrencyBalance { get; } = 0;

        public event IInventoryService.OnWorldCurrencyBalanceChangedDelegate onWorldCurrencyBalanceChanged;
        public event IInventoryService.OnItemOwnedChangedDelegate onItemOwnedChanged;
        public event IInventoryService.OnItemUsedDelegate onItemUsed;
        public event IInventoryService.OnItemConsumedDelegate onItemConsumed;

        public AwardWorldCurrencyRequest AwardWorldCurrency(ulong amount)
        {
            AwardWorldCurrencyRequest request = new();
            request.InvokeCompletionEvent();
            return request;
        }

        public AddInventoryItemRequest AddItem(string itemID, ulong amount = 1, bool silent = false)
        {
            AddInventoryItemRequest request = new();
            request.InvokeCompletionEvent();
            return request;
        }

        public DeleteInventoryItemRequest DeleteItem(string itemID)
        {
            DeleteInventoryItemRequest request = new();
            request.InvokeCompletionEvent();
            return request;
        }

        public void SetItemTypeEnabled(ItemType itemType, bool enabled, string disabledMessage) { }
    }
}