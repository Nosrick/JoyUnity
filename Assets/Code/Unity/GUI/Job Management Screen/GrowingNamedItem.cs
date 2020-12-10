using JoyLib.Code.Events;

namespace JoyLib.Code.Unity.GUI.Job_Management_Screen
{
    public class GrowingNamedItem : NamedItem
    {
        public int IncreaseCost;
        public int DecreaseCost;

        public override int IncreaseValue(int delta = 1)
        {
            if (m_Parent.Value >= IncreaseCost)
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
                int previous = m_Value;
                m_Value = value;
                m_ValueText.text = m_Value.ToString();
                ValueChanged?.Invoke(this, new ValueChangedEventArgs()
                {
                    Name = this.Name,
                    NewValue = m_Value,
                    Delta = previous > m_Value ? DecreaseCost : IncreaseCost
                });
            }
        }
        
        public virtual event ValueChangedEventHandler ValueChanged;
    }
}