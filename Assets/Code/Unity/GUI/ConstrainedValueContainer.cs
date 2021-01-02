using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace JoyLib.Code.Unity.GUI
{
    public class ConstrainedValueContainer : ValueContainer
    {
        [FormerlySerializedAs("Name")] [SerializeField] protected TextMeshProUGUI NameText;
        
        public List<string> Container
        {
            get => this.m_Container;
            set
            {
                this.m_Container = value;
                this.Minimum = 0;
                this.Maximum = this.m_Container.Count() - 1;
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