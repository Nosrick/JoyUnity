using TMPro;
using UnityEngine;

namespace JoyLib.Code.Unity.GUI
{
    public class ManagedFonts : MonoBehaviour
    {
        protected bool Initialised { get; set; }
        
        protected TextMeshProUGUI[] Texts { get; set; }
        
        public bool HasFont { get; protected set; }
        public bool HasFontColours { get; protected set; }

        public virtual void Awake()
        {
            this.Texts = this.GetComponentsInChildren<TextMeshProUGUI>(true);
            this.Initialised = true;
        }

        public void SetFonts(TMP_FontAsset font)
        {
            if (this.Initialised == false)
            {
                this.Awake();
            }
            foreach (var text in this.Texts)
            {
                text.font = font;
            }

            this.HasFont = true;
        }

        public void SetFontColour(Color colour)
        {
            if (this.Initialised == false)
            {
                this.Awake();
            }
            foreach (var text in this.Texts)
            {
                text.color = colour;
            }

            this.HasFontColours = true;
        }

        public void SetOutline(Color colour, float thickness = 0)
        {
            if (this.Initialised == false)
            {
                this.Awake();
            }
            foreach (var text in this.Texts)
            {
                text.outlineWidth = thickness;
                text.outlineColor = colour;
            }
        }

        public void SetMinMaxFontSizes(float min, float max)
        {
            foreach (var text in this.Texts)
            {
                if (min > 0)
                {
                    text.fontSizeMin = min;
                }

                if (max > 0 && max > min)
                {
                    text.fontSizeMax = max;
                }
            }
        }

        public void SetFontSizes(float size)
        {
            if (this.Initialised == false)
            {
                this.Awake();
            }
            foreach (var text in this.Texts)
            {
                text.fontSize = size;
            }
        }
    }
}