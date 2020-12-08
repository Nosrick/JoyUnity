using DevionGames;
using DevionGames.InventorySystem;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Scripting;
using UnityEngine;

namespace JoyLib.Code.Unity
{
    public class JoyPickup : Pickup
    {
        protected static GameObject s_WorldObjects;
        protected static ILiveEntityHandler s_EntityHandler;

        protected ItemBehaviourHandler ItemBehaviourHandler;

        protected void Initialise()
        {
            if (s_WorldObjects is null)
            {
                s_WorldObjects = GameObject.Find("WorldObjects");
                s_EntityHandler = GlobalConstants.GameManager.EntityHandler;
            }

            if (ItemBehaviourHandler is null)
            {
                ItemBehaviourHandler = this.gameObject.GetComponent<ItemBehaviourHandler>();
            }
        }

        protected override ActionStatus PickupItems()
        {
            Initialise();
            
            if (ItemBehaviourHandler.MyJoyObject is ItemInstance item)
            {
                bool result = ItemBehaviourHandler.LiveItemHandler.RemoveItemFromWorld(item.GUID);
                IJoyAction addItemAction = ItemBehaviourHandler.EntityInRange.FetchAction("additemaction");
                result &= addItemAction.Execute(
                    new IJoyObject[] {ItemBehaviourHandler.EntityInRange, item},
                    new string[] {"pickup"},
                    new object[] {true});

                item.MyWorld.RemoveObject(item.WorldPosition, item);
                //item.Move(ItemBehaviourHandler.EntityInRange.WorldPosition);
                
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
                IEntity player = s_EntityHandler.GetPlayer();
                Vector2Int playerPosition = player.WorldPosition;
                this.gameObject.transform.position = new Vector3(playerPosition.x, playerPosition.y, 0);
                IJoyAction placeInWorldAction = player.FetchAction("placeiteminworldaction");
                placeInWorldAction.Execute(
                    new IJoyObject[] {player, itemInstance},
                    new string[] { "drop" });
            }
        }
    }
}