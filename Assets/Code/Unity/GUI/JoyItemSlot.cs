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
        
        public static ILiveEntityHandler EntityHandler { get; set; }
        
        protected static IEntity Player { get; set; }

        public void Awake()
        {
            GetBits();
        }

        protected void GetBits()
        {
            if (GlobalConstants.GameManager is null || GUIManager is null == false)
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

            if (ObservedItem is null == false && ObservedItem is ItemInstance item)
            {
                if (Container.useButton.HasFlag((InputButton)Mathf.Clamp(((int)eventData.button * 2), 1, int.MaxValue))
                && item.HasTag("container"))
                {
                    menu.AddMenuItem("Open", this.OpenContainer);
                    menu.Show();
                }
                Player = EntityHandler?.GetPlayer();
                item.SetUser(Player);
            }
        }

        protected void OpenContainer()
        {
            if (this.ObservedItem is ItemInstance item
            && item.HasTag("container"))
            {
                GUIManager?.OpenGUI(GUINames.INVENTORY_CONTAINER);
                MutableItemContainer container = GUIManager?.GetGUI(GUINames.INVENTORY_CONTAINER).GetComponent<MutableItemContainer>();
                container.Owner = item;
                container.RemoveItems();
                foreach (IItemInstance content in item.Contents)
                {
                    if (content is ItemInstance instance)
                    {
                        container.StackOrAdd(instance);
                    }
                }
            }
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

                //Activate the game object
                GameObject go = item.MonoBehaviourHandler.gameObject;
                Player.FetchAction("placeiteminworldaction")
                    .Execute(new IJoyObject[] {Player, item});
                go.SetActive(true);

                ItemContainer.RemoveItemCompletely(item);
                Container.NotifyDropItem(item, go);
            }
        }
    }
}