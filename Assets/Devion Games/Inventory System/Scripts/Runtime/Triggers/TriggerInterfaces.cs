using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DevionGames.InventorySystem {
    /// <summary>
    /// Base class that all Trigger events inherit from.
    /// </summary>
    public interface ITriggerEventHandler
    {
    }

    public interface ITriggerUsedHandler : ITriggerEventHandler
    {
        /// <summary>
        /// Use this callback to detect trigger used events
        /// </summary>
        void OnTriggerUsed(GameObject player);
    }

    public interface ITriggerUnUsedHandler : ITriggerEventHandler
    {
        /// <summary>
        /// Use this callback to detect trigger un-used events
        /// </summary>
        void OnTriggerUnUsed(GameObject player);
    }

    public interface ITriggerCameInRange : ITriggerEventHandler {
        /// <summary>
        /// Use this callback to detect when player comes in range
        /// </summary>
        void OnCameInRange(GameObject player);
    }

    public interface ITriggerWentOutOfRange : ITriggerEventHandler
    {
        /// <summary>
        /// Use this callback to detect when player went out of range
        /// </summary>
        void OnWentOutOfRange(GameObject player);
    }

    public interface ITriggerPointerEnter : ITriggerEventHandler
    {
        /// <summary>
        /// Use this callback to detect when the pointer enters the trigger(Mouse over trigger)
        /// </summary>
        void OnPointerEnter(PointerEventData eventData);
    }

    public interface ITriggerPointerExit : ITriggerEventHandler
    {
        /// <summary>
        /// Use this callback to detect when the pointer exits the trigger(Mouse not longer over trigger)
        /// </summary>
        void OnPointerExit(PointerEventData eventData);
    }

    public interface ITriggerSelectSellItem : ITriggerEventHandler
    {
        /// <summary>
        /// Use this callback to detect when an item is selected for sell
        /// </summary>
        void OnSelectSellItem(Item item, GameObject player);
    }

    public interface ITriggerSoldItem : ITriggerEventHandler
    {
        /// <summary>
        /// Use this callback to detect when an item was sold
        /// </summary>
        void OnSoldItem(Item item, GameObject player);
    }

    public interface ITriggerFailedToSellItem: ITriggerEventHandler
    {
        /// <summary>
        /// Use this callback to detect when an item couldn't be sold
        /// </summary>
        void OnFailedToSellItem(Item item, GameObject player, Trigger.FailureCause failureCause);
    }

    public interface ITriggerSelectBuyItem : ITriggerEventHandler
    {
        /// <summary>
        /// Use this callback to detect when an item is selected for purchase
        /// </summary>
        void OnSelectBuyItem(Item item, GameObject player);
    }

    public interface ITriggerBoughtItem : ITriggerEventHandler
    {
        /// <summary>
        /// Use this callback to detect when an item was bought
        /// </summary>
        void OnBoughtItem(Item item, GameObject player);
    }

    public interface ITriggerFailedToBuyItem : ITriggerEventHandler
    {
        /// <summary>
        /// Use this callback to detect when a purchase failed(Often because no currency or if the inventory is full)
        /// </summary>
        void OnFailedToBuyItem(Item item, GameObject player, Trigger.FailureCause failureCause);
    }

    public interface ITriggerCraftStart : ITriggerEventHandler
    {
        /// <summary>
        /// Use this callback to detect when the user starts crafting
        /// </summary>
        void OnCraftStart(Item item, GameObject player);
    }

    public interface ITriggerFailedCraftStart : ITriggerEventHandler
    {
        /// <summary>
        /// Use this callback to detect when craft start failed, for example the player has no ingredients or if he is already crafting
        /// </summary>
        void OnFailedCraftStart(Item item, GameObject player, Trigger.FailureCause failureCause);
    }

    public interface ITriggerCraftItem : ITriggerEventHandler
    {
        /// <summary>
        /// Use this callback to detect when the user crafted an item
        /// </summary>
        void OnCraftItem(Item item, GameObject player);
    }

    public interface ITriggerFailedToCraftItem : ITriggerEventHandler
    {
        /// <summary>
        /// Use this callback to detect when the user failed to craft item
        /// </summary>
        void OnFailedToCraftItem(Item item, GameObject player, Trigger.FailureCause failureCause);
    }

    public interface ITriggerCraftStop : ITriggerEventHandler
    {
        /// <summary>
        /// Use this callback to detect when the user stops crafting
        /// </summary>
        void OnCraftStop(Item item, GameObject player);
    }
}