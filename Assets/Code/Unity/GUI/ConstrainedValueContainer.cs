using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace JoyLib.Code.Unity.GUI
{
    public class ConstrainedValueContainer : ValueContainer
    {
        [FormerlySerializedAs("Name")] 
        [SerializeField] protected TextMeshProUGUI NameText;

        [SerializeField] protected bool m_WrapAround;

        public override int DecreaseValue(int delta = 1)
        {
            if (this.Value - delta >= this.Minimum)
            {
                this.Value -= delta;
            }
            else if (this.Value - delta < this.Minimum && this.m_WrapAround)
            {
                this.Value = this.Maximum - 1;
            }
            return this.Value;
        }

        public override int IncreaseValue(int delta = 1)
        {
            if (this.Value + delta < this.Maximum)
            {
                this.Value += delta;
            }
            else if (this.Value + delta >= this.Maximum && this.m_WrapAround)
            {
                this.Value = Math.Abs((this.Value + delta) % this.Maximum);
            }
            return this.Value;
        }
        
        public List<string> Container
        {
            get => this.m_Container;
            set
            {
                this.m_Container = value;
                this.Minimum = 0;
                this.Maximum = this.m_Container.Count;
                this.Value = this.Minimum;
            }
        }
        protected List<string> m_Container;

        public string Selected => this.m_Container[this.Value];

        public override int Value
        {
            get => this.m_Value;
            set
            {
                base.Value = value;
                this.NameText.text = this.Selected;
            }
        }
    }
}