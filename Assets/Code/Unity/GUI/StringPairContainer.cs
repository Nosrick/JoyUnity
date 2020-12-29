using System;
using TMPro;
using UnityEngine;

namespace JoyLib.Code.Unity.GUI
{
    public class StringPairContainer : MonoBehaviour
    {
        [SerializeField] protected TextMeshProUGUI m_Key;
        [SerializeField] protected TextMeshProUGUI m_Value;

        private Tuple<string, string> m_Target;

        public Tuple<string, string> Target
        {
            get => this.m_Target;
            set
            {
                this.m_Target = value;
                this.Repaint();
            }
        }

        public virtual void Repaint()
        {
            this.m_Key.text = this.Target.Item1;
            this.m_Value.text = this.Target.Item2;
        }
    }
}