using System;
using DevionGames.InventorySystem;
using UnityEngine.EventSystems;
using JoyLib.Code.Conversation;
using JoyLib.Code.Entities;
using UnityEngine;
using ContextMenu = DevionGames.UIWidgets.ContextMenu;

namespace JoyLib.Code.Unity.GUI
{
    public class JoyItemSlot : ItemSlot
    {
        protected static ConversationEngine ConversationEngine { get; set; }
        
        protected static GUIManager GUIManager { get; set; }

        public void Awake()
        {
            if (!(ConversationEngine is null))
            {
                return;
            }
            ConversationEngine = GameObject.Find("GameManager").GetComponent<ConversationEngine>();
            GUIManager = GameObject.Find("GameManager").GetComponent<GUIManager>();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);

            if (eventData.dragging)
            {
                return;
            }
            
            ContextMenu menu = InventoryManager.UI.contextMenu;
            if (menu is null || Container.UseContextMenu == false)
            {
                return;
            }
            
            if (Container.useButton.HasFlag((InputButton)Mathf.Clamp(((int)eventData.button * 2), 1, int.MaxValue)))
            {
                menu.AddMenuItem("Give", GiveItem);
                            
                menu.Show();
            }
           
        }

        protected void GiveItem()
        {
            Entity left = ConversationEngine.Instigator;
            Entity right = ConversationEngine.Listener;

            if (left is null || right is null)
            {
                return;
            }

            if (ObservedItem is JoyItem joyItem)
            {
                left.FetchAction("giveitemaction").Execute(
                    new IJoyObject[] {left, right},
                    new string[] {"give"},
                    joyItem.ItemInstance);
                
                GUIManager.CloseGUI("Inventory");
            }
        }
    }
}