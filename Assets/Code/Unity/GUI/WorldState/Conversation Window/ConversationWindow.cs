using System;
using System.Collections.Generic;
using JoyLib.Code.Conversation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JoyLib.Code.Unity.GUI
{
    public class ConversationWindow : MonoBehaviour
    {
        [SerializeField] protected TextMeshProUGUI LastSaidGUI;
        [SerializeField] protected ManagedUISprite LastSpokeIcon;
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
            this.MenuItem.SetActive(false);
            
            if (GlobalConstants.GameManager.ConversationEngine is null == false && this.ConversationEngine is null)
            {
                this.MenuList = new List<ConversationMenu>();
                this.ConversationEngine = GlobalConstants.GameManager.ConversationEngine;
                this.ConversationEngine.OnOpen += new EventHandler(this.SetActors);
                this.ConversationEngine.OnConverse += new EventHandler(this.SetTitle);
                this.ConversationEngine.OnConverse += new EventHandler(this.CreateMenuItems);
                this.ConversationEngine.OnClose += new EventHandler(this.CloseMe);
            }
        }

        protected void CloseMe(object sender, EventArgs args)
        {
            GlobalConstants.GameManager.GUIManager.CloseGUI(GUINames.CONVERSATION);
        }

        protected void SetActors(object sender, EventArgs args)
        {
            this.LastSpokeIcon.Clear();
            this.LastSpokeIcon.AddSpriteState(this.ConversationEngine.Listener.MonoBehaviourHandler.CurrentSpriteState);
            this.LastSpokeName.text = this.ConversationEngine.ListenerInfo;
            this.LastSaidGUI.text = this.ConversationEngine.LastSaidWords;
        }
        
        protected void SetTitle(object sender, EventArgs args)
        {
            double remainingWidth = this.TitleRect.rect.width - this.ListenerSection.rect.width;

            //LastSaidGUI.text = WrapText(LastSaidGUI, text, remainingWidth);
            this.LastSaidGUI.text = this.ConversationEngine.LastSaidWords;
            LayoutRebuilder.ForceRebuildLayoutImmediate(this.TitleRect);
        }
        
        protected void CreateMenuItems(object sender, EventArgs args)
        {
            if (this.ConversationEngine.CurrentTopics.Length > this.MenuList.Count)
            {
                for (int i = this.MenuList.Count; i < this.ConversationEngine.CurrentTopics.Length; i++)
                {
                    ConversationMenu child = Instantiate(this.MenuItem, this.Window.transform).GetComponent<ConversationMenu>();
                    child.Awake();
                    this.MenuList.Add(child);
                }
            }
            
            for(int i = 0; i < this.ConversationEngine.CurrentTopics.Length; i++)
            {
                this.MenuList[i].TopicID = this.ConversationEngine.CurrentTopics[i].ID;
                this.MenuList[i].SetText(this.ConversationEngine.CurrentTopics[i].Words);
                this.MenuList[i].gameObject.SetActive(true);
                this.MenuList[i].Index = i;
            }

            for (int i = this.ConversationEngine.CurrentTopics.Length; i < this.MenuList.Count; i++)
            {
                this.MenuList[i].gameObject.SetActive(false);
            }
        }
    }
}