using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Graphics;
using UnityEngine;
using UnityEngine.UI;

namespace JoyLib.Code.Unity.GUI
{
    public class ManagedAccent : ManagedFonts
    {
        public bool Initialised { get; protected set; }
        public bool HasBackgroundColours { get; protected set; }
        public bool HasBackgroundImage { get; protected set; }
        
        protected Image[] Images { get; set; }
        protected ManagedUISprite[] ManagedUISprites { get; set; }

        public override void Awake()
        {
            if (this.Initialised)
            {
                return;
            }
            this.Images = this.GetComponentsInChildren<Image>(true);
            this.ManagedUISprites = this.GetComponentsInChildren<ManagedUISprite>(true);
            base.Awake();
        }

        public void SetBackgrounds(ISpriteState state)
        {
            foreach(Image image in this.Images)
            {
                image.sprite = state.SpriteData.m_Parts.First().m_FrameSprites.First();
            }
            foreach (ManagedUISprite sprite in this.ManagedUISprites)
            {
                sprite.Clear();
                sprite.AddSpriteState(state);
            }
            this.HasBackgroundImage = true;
        }

        public void SetBackgroundColours(IDictionary<string, Color> colours)
        {
            foreach(Image image in this.Images)
            {
                image.color = colours.First().Value;
            }
            foreach (ManagedUISprite sprite in this.ManagedUISprites)
            {
                sprite.OverrideAllColours(colours);
            }
            this.HasBackgroundColours = true;
        }
    }
}