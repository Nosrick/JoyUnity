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
        protected List<MenuItem> MenuList
        {
            get;
            set;
        }

        protected void OnEnable()
        {
            this.MenuItem.SetActive(false);
            
            if (GlobalConstants.GameManager.ConversationEngine is null == false 
                && (this.ConversationEngine is null
                || this.ConversationEngine.Guid != GlobalConstants.GameManager.ConversationEngine?.Guid))
            {
                this.MenuList = new List<MenuItem>();
                
                this.ConversationEngine = GlobalConstants.GameManager.ConversationEngine;
                
                this.ConversationEngine.OnOpen -= this.SetActors;
                this.ConversationEngine.OnConverse -= this.SetTitle;
                this.ConversationEngine.OnConverse -= this.CreateMenuItems;
                this.ConversationEngine.OnClose -= this.CloseMe;
                
                this.ConversationEngine.OnOpen += this.SetActors;
                this.ConversationEngine.OnConverse += this.SetTitle;
                this.ConversationEngine.OnConverse += this.CreateMenuItems;
                this.ConversationEngine.OnClose += this.CloseMe;
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
                    MenuItem child = Instantiate(this.MenuItem, this.Window.transform).GetComponent<MenuItem>();
                    child.Awake();
                    this.MenuList.Add(child);
                }
            }
            
            for(int i = 0; i < this.ConversationEngine.CurrentTopics.Length; i++)
            {
                var current = this.MenuList[i]; 
                current.Text.text = this.ConversationEngine.CurrentTopics[i].Words;
                current.gameObject.SetActive(true);
                current.AddListener(
                    delegate
                    {
                        this.OnItemClick(current.gameObject);
                    });
            }

            for (int i = this.ConversationEngine.CurrentTopics.Length; i < this.MenuList.Count; i++)
            {
                this.MenuList[i].gameObject.SetActive(false);
                this.MenuList[i].RemoveAllListeners();
            }
        }

        protected void OnItemClick(GameObject go)
        {
            this.ConversationEngine.Converse(go.transform.GetSiblingIndex());
        }
    }
}