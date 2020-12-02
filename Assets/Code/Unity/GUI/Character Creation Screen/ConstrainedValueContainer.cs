using System;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Unity.GUI;
using TMPro;
using UnityEngine;

namespace JoyLib.Code.Unity.GUI
{
    public class ConstrainedValueContainer : ValueContainer
    {
        [SerializeField] protected TextMeshProUGUI Name;
        
        public List<string> Container
        {
            get => m_Container;
            set
            {
                m_Container = value;
                this.Minimum = 0;
                this.Maximum = m_Container.Count() - 1;
                this.Value = this.Minimum;
            }
        }
        protected List<string> m_Container;

        public string Selected => m_Container[Value];

        public override int Value
        {
            get => m_Value;
            set
            {
                base.Value = value;
                Name.text = Selected;
            }
        }
    }
}