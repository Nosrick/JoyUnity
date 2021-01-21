using System.Collections.Generic;
using GameSettings;
using JoyLib.Code;
using JoyLib.Code.Settings;
using JoyLib.Code.Unity.GUI;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Unity.GUI.SettingsScreen
{
    public class SettingsScreen : MonoBehaviour
    {
        [SerializeField] protected VerticalLayoutGroup m_Container;
        [SerializeField] protected SettingItem m_SettingItemPrefab;
        protected SettingsManager SettingsManager { get; set; }
        
        protected List<SettingItem> SettingItems { get; set; }
        
        public bool Initialised { get; protected set; }

        public void Awake()
        {
            this.Initialise();
        }

        protected void Initialise()
        {
            if (this.Initialised)
            {
                return;
            }

            this.SettingItems = new List<SettingItem>();
            this.SettingsManager = GlobalConstants.GameManager.SettingsManager;
            this.SetUpItems();
            this.Initialised = true;
        }

        protected void SetUpItems()
        {
            foreach (GameSetting setting in this.SettingsManager.Settings.Values)
            {
                SettingItem newItem = Instantiate(this.m_SettingItemPrefab, this.m_Container.transform);
                newItem.MySetting = setting;
            }
        }

        public void Back()
        {
            GlobalConstants.GameManager.GUIManager.CloseGUI(GUINames.SETTINGS);
        }

        public void SaveAndBack()
        {
            foreach (SettingItem item in this.SettingItems)
            {
                this.SettingsManager.SetSetting(item.MySetting);
            }
            this.SettingsManager.Save();
            
            this.Back();
        }
    }
}