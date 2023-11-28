using SpatialSys.UnitySDK.Services;

namespace SpatialSys.UnitySDK.EditorSimulation
{
    public class EditorMarketplaceService : IMarketplaceService
    {
        public event IMarketplaceService.OnItemPurchasedDelegate onItemPurchased;

        public PurchaseItemRequest PurchaseItem(string itemID, ulong amount = 1, bool silent = false)
        {
            PurchaseItemRequest request = new() {
                itemID = itemID,
                amount = amount,
                succeeded = true
            };
            request.InvokeCompletionEvent();
            onItemPurchased?.Invoke(itemID);
            return request;
        }
    }
}
