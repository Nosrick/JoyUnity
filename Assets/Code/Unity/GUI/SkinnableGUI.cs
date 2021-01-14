using System.Collections.Generic;
using JoyLib.Code.Graphics;
using TMPro;
using UnityEngine;

namespace JoyLib.Code.Unity.GUI
{
    public class SkinnableGUI : GUIData
    {
        [SerializeField] protected ManagedUISprite m_Background;
        
        public bool HasBackground { get; protected set; }
        public bool HasColours { get; protected set; }
        
        public bool HasFont { get; protected set; }

        public override void Awake()
        {
            base.Awake();
            this.m_Background.Awake();
        }

        public void SetBackground(ISpriteState sprite)
        {
            this.m_Background.Clear();
            this.m_Background.AddSpriteState(sprite);
            this.HasBackground = true;
        }

        public void SetColours(IDictionary<string, Color> colours)
        {
            if (this.m_Background.CurrentSpriteState is null)
            {
                GlobalConstants.ActionLog.AddText("Trying to set colours of a null sprite state. " + this.name);
                //GlobalConstants.ActionLog.AddText();
                return;
            }

            this.m_Background.OverrideAllColours(colours);
            this.HasColours = true;
        }

        public void SetFont(TMP_FontAsset font)
        {
            TextMeshProUGUI[] texts = this.GetComponentsInChildren<TextMeshProUGUI>(true);
            foreach (TextMeshProUGUI text in texts)
            {
                text.font = font;
            }

            this.HasFont = true;
        }
    }
}