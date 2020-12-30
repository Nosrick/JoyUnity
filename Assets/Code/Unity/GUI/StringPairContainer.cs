using System;
using Castle.Core.Internal;
using TMPro;
using UnityEngine;

namespace JoyLib.Code.Unity.GUI
{
    [RequireComponent(typeof(RectTransform))]
    public class StringPairContainer : MonoBehaviour
    {
        [SerializeField] protected TextMeshProUGUI m_Key;
        [SerializeField] protected TextMeshProUGUI m_Value;
        
        protected Tuple<string, string> m_Target;

        public void OnEnable()
        {
        }

        public Tuple<string, string> Target
        {
            get => this.m_Target;
            set
            {
                this.m_Target = value;
                this.Repaint();
            }
        }

        protected virtual void Repaint()
        {
            this.m_Key.text = this.Target.Item1;
            this.m_Value.text = this.Target.Item2;

            this.m_Key.gameObject.SetActive(!this.m_Key.text.IsNullOrEmpty());
            this.m_Value.gameObject.SetActive(!this.m_Value.text.IsNullOrEmpty());

            this.m_Value.alignment = this.Target.Item1.IsNullOrEmpty()
                ? TextAlignmentOptions.Center
                : TextAlignmentOptions.Left;

            this.m_Key.alignment = this.Target.Item2.IsNullOrEmpty()
                ? TextAlignmentOptions.Center
                : TextAlignmentOptions.Right;
        }
    }
}