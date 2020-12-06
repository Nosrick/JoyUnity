using DevionGames;
using DevionGames.InventorySystem;
using JoyLib.Code.Conversation;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using UnityEngine;
using UnityEngine.EventSystems;
using ContextMenu = DevionGames.UIWidgets.ContextMenu;

namespace JoyLib.Code.Unity.GUI
{
    public class JoyItemSlot : ItemSlot
    {
        public static IConversationEngine ConversationEngine { get; set; }
        
        public static IGUIManager GUIManager { get; set; }
        
        public static GameObject ItemHolder { get; set;}
        
        public static ILiveEntityHandler EntityHandler { get; set; }
        
        protected static IEntity Player { get; set; }

        public void Awake()
        {
            GetBits();
        }

        protected void GetBits()
        {
            if (GlobalConstants.GameManager is null)
            {
                return;
            }
            
            ConversationEngine = GlobalConstants.GameManager.ConversationEngine;
            GUIManager = GlobalConstants.GameManager.GUIManager;
            EntityHandler = GlobalConstants.GameManager.EntityHandler;
        }
        
        public override void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.dragging)
            {
                return;
            }
            
            ContextMenu menu = InventoryManager.UI.contextMenu;
            if (menu is null || Container.UseContextMenu == false)
            {
                return;
            }
            
            base.OnPointerUp(eventData);
            
            GetBits();
            
            if (Container.useButton.HasFlag((InputButton)Mathf.Clamp(((int)eventData.button * 2), 1, int.MaxValue)))
            {
                menu.AddMenuItem("Give", GiveItem);
                            
                menu.Show();
            }

            if (ObservedItem is null == false && ObservedItem is ItemInstance item)
            {
                Player = EntityHandler?.GetPlayer();
                item.SetUser(Player);
            }
        }

        protected void GiveItem()
        {
            IEntity left = ConversationEngine?.Instigator;
            IEntity right = ConversationEngine?.Listener;

            if (left is null || right is null)
            {
                return;
            }

            if (!(ObservedItem is ItemInstance joyItem))
            {
                return;
            }
            
            left.FetchAction("giveitemaction").Execute(
                new IJoyObject[] {left, right},
                new string[] {"give"},
                joyItem);
                
            GUIManager?.CloseGUI("Inventory");
        }

        protected override void DropItem()
        {
            GetBits();
            
            //Get the item to drop
            ItemInstance item = dragObject != null ? (ItemInstance)dragObject.item : (ItemInstance)ObservedItem;

            //Check if the item is droppable
            if (item != null && item.IsDroppable)
            {
                //Get item prefab
                GameObject prefab = item.OverridePrefab != null ? item.OverridePrefab : item.Prefab;
                RaycastHit hit;
                Vector3 position = Vector3.zero;
                Vector3 forward = Vector3.zero;
                if (InventoryManager.current.PlayerInfo.transform != null)
                {
                    position = InventoryManager.current.PlayerInfo.transform.position;
                    forward = InventoryManager.current.PlayerInfo.transform.forward;
                }

                //Cast a ray from mouse postion to ground
                if (UnityEngine.Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit) &&
                    !UnityTools.IsPointerOverUI())
                {
                    //Clamp the drop distance to max drop distance defined in setting.
                    Vector3 worldPos = hit.point;
                    Vector3 diff = worldPos - position;
                    float distance = diff.magnitude;
                    if (distance > (InventoryManager.DefaultSettings.maxDropDistance - (transform.localScale.x / 2)))
                    {
                        position = position + (diff / distance) * InventoryManager.DefaultSettings.maxDropDistance;
                    }
                    else
                    {
                        position = worldPos;
                    }
                }
                else
                {
                    position = position + forward;
                }

                //Instantiate the prefab at position
                GameObject go = InventoryManager.Instantiate(prefab, position + Vector3.up * 0.3f, Quaternion.identity);
                go.name = go.name.Replace("(Clone)", "");
                
                go.transform.parent = ItemHolder?.transform;
                go.GetComponent<ItemBehaviourHandler>().AttachJoyObject(item);
                SpriteRenderer renderer = go.GetComponent<SpriteRenderer>(); 
                renderer.sprite = item.Sprite;
                item.Move(Vector2Int.FloorToInt(position));
                renderer.sortingLayerName = "Objects";
                
                //Reset the item collection of the prefab with this item
                ItemCollection collection = go.GetComponent<ItemCollection>();
                if (collection != null)
                {
                    collection.Clear();
                    collection.Add(item);
                }

                ItemContainer.RemoveItemCompletely(item);
                Container.NotifyDropItem(item, go);
            }
        }
    }
}