using System.Globalization;
using JoyLib.Code.Events;
using TMPro;
using UnityEngine;

namespace JoyLib.Code.Unity.GUI
{
    public class NamedItem : ValueContainer
    {
        protected string m_Name;

        public ValueContainer Parent { get; set; }
        [SerializeField] protected TextMeshProUGUI m_ValueText;
        [SerializeField] protected TextMeshProUGUI m_NameText;

        public override int IncreaseValue(int delta = 1)
        {
            if (this.Parent.Value >= delta)
            {
                return base.IncreaseValue(delta);
            }

            return this.Value;
        }

        public override int Value
        {
            get => this.m_Value;
            set
            {
                int previous = this.m_Value;
                this.m_Value = value;
                this.m_ValueText.text = this.m_Value.ToString();
                this.ValueChanged?.Invoke(this, new ValueChangedEventArgs<int>
                {
                    Name = this.Name,
                    NewValue = this.m_Value,
                    Delta = this.m_Value - previous
                });
            }
        }

        public override string Name
        {
            get => this.m_Name;
            set
            {
                this.m_Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value);
                this.gameObject.name = this.m_Name;
                this.m_NameText.text = this.m_Name;
            }
        }
        
        public override event ValueChangedEventHandler<int> ValueChanged;
    }
}