
namespace SpatialSys.UnitySDK.Services
{
    public class PurchaseItemRequest : SpatialAsyncOperation
    {
        public string itemID;
        public ulong amount;
        public bool succeeded;
    }

    public interface IMarketplaceService
    {
        public delegate void OnItemPurchasedDelegate(string itemID);
        public event OnItemPurchasedDelegate onItemPurchased;

        /// <summary>
        /// Purchase an item from the shop for the current space.
        /// When the item is priced in Spatial Coins, the user will be prompted to confirm the purchase. If the item is
        /// priceed in world currency, the purchase will be made immediately without confirmation.
        /// </summary>
        /// <param name="itemID">The ID of the item to purchase</param>
        /// <param name="amount">The total amount of the same item to purchase in bulk</param>
        /// <param name="silent">If possible, don't show any confirmation messages in the UI</param>
        PurchaseItemRequest PurchaseItem(string itemID, ulong amount = 1, bool silent = false);
    }
}
