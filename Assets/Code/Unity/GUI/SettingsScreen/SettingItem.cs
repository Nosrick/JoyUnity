using System;
using GameSettings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Unity.GUI.SettingsScreen
{
    public class SettingItem : MonoBehaviour
    {
        [SerializeField] protected TextMeshProUGUI m_NameText;
        [SerializeField] protected TextMeshProUGUI m_ValueText;
        [SerializeField] protected Slider m_RangeObject;

        public GameSetting MySetting
        {
            get => this.m_Setting;
            set
            {
                this.m_Setting = value;
                this.SetUp();
            }
        }
        protected GameSetting m_Setting;

        public string NameText
        {
            get => this.m_NameText.text;
        }

        protected void SetUp()
        {
            this.m_RangeObject.onValueChanged.RemoveAllListeners();
            this.m_RangeObject.onValueChanged.AddListener(this.ValueChanged);
            this.m_RangeObject.value = Convert.ToSingle(this.MySetting.objectValue);
            this.m_ValueText.text = this.MySetting.objectValue.ToString();
            this.m_NameText.text = this.MySetting.settingName;
        }

        protected void ValueChanged(float value)
        {
            this.MySetting.objectValue = value;
            this.m_ValueText.text = this.MySetting.objectValue.ToString();
        }
    }
}