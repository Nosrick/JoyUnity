using JoyLib.Code.Events;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JoyLib.Code.Unity.GUI
{
    public class ValueContainer : MonoBehaviour, IValueContainer
    {
        [SerializeField] protected int m_Minimum = 1;
        [SerializeField] protected int m_Maximum = 10;
        [SerializeField] public int DecreaseDelta = 1;
        [SerializeField] public int IncreaseDelta = 1;
        [SerializeField] public bool AllowIncrease = true;
        [SerializeField] public bool AllowDecrease = true;

        protected int m_Value;

        protected string m_Tooltip;
        
        protected IGUIManager GUIManager { get; set; }

        public virtual void OnEnable()
        {
            if (this.GUIManager is null && GlobalConstants.GameManager is null == false)
            {
                this.GUIManager = GlobalConstants.GameManager.GUIManager;
            }
        }

        public virtual int Maximum
        {
            get => this.m_Maximum;
            set => this.m_Maximum = value;
        }

        public virtual int Minimum
        {
            get => this.m_Minimum;
            set => this.m_Minimum = value;
        }

        public virtual string Tooltip
        {
            get => this.m_Tooltip;
            set
            {
                this.m_Tooltip = value;
                if (this.GUIManager.IsActive(GUINames.TOOLTIP))
                {
                    this.GUIManager.CloseGUI(GUINames.TOOLTIP);
                    this.GUIManager.OpenGUI(GUINames.TOOLTIP).GetComponent<Tooltip>().Show(null, this.Tooltip);
                }
            }
        }

        public virtual string Name
        {
            get;
            set;
        }

        protected virtual void OnChangeValue(object sender, ValueChangedEventArgs args)
        {
            this.ValueChanged?.Invoke(sender, args);
        }

        public void OnPointerEnter(PointerEventData data)
        {
            if (this.Tooltip is null)
            {
                return;
            }

            Tooltip tooltip = this.GUIManager.OpenGUI(GUINames.TOOLTIP).GetComponent<Tooltip>();
            tooltip.Show(null, this.Tooltip);
        }

        public void OnPointerExit(PointerEventData data)
        {
            this.GUIManager.CloseGUI(GUINames.TOOLTIP);
        }

        public virtual void DecreaseValueButtonPress()
        {
            if (this.AllowDecrease == false)
            {
                return;
            }
            this.DecreaseValue(this.DecreaseDelta);
        }

        public virtual void IncreaseValueButtonPress()
        {
            if (this.AllowIncrease == false)
            {
                return;
            }
            this.IncreaseValue(this.IncreaseDelta);
        }

        public virtual int DecreaseValue(int delta = 1)
        {
            if (this.Value - delta >= this.Minimum)
            {
                this.Value -= delta;
            }
            return this.Value;
        }

        public virtual int IncreaseValue(int delta = 1)
        {
            if (this.Value + delta <= this.Maximum)
            {
                this.Value += delta;
            }
            return this.Value;
        }

        public virtual int DirectValueSet(int newValue)
        {
            this.m_Value = newValue;
            this.Value = this.m_Value;
            return this.m_Value;
        }

        public virtual int Value
        {
            get => this.m_Value;
            set
            {
                int previous = this.m_Value;
                this.m_Value = value;
                this.ValueChanged?.Invoke(this, new ValueChangedEventArgs()
                {
                    NewValue = this.m_Value,
                    Delta = this.m_Value - previous
                });
            }
        }
        
        public virtual event ValueChangedEventHandler ValueChanged;
    }
}