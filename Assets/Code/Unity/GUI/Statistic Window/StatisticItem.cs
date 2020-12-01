using System;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace JoyLib.Code.Unity.GUI.StatisticWindow
{
    public class StatisticItem : MonoBehaviour
    {
        protected int m_Value;
        protected string m_Name;

        public int Value
        {
            get => m_Value;
            set
            {
                m_Value = value;
                m_ValueText.text = m_Value.ToString();
            }
        }

        public string Name
        {
            get => m_Name;
            set
            {
                m_Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value);
                m_NameText.text = m_Name;
            }
        }

        [SerializeField] protected TextMeshProUGUI m_ValueText;
        [SerializeField] protected TextMeshProUGUI m_NameText;
    }
}