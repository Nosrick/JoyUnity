using JoyLib.Code.Settings;
using TMPro;
using UnityEngine;

namespace Code.Unity.GUI.SettingsScreen
{
    public class SettingItem : MonoBehaviour
    {
        [SerializeField] protected TextMeshProUGUI m_NameText;
        [SerializeField] protected GameObject m_RangeObject;

        public SettingData MySetting
        {
            get => this.m_Setting;
            set
            {
                this.m_Setting = value;
                this.SetUp();
            }
        }

        protected SettingData m_Setting;

        protected void SetUp()
        {
            
        }
    }
}