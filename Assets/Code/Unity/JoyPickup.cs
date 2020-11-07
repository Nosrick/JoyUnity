using System.Collections.Generic;
using DevionGames;
using DevionGames.InventorySystem;
using DevionGames.UIWidgets;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Rollers;
using UnityEngine;

namespace JoyLib.Code.Unity
{
    public class JoyPickup : Pickup
    {
        protected static GameObject s_WorldObjects;
        protected static LiveEntityHandler s_EntityHandler;

        public JoyPickup()
        {
            if (s_WorldObjects is null)
            {
                s_WorldObjects = GameObject.Find("WorldObjects");
                s_EntityHandler = GameObject.Find("GameManager").GetComponent<LiveEntityHandler>();
            }
        }

        protected override ActionStatus PickupItems()
        {
            ItemBehaviourHandler itemBehaviourHandler = this.gameObject.GetComponent<ItemBehaviourHandler>();
            if (itemBehaviourHandler.MyJoyObject is ItemInstance item)
            {
                bool result = ItemBehaviourHandler.LiveItemHandler.RemoveItemFromWorld(item.GUID);
                result &= itemBehaviourHandler.EntityInRange.AddContents(item);
                GameObject.DestroyImmediate(this.gameObject);
                if (result)
                {
                    return ActionStatus.Success;
                }
            }

            return ActionStatus.Failure;
        }

        protected override void DropItem(Item item)
        {
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