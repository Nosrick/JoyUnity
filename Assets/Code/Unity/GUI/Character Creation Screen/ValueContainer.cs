using System;
using JoyLib.Code.Unity.GUI;
using UnityEngine;

namespace JoyLib.Code.Unity.GUI
{
    public class ValueContainer : MonoBehaviour, IValueContainer
    {
        [SerializeField] public int Minimum = 1;
        [SerializeField] public int Maximum = 10;
        
        public virtual int DecreaseValue(int value = 1)
        {
            if (Value - value >= Minimum)
            {
                Value -= value;
            }
            return Value;
        }

        public virtual int IncreaseValue(int value = 1)
        {
            if (Value + value <= Maximum)
            {
                Value += value;
            }
            return Value;
        }

        public virtual int Value
        {
            get => m_Value;
            set
            {
                m_Value = value;
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        protected int m_Value;
        
        public event EventHandler ValueChanged;
    }
}