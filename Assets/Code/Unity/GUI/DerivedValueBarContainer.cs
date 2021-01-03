using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JoyLib.Code.Unity.GUI
{
    public class DerivedValueBarContainer : DerivedValueContainer
    {
        [SerializeField] protected Image BarImage;
        [SerializeField] protected TextMeshProUGUI NameText;
        [SerializeField] protected TextMeshProUGUI ValueText;

        [SerializeField] public Color BarColour;
        [SerializeField] public Color TextColour;

        protected void SetFillAmount()
        {
            this.BarImage.color = this.BarColour;
            this.NameText.color = this.TextColour;
            this.ValueText.color = this.TextColour;
            this.BarImage.fillAmount = this.Value / (float)this.Maximum;
        }

        public override int Value
        {
            get => base.Value;
            set
            {
                base.Value = value;
                this.SetFillAmount();
            }
        }

        public override int Maximum
        {
            get => this.m_Maximum;
            set
            {
                base.Maximum = value;
                this.SetFillAmount();
            }
        }
    }
}