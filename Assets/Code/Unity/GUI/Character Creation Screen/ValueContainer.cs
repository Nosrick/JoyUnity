﻿using JoyLib.Code.Events;
using UnityEngine;

namespace JoyLib.Code.Unity.GUI
{
    public class ValueContainer : MonoBehaviour, IValueContainer
    {
        [SerializeField] public int Minimum = 1;
        [SerializeField] public int Maximum = 10;
        [SerializeField] public int Delta = 1;
        
        public virtual int DecreaseValue(int delta = 1)
        {
            if (Value - delta >= Minimum)
            {
                Value -= delta;
            }
            return Value;
        }

        public virtual int IncreaseValue(int delta = 1)
        {
            if (Value + delta <= Maximum)
            {
                Value += delta;
            }
            return Value;
        }

        public virtual int DirectValueSet(int newValue)
        {
            m_Value = newValue;
            Value = m_Value;
            return m_Value;
        }

        public virtual int Value
        {
            get => m_Value;
            set
            {
                int previous = m_Value;
                m_Value = value;
                ValueChanged?.Invoke(this, new ValueChangedEventArgs()
                {
                    NewValue = m_Value,
                    Delta = m_Value - previous
                });
            }
        }

        protected int m_Value;
        
        public event ValueChangedEventHandler ValueChanged;
    }
}