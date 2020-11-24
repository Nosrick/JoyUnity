using System;
using System.Collections.Generic;
using System.Linq;
using DevionGames;
using DevionGames.InventorySystem;
using DevionGames.UIWidgets;
using JoyLib.Code.Entities;
using JoyLib.Code.Quests;
using Lean.Gui;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JoyLib.Code.Unity.GUI
{
    public class QuestJournal : MonoBehaviour
    {
        public Entity Player { get; set; }
        
        protected LiveEntityHandler EntityHandler { get; set; }
        
        protected QuestTracker QuestTracker { get; set; }
        
        protected GameObject MenuItemPrefab { get; set; }
        protected RectTransform MenuItemRect { get; set; }
        
        protected List<MenuItem> MenuItems { get; set; }
        
        protected GameObject Container { get; set; }

        public void Awake()
        {
            if (MenuItemPrefab is null)
            {
                MenuItemPrefab = this.gameObject.FindChild("Menu Item", true);
                MenuItemRect = MenuItemPrefab.GetComponent<RectTransform>();
                Container = this.gameObject.FindChild("Quest MenuContainer", true);
                MenuItems = new List<MenuItem>();
                QuestTracker = GlobalConstants.GameManager.GetComponent<QuestTracker>();
                EntityHandler = GlobalConstants.GameManager.GetComponent<LiveEntityHandler>();
                Player = EntityHandler.GetPlayer();
            }
        }

        public void OnEnable()
        {
            Repaint();
        }

        public void Repaint()
        {
            Player = EntityHandler.GetPlayer();
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

            GameObject questMenuContainer = this.gameObject.FindChild("Quest MenuContainer", true);
            LeanConstrainAnchoredPosition constraint = questMenuContainer.GetComponent<LeanConstrainAnchoredPosition>();
            VerticalLayoutGroup layoutGroup = questMenuContainer.GetComponent<VerticalLayoutGroup>();
            int activeMenuItems = MenuItems.Count(item => item.IsActive());
            constraint.VerticalMax = (activeMenuItems * (MenuItemRect.rect.height + layoutGroup.spacing)) / 2;
        }
    }
}