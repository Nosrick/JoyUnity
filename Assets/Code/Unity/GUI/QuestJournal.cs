using System;
using System.Collections.Generic;
using DevionGames;
using DevionGames.UIWidgets;
using JoyLib.Code.Entities;
using JoyLib.Code.Quests;
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
        
        protected List<MenuItem> MenuItems { get; set; }
        
        protected GameObject Container { get; set; }

        public void Awake()
        {
            if (MenuItemPrefab is null)
            {
                MenuItemPrefab = this.gameObject.FindChild("Menu Item", true);
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
                MenuItems.Add(
                    GameObject.Instantiate(MenuItemPrefab, Container.transform)
                        .GetComponent<MenuItem>());
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
        }
    }
}