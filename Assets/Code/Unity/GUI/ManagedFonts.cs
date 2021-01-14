using TMPro;
using UnityEngine;

namespace JoyLib.Code.Unity.GUI
{
    public class ManagedFonts : MonoBehaviour
    {
        protected TextMeshProUGUI[] Texts { get; set; }
        
        public bool HasFont { get; protected set; }

        public void Awake()
        {
            this.Texts = this.GetComponentsInChildren<TextMeshProUGUI>(true);
        }

        public void SetFonts(TMP_FontAsset font)
        {
            foreach (var text in this.Texts)
            {
                text.font = font;
            }

            this.HasFont = true;
        }

        public void SetColour(Color colour)
        {
            foreach (var text in this.Texts)
            {
                text.color = colour;
            }
        }

        public void SetOutline(Color colour, float thickness = 0)
        {
            foreach (var text in this.Texts)
            {
                text.outlineWidth = thickness;
                text.outlineColor = colour;
            }
        }

        public void SetFontSizes(float size, float min = 0, float max = 0)
        {
            foreach (var text in this.Texts)
            {
                text.fontSize = size;
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
    }
}