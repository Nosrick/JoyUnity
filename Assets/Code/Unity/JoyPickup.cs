using System.Collections.Generic;
using DevionGames;
using DevionGames.InventorySystem;
using DevionGames.UIWidgets;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Rollers;

namespace JoyLib.Code.Unity
{
    public class JoyPickup : Pickup
    {
        
        protected override ActionStatus PickupItems()
        {
            ItemBehaviourHandler itemBehaviourHandler = this.gameObject.GetComponent<ItemBehaviourHandler>();
            if (itemBehaviourHandler.MyJoyObject is ItemInstance item)
            {
                bool result = ItemBehaviourHandler.LiveItemHandler.RemoveItem(item.GUID);
                if (result)
                {
                    return ActionStatus.Success;
                }
            }

            return ActionStatus.Failure;
        }
    }
}