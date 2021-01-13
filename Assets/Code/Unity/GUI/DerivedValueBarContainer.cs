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
        [SerializeField] public Color OutlineColour;

        [SerializeField] protected bool InvertWhenEmptying = true;
        
        protected RectTransform ValueRect { get; set; }
        protected RectTransform NameRect { get; set; }
        
        protected float ValueMid { get; set; }
        protected float NameMid { get; set; }

        public void Initialise()
        {
            this.ValueRect = this.ValueText.GetComponent<RectTransform>();
            this.NameRect = this.NameText.GetComponent<RectTransform>();

            this.ValueMid = (this.ValueRect.anchorMin.x + this.ValueRect.anchorMax.x) / 2;
            this.NameMid = (this.NameRect.anchorMin.x + this.NameRect.anchorMax.x) / 2;

            //this.NameText.outlineWidth = 0.05f;
            //this.ValueText.outlineWidth = 0.05f;
        }

        protected void SetFillAmount()
        {
            this.BarImage.fillAmount = this.Value / (float)this.Maximum;
            this.BarImage.color = this.BarColour;

            if (this.BarImage.fillAmount < this.ValueMid && this.InvertWhenEmptying)
            {
                this.ValueText.color = this.OutlineColour;
                this.ValueText.outlineColor = this.TextColour;
            }
            else
            {
                this.ValueText.color = this.TextColour;
                this.ValueText.outlineColor = this.OutlineColour;
            }

            if (this.BarImage.fillAmount < this.NameMid && this.InvertWhenEmptying)
            {
                this.NameText.color = this.OutlineColour;
                this.NameText.outlineColor = this.TextColour;
            }
            else
            {
                this.NameText.color = this.TextColour;
                this.NameText.outlineColor = this.OutlineColour;
            }
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