using GameSettings;
using GameSettings.UI;
using JoyLib.Code.Settings;
using TMPro;
using UnityEngine;

namespace Code.Unity.GUI.SettingsScreen
{
    public class SettingItem : MonoBehaviour
    {
        [SerializeField] protected TextMeshProUGUI m_NameText;
        [SerializeField] protected TextMeshProUGUI m_ValueText;
        [SerializeField] protected GameObject m_Container;
        [SerializeField] protected SettingsSlider s_SliderPrefab;
        [SerializeField] protected SettingsToggle s_TogglePrefab;

        protected GameObject m_SettingsObject;

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
            if (this.m_SettingsObject is null == false)
            {
                Destroy(this.m_SettingsObject);
            }
            switch (this.MySetting)
            {
                case IntRangeSetting intSetting:
                {
                    SettingsSlider slider = Instantiate(s_SliderPrefab, this.m_Container.transform);
                    slider.gameSetting = intSetting;
                    slider.slider.minValue = intSetting.Min;
                    slider.slider.maxValue = intSetting.Max;
                    slider.slider.value = intSetting.value;
                    slider.slider.wholeNumbers = true;
                    slider.slider.onValueChanged.AddListener(this.FloatValueChanged);
                    this.m_SettingsObject = slider.gameObject;
                    break;
                }
                case FloatRangeSetting floatSetting:
                {
                    SettingsSlider slider = Instantiate(s_SliderPrefab, this.m_Container.transform);
                    slider.gameSetting = floatSetting;
                    slider.slider.minValue = floatSetting.Min;
                    slider.slider.maxValue = floatSetting.Max;
                    slider.slider.value = floatSetting.value;
                    slider.slider.wholeNumbers = false;
                    slider.slider.onValueChanged.AddListener(this.FloatValueChanged);
                    this.m_SettingsObject = slider.gameObject;
                    break;
                }
                case BoolSetting boolSetting:
                {
                    SettingsToggle toggle = Instantiate(s_TogglePrefab, this.m_Container.transform);
                    toggle.gameSetting = boolSetting;
                    toggle.toggle.isOn = boolSetting.value;
                    toggle.toggle.onValueChanged.AddListener(this.BoolValueChanged);
                    this.m_SettingsObject = toggle.gameObject;
                    break;
                }
            }

            this.m_ValueText.text = this.MySetting.objectValue.ToString();
            this.m_NameText.text = this.MySetting.settingName;
        }

        protected void ValueChanged(dynamic value)
        {
            this.MySetting.objectValue = value;
            this.m_ValueText.text = this.MySetting.objectValue.ToString();
        }

        protected void BoolValueChanged(bool value)
        {
            this.ValueChanged(value);
        }

        protected void FloatValueChanged(float value)
        {
            this.ValueChanged(value);
        }

        protected void IntValueChanged(int value)
        {
            this.ValueChanged(value);
        }
    }
}