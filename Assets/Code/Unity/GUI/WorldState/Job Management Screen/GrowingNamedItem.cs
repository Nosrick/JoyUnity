using JoyLib.Code.Events;

namespace JoyLib.Code.Unity.GUI.Job_Management_Screen
{
    public class GrowingNamedItem : NamedItem
    {
        public int IncreaseCost;
        public int DecreaseCost;

        public override int IncreaseValue(int delta = 1)
        {
            if (this.Parent.Value >= this.IncreaseCost)
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
                    Delta = previous > this.m_Value ? this.DecreaseCost : this.IncreaseCost
                });
                GUIManager.CloseGUI(GUINames.TOOLTIP);
                GUIManager.OpenGUI(GUINames.TOOLTIP).GetComponent<Tooltip>().Show(
                    null,
                    this.Tooltip);
            }
        }
        
        public override event ValueChangedEventHandler<int> ValueChanged;
    }
}