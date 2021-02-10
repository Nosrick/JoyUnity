using System.Collections.Generic;
using JoyLib.Code.Entities;
using JoyLib.Code.Quests;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JoyLib.Code.Unity.GUI
{
    public class QuestJournal : MonoBehaviour
    {
        public IEntity Player { get; set; }
        
        protected ILiveEntityHandler EntityHandler { get; set; }
        
        protected IQuestTracker QuestTracker { get; set; }

        [SerializeField] protected GameObject MenuItemPrefab;
        [SerializeField] protected LayoutGroup MenuContainer;
        protected RectTransform MenuItemRect { get; set; }
        
        protected List<MenuItem> MenuItems { get; set; }

        public void Awake()
        {
            this.MenuItemRect = this.MenuItemPrefab.GetComponent<RectTransform>();
            this.MenuItems = new List<MenuItem>();
            this.FindBits();
        }

        public void OnEnable()
        {
            this.FindBits();
            this.Repaint();
        }

        protected void FindBits()
        {
            if (GlobalConstants.GameManager is null)
            {
                return;
            }

            this.QuestTracker = GlobalConstants.GameManager.QuestTracker ?? this.QuestTracker;
            this.EntityHandler = GlobalConstants.GameManager.EntityHandler ?? this.EntityHandler;
            this.Player = this.EntityHandler?.GetPlayer();
        }

        public void Repaint()
        {
            if (this.Player is null)
            {
                return;
            }
            
            List<IQuest> quests = this.QuestTracker.GetQuestsForEntity(this.Player.Guid);
            if (quests.Count > this.MenuItems.Count)
            {
                int difference = quests.Count - this.MenuItems.Count;
                for (int i = 0; i < difference; i++)
                {
                    this.MenuItems.Add(
                        Instantiate(this.MenuItemPrefab, this.MenuContainer.transform)
                            .GetComponent<MenuItem>());
                }
            }
            
            for (int i = 0; i < quests.Count; i++)
            {
                IQuest quest = quests[i];
                this.MenuItems[i].GetComponentInChildren<TextMeshProUGUI>().text = quest.ToString();
                this.MenuItems[i].gameObject.SetActive(true);
                this.MenuItems[i].Trigger.AddListener(
                    delegate
                    {
                        this.SetUpContextMenu(quest);
                    });
            }

            for (int i = quests.Count; i < this.MenuItems.Count; i++)
            {
                this.MenuItems[i].gameObject.SetActive(false);
            }

            if (quests.Count == 0)
            {
                if (this.MenuItems.Count == 0)
                {
                    this.MenuItems.Add(
                        Instantiate(this.MenuItemPrefab, this.MenuContainer.transform)
                            .GetComponent<MenuItem>());
                }

                this.MenuItems[0].GetComponentInChildren<TextMeshProUGUI>().text = "You have no quests.";
                this.MenuItems[0].gameObject.SetActive(true);
                this.MenuItems[0].Trigger = null;
            }
        }

        protected void SetUpContextMenu(IQuest quest)
        {
            ContextMenu contextMenu = GlobalConstants.GameManager.GUIManager.OpenGUI(GUINames.CONTEXT_MENU)
                .GetComponent<ContextMenu>();
            
            contextMenu.Clear();

            contextMenu.AddMenuItem("Abandon",
                delegate
                {
                    this.AbandonQuest(quest);
                });
            
            contextMenu.Show();
        }

        protected void AbandonQuest(IQuest quest)
        {
            this.QuestTracker.AbandonQuest(this.Player, quest);
            this.Repaint();
        }
    }
}