using System.Globalization;
using TMPro;
using UnityEngine;

namespace JoyLib.Code.Unity.GUI
{
    public class StatisticItem : ValueContainer
    {
        protected string m_Name;

        [SerializeField] protected ValueContainer m_Parent;
        [SerializeField] protected TextMeshProUGUI m_ValueText;
        [SerializeField] protected TextMeshProUGUI m_NameText;

        public override int IncreaseValue(int delta = 1)
        {
            if (m_Parent.Value >= delta)
            {
                return base.IncreaseValue(delta);
            }

            return Value;
        }

        public override int Value
        {
            get => m_Value;
            set
            {
                base.Value = value;
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
    }
}