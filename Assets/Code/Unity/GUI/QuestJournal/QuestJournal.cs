using System.Collections.Generic;
using JoyLib.Code.Entities;
using JoyLib.Code.Quests;
using UnityEngine;
using UnityEngine.UI;

namespace JoyLib.Code.Unity.GUI
{
    public class QuestJournal : GUIData
    {
        public IEntity Player { get; set; }
        
        protected ILiveEntityHandler EntityHandler { get; set; }
        
        protected IQuestTracker QuestTracker { get; set; }

        [SerializeField] protected GameObject MenuItemPrefab;
        protected RectTransform MenuItemRect { get; set; }
        
        protected List<MenuItem> MenuItems { get; set; }
        
        protected GameObject Container { get; set; }

        public override void Awake()
        {
            base.Awake();
            MenuItemRect = MenuItemPrefab.GetComponent<RectTransform>();
            Container = this.gameObject.transform.Find("Quest MenuContainer").gameObject;
            MenuItems = new List<MenuItem>();
            FindBits();
        }

        public void OnEnable()
        {
            FindBits();
            Repaint();
        }

        protected void FindBits()
        {
            if (GlobalConstants.GameManager is null)
            {
                return;
            }
            QuestTracker = GlobalConstants.GameManager.QuestTracker ?? QuestTracker;
            EntityHandler = GlobalConstants.GameManager.EntityHandler ?? EntityHandler;
            Player = EntityHandler is null == false ? Player is null ? EntityHandler.GetPlayer() : Player : Player;
        }

        public void Repaint()
        {
            if (Player is null)
            {
                return;
            }
            
            List<IQuest> quests = QuestTracker.GetQuestsForEntity(Player.GUID);
            if (quests.Count > MenuItems.Count)
            {
                int difference = quests.Count - MenuItems.Count;
                for (int i = 0; i < difference; i++)
                {
                    MenuItems.Add(
                        GameObject.Instantiate(MenuItemPrefab, Container.transform)
                            .GetComponent<MenuItem>());
                }
            }
            
            for (int i = 0; i < quests.Count; i++)
            {
                MenuItems[i].GetComponentInChildren<Text>().text = quests[i].ToString();
                MenuItems[i].gameObject.SetActive(true);
            }

            for (int i = quests.Count; i < MenuItems.Count; i++)
            {
                MenuItems[i].gameObject.SetActive(false);
            }

            if (quests.Count == 0)
            {
                if (MenuItems.Count == 0)
                {
                    MenuItems.Add(
                        GameObject.Instantiate(MenuItemPrefab, Container.transform)
                            .GetComponent<MenuItem>());
                }

                MenuItems[0].GetComponentInChildren<Text>().text = "You have no quests.";
                MenuItems[0].gameObject.SetActive(true);
            }
        }
    }
}