using System;

namespace SpatialSys.UnitySDK.EditorSimulation
{
    public class EditorMarketplaceService : IMarketplaceService
    {
        public event Action<ItemPurchasedEventArgs> onItemPurchased;

        public PurchaseItemRequest PurchaseItem(string itemID, ulong amount = 1, bool silent = false)
        {
            PurchaseItemRequest request = new() {
                itemID = itemID,
                amount = amount,
                succeeded = true
            };
            request.InvokeCompletionEvent();
            onItemPurchased?.Invoke(new ItemPurchasedEventArgs {
                itemID = itemID,
            });
            return request;
        }
    }
}
