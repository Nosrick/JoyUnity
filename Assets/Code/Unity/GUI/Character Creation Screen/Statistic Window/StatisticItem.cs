using System;
using System.Globalization;
using JoyLib.Code.Unity.GUI;
using TMPro;
using UnityEngine;

namespace JoyLib.Code.Unity.GUI
{
    public class StatisticItem : ValueContainer
    {
        protected string m_Name;
        [SerializeField] protected StatisticWindow Parent;

        [SerializeField] protected TextMeshProUGUI m_ValueText;
        [SerializeField] protected TextMeshProUGUI m_NameText;

        public override int DecreaseValue(int value = 1)
        {
            if (Value - value >= Minimum)
            {
                Parent.PointsRemaining += value;
                Value -= value;
            }
            return Value;
        }

        public override int IncreaseValue(int value = 1)
        {
            if(Parent.PointsRemaining >= value && Value + value <= Maximum)
            {
                Parent.PointsRemaining -= value;
                Value += value;
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