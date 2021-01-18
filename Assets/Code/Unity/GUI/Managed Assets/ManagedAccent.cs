using System;
using System.Collections.Generic;
using JoyLib.Code.Graphics;
using JoyLib.Code.Unity.GUI.Managed_Assets;
using UnityEngine;

namespace JoyLib.Code.Unity.GUI
{

    public class ManagedAccent : ManagedElement
    {
        public bool HasBackgroundColours { get; protected set; }
        public bool HasBackgroundImage { get; protected set; }
        
        protected ManagedUISprite[] ManagedUISprites { get; set; }

        public void Awake()
        {
            this.Initialise();
        }

        protected void Initialise()
        {
            if (this.Initialised)
            {
                return;
            }
            this.ManagedUISprites = this.GetComponentsInChildren<ManagedUISprite>(true);
            this.Initialised = true;
        }

        public void SetBackgrounds(ISpriteState state)
        {
            this.Initialise();

            if (state.Name.Equals(this.ElementName, StringComparison.OrdinalIgnoreCase) == false)
            {
                return;
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
            this.Initialise();
            foreach (ManagedUISprite sprite in this.ManagedUISprites)
            {
                sprite.OverrideAllColours(colours, true);
            }
            this.HasBackgroundColours = true;
        }
    }
}