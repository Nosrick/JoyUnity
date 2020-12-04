using System;
using System.Collections.Generic;
using DevionGames.UIWidgets;
using JoyLib.Code.Conversation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JoyLib.Code.Unity.GUI
{
    public class ConversationWindow : UIWidget
    {
        [SerializeField] protected GameManager GameManager;
        [SerializeField] protected TextMeshProUGUI LastSaidGUI;
        [SerializeField] protected Image LastSpokeIcon;
        [SerializeField] protected TextMeshProUGUI LastSpokeName;
        [SerializeField] protected RectTransform ListenerSection;
        [SerializeField] protected GameObject Window;
        [SerializeField] protected GameObject MenuItem;
        [SerializeField] protected RectTransform TitleRect;
        protected IConversationEngine ConversationEngine { get; set; }
        protected List<ConversationMenu> MenuList
        {
            get;
            set;
        }

        protected void OnEnable()
        {
            if (GameManager.ConversationEngine is null == false && ConversationEngine is null)
            {
                MenuList = new List<ConversationMenu>();
                ConversationEngine = GameManager.ConversationEngine;
                ConversationEngine.OnOpen += new EventHandler(SetActors);
                ConversationEngine.OnConverse += new EventHandler(SetTitle);
                ConversationEngine.OnConverse += new EventHandler(CreateMenuItems);
                ConversationEngine.OnClose += new EventHandler(CloseMe);
            }
        }

        protected void CloseMe(object sender, EventArgs args)
        {
            GameManager.GUIManager.CloseGUI(GlobalConstants.CONVERSATION);
        }

        protected void SetActors(object sender, EventArgs args)
        {
            LastSpokeIcon.sprite = ConversationEngine.Listener.Sprite;
            LastSpokeName.text = ConversationEngine.ListenerInfo;
            LastSaidGUI.text = ConversationEngine.LastSaidWords;
        }
        
        protected void SetTitle(object sender, EventArgs args)
        {
            double remainingWidth = TitleRect.rect.width - ListenerSection.rect.width;

            //LastSaidGUI.text = WrapText(LastSaidGUI, text, remainingWidth);
            LastSaidGUI.text = ConversationEngine.LastSaidWords;
            LayoutRebuilder.ForceRebuildLayoutImmediate(TitleRect);
        }
        
        protected void CreateMenuItems(object sender, EventArgs args)
        {
            if (ConversationEngine.CurrentTopics.Length > MenuList.Count)
            {
                for (int i = MenuList.Count; i < ConversationEngine.CurrentTopics.Length; i++)
                {
                    ConversationMenu child = GameObject.Instantiate(MenuItem, Window.transform).GetComponent<ConversationMenu>();
                    MenuList.Add(child);
                }
            }
            
            for(int i = 0; i < ConversationEngine.CurrentTopics.Length; i++)
            {
                MenuList[i].TopicID = ConversationEngine.CurrentTopics[i].ID;
                MenuList[i].SetText(ConversationEngine.CurrentTopics[i].Words);
                MenuList[i].gameObject.SetActive(true);
                MenuList[i].Index = i;
                ConversationMenu menu = MenuList[i].GetComponent<ConversationMenu>();
                MenuItem menuItem = MenuList[i].GetComponent<MenuItem>();
                Button.ButtonClickedEvent buttonClickedEvent = new Button.ButtonClickedEvent();
                buttonClickedEvent.AddListener(menu.OnMouseDown);
                menuItem.onTrigger = buttonClickedEvent;
            }

            for (int i = ConversationEngine.CurrentTopics.Length; i < MenuList.Count; i++)
            {
                MenuList[i].gameObject.SetActive(false);
            }
        }
    }
}