using System.Collections.Generic;

namespace SpatialSys.UnitySDK
{
    /// <summary>
    /// Service to handle inventory and currency
    /// </summary>
    [DocumentationCategory("Services/Inventory Service")]
    public interface IInventoryService
    {
        /// <summary>
        /// A dictionary of all items in the user's inventory, keyed by item ID.
        /// </summary>
        IReadOnlyDictionary<string, IInventoryItem> items { get; }

        /// <summary>
        /// The user's world currency balance if this space or the world that this space belongs to has a currency.
        /// </summary>
        ulong worldCurrencyBalance { get; }

        /// <summary>
        /// Triggered when the user's world currency balance changes.
        /// </summary>
        event OnWorldCurrencyBalanceChangedDelegate onWorldCurrencyBalanceChanged;
        public delegate void OnWorldCurrencyBalanceChangedDelegate(ulong balance);

        /// <summary>
        /// Triggered when user acquires an item.
        /// </summary>
        event OnItemOwnedChangedDelegate onItemOwnedChanged;
        public delegate void OnItemOwnedChangedDelegate(string itemID, bool isOwned);

        /// <summary>
        /// Triggered when the user uses an item.
        /// </summary>
        event OnItemUsedDelegate onItemUsed;
        public delegate void OnItemUsedDelegate(string itemID);

        /// <summary>
        /// Triggered when user consumes an item. This is only relevant for consumable items.
        /// </summary>
        event OnItemConsumedDelegate onItemConsumed;
        public delegate void OnItemConsumedDelegate(string itemID);

        /// <summary>
        /// Award world currency to the user.
        /// </summary>
        /// <param name="amount">Amount of the currency to add</param>
        /// <returns>Award world currency request</returns>
        AwardWorldCurrencyRequest AwardWorldCurrency(ulong amount);

        /// <summary>
        /// Add an item to the user's inventory.
        /// </summary>
        /// <param name="itemID">Item to add to backpack</param>
        /// <param name="amount">Amount to add</param>
        /// <param name="silent">True if operation should show a toast message, otherwise false.</param>
        /// <returns>Add Item request</returns>
        AddInventoryItemRequest AddItem(string itemID, ulong amount = 1, bool silent = false);

        /// <summary>
        /// Delete an item from the user's inventory. This will only work if the item is in the same world as the space.
        /// </summary>
        /// <param name="itemID">Item to remove from backpack</param>
        /// <returns>Delete item request</returns>
        DeleteInventoryItemRequest DeleteItem(string itemID);

        /// <summary>
        /// Set the enabled state for items by their type. This overrides individual item enabled states.
        /// </summary>
        /// <param name="itemType">Type of the item</param>
        /// <param name="enabled">Enabled or disabled</param>
        /// <param name="disabledMessage">The tooltip or explanation message to show to the user explaining why the item is disabled. Maximum 50 characters.</param>
        void SetItemTypeEnabled(ItemType itemType, bool enabled, string disabledMessage);
    }

    /// <summary>
    /// Represents an item in the user's inventory / backpack.
    /// </summary>
    [DocumentationCategory("Services/Inventory Service")]
    public interface IInventoryItem
    {
        /// <summary>
        /// Item ID. This can be found on <see href="https://www.spatial.io/studio">Spatial Studio</see>
        /// </summary>
        string itemID { get; }

        /// <summary>
        /// Does the user have this item in their inventory?
        /// </summary>
        bool isOwned { get; }

        /// <summary>
        /// The amount of the item owned in the inventory
        /// </summary>
        ulong amount { get; }

        /// <summary>
        /// True if the item is consumable
        /// </summary>
        bool isConsumable { get; }

        /// <summary>
        /// How long the consumable item's effects remains.
        /// </summary>
        float consumableDurationRemaining { get; }

        /// <summary>
        /// True if the item is being consumed (consumableDurationRemaining > 0)
        /// </summary>
        bool isConsumeActive { get; }

        /// <summary>
        /// Cooldown time remaining
        /// </summary>
        float consumableCooldownRemaining { get; }

        /// <summary>
        /// True if the item is on cooldown (consumableCooldownRemaining > 0)
        /// </summary>
        bool isOnCooldown { get; }

        /// <summary>
        /// Triggered when an item's amount has changed.
        /// </summary>
        event OnItemAmountChangedDelegate onItemAmountChanged;
        public delegate void OnItemAmountChangedDelegate(ulong amount);

        /// <summary>
        /// Triggered when the user uses an item.
        /// </summary>
        event OnItemUsedDelegate onItemUsed;
        public delegate void OnItemUsedDelegate();

        /// <summary>
        /// Triggered when user uses a consumable item (item's amount decreases by 1) before consume duration timer starts.
        /// </summary>
        event OnItemConsumedDelegate onItemConsumed;
        public delegate void OnItemConsumedDelegate();

        /// <summary>
        /// Triggered when a consumable item's effects finish (consumableDurationRemaining == 0).
        /// </summary>
        event OnConsumableItemDurationExpiredDelegate onConsumableItemDurationExpired;
        public delegate void OnConsumableItemDurationExpiredDelegate();

        /// <summary>
        /// Set the enabled state for an item in the user's inventory. This can be useful to prevent users from equipping
        /// or consuming certain items based on logic within your experience.
        /// All items in the user's inventory are enabled by default. 
        /// </summary>
        /// <param name="enabled">Enabled or disabled</param>
        /// <param name="disabledMessage">The tooltip or explanation message to show to the user explaining why the item is disabled</param>
        void SetEnabled(bool enabled, string disabledMessage);

        /// <summary>
        /// Use the item. This will trigger the OnItemUsed event.
        /// </summary>
        /// <returns>Uses the item. If it's a consumable, consumes one out of the amount.</returns>
        UseInventoryItemRequest Use();
    }

    [DocumentationCategory("Services/Inventory Service")]
    public class AwardWorldCurrencyRequest : SpatialAsyncOperation
    {
        /// <summary>
        /// The amount of currency to award
        /// </summary>
        public ulong amount;

        /// <summary>
        /// True if the award request succeeded
        /// </summary>
        public bool succeeded;
    }

    [DocumentationCategory("Services/Inventory Service")]
    public class AddInventoryItemRequest : SpatialAsyncOperation
    {
        /// <summary>
        /// The ID of the item to add
        /// </summary>
        public string itemID;

        /// <summary>
        /// The amount of the item to add
        /// </summary>
        public ulong amount;

        /// <summary>
        /// True if the item was added successfully
        /// </summary>
        public bool succeeded;
    }

    [DocumentationCategory("Services/Inventory Service")]
    public class DeleteInventoryItemRequest : SpatialAsyncOperation
    {
        /// <summary>
        /// The ID of the item to delete
        /// </summary>
        public string itemID;

        /// <summary>
        /// True if the item was deleted successfully
        /// </summary>
        public bool succeeded;
    }

    [DocumentationCategory("Services/Inventory Service")]
    public class GetInventoryItemRequest : SpatialAsyncOperation
    {
        /// <summary>
        /// The ID of the item to get
        /// </summary>
        public string itemID;

        /// <summary>
        /// True if the item was found
        /// </summary>
        public bool succeeded;
    }

    [DocumentationCategory("Services/Inventory Service")]
    public class UseInventoryItemRequest : SpatialAsyncOperation
    {
        /// <summary>
        /// The ID of the item to use
        /// </summary>
        public string itemID;

        /// <summary>
        /// True if the item was used successfully
        /// </summary>
        public bool succeeded;
    }
}
