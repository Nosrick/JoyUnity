using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DevionGames.InventorySystem
{
    /// <summary>
    /// Wrapper storage class for trigger events
    /// </summary>
    public class TriggerEventData : CallbackEventData
    {
        /// <summary>
        /// Current used trigger
        /// </summary>
        public Trigger trigger;
        /// <summary>
        /// Reference to player game object
        /// </summary>
        public GameObject player;
        /// <summary>
        /// Current pointer event data
        /// </summary>
        public PointerEventData eventData;
        /// <summary>
        /// Current used item. For example in OnSoldItem, this is the item sold.
        /// </summary>
        public Item item;
        /// <summary>
        /// Failure cause of an interaction
        /// </summary>
        public Trigger.FailureCause failureCause;

    }
}