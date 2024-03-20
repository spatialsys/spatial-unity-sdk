namespace SpatialSys.UnitySDK
{
    [DocumentationCategory("Services/Marketplace Service")]
    public interface IMarketplaceService
    {
        /// <summary>
        /// Triggered when an item is purchased by the local user.
        /// </summary>
        event OnItemPurchasedDelegate onItemPurchased;
        public delegate void OnItemPurchasedDelegate(ItemPurchasedEventArgs args);

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

    [DocumentationCategory("Services/Marketplace Service")]
    public class PurchaseItemRequest : SpatialAsyncOperation
    {
        public string itemID;
        public ulong amount;
        public bool succeeded;
    }

    [DocumentationCategory("Services/Marketplace Service")]
    public struct ItemPurchasedEventArgs
    {
        public string itemID;
    }
}
