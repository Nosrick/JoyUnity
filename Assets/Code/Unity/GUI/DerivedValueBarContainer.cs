﻿using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JoyLib.Code.Unity.GUI
{
    public class DerivedValueBarContainer : DerivedValueContainer
    {
        [SerializeField] protected Image BarImage;
        [SerializeField] protected TextMeshProUGUI NameText;
        [SerializeField] protected TextMeshProUGUI ValueText;

        [SerializeField] public Color BackgroundColour;
        [SerializeField] public Color TextColour;

        public override int DecreaseValue(int delta = 1)
        {
            if (this.m_Parent is null)
            {
                if (this.Value - delta >= this.Minimum)
                {
                    this.Value -= delta;
                }
                return this.Value;
            }

            return base.DecreaseValue(delta);
        }

        public override int IncreaseValue(int delta = 1)
        {
            if (this.m_Parent is null)
            {
                if (this.Value + delta <= this.Maximum)
                {
                    this.Value += delta;
                }

                return this.Value;
            }
            
            return base.IncreaseValue(delta);
        }

        protected void SetFillAmount()
        {
            this.BarImage.color = this.BackgroundColour;
            this.NameText.color = this.TextColour;
            this.ValueText.color = this.TextColour;
            this.BarImage.fillAmount = (float)this.Value / (float)this.Maximum;
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