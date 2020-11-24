using System.Collections.Generic;
using DevionGames;
using DevionGames.InventorySystem;
using DevionGames.UIWidgets;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Rollers;
using JoyLib.Code.Scripting;
using UnityEngine;

namespace JoyLib.Code.Unity
{
    public class JoyPickup : Pickup
    {
        protected static GameObject s_WorldObjects;
        protected static LiveEntityHandler s_EntityHandler;

        protected void Initialise()
        {
            if (s_WorldObjects is null)
            {
                s_WorldObjects = GameObject.Find("WorldObjects");
                s_EntityHandler = GlobalConstants.GameManager.GetComponent<LiveEntityHandler>();
            }
        }

        protected override ActionStatus PickupItems()
        {
            Initialise();
            
            ItemBehaviourHandler itemBehaviourHandler = this.gameObject.GetComponent<ItemBehaviourHandler>();
            if (itemBehaviourHandler.MyJoyObject is ItemInstance item)
            {
                bool result = ItemBehaviourHandler.LiveItemHandler.RemoveItemFromWorld(item.GUID);
                IJoyAction addItemAction = itemBehaviourHandler.EntityInRange.FetchAction("additemaction");
                result &= addItemAction.Execute(
                    new IJoyObject[] {itemBehaviourHandler.EntityInRange, item},
                    new string[] {"pickup"},
                    new object[] {true});
                
                //GameObject.DestroyImmediate(this.gameObject);
                if (result)
                {
                    return ActionStatus.Success;
                }
            }

            return ActionStatus.Failure;
        }

        protected override void DropItem(Item item)
        {
            Initialise();
            
            ItemBehaviourHandler itemBehaviourHandler = this.gameObject.GetComponent<ItemBehaviourHandler>();
            if (itemBehaviourHandler.MyJoyObject is ItemInstance itemInstance)
            {
                this.gameObject.transform.parent = s_WorldObjects.transform;
                Vector2Int playerPosition = s_EntityHandler.GetPlayer().WorldPosition;
                this.gameObject.transform.position = new Vector3(playerPosition.x, playerPosition.y, 0);
            }
        }
    }
}