﻿using System.Collections.Generic;
using JoyLib.Code.Graphics;
using UnityEngine;

namespace JoyLib.Code.Unity.GUI
{
    public class SkinnableGUI : GUIData
    {
        [SerializeField] protected ManagedUISprite m_Background;

        public override void Awake()
        {
            base.Awake();
            this.m_Background.Awake();
        }

        public void SetBackground(ISpriteState sprite)
        {
            this.m_Background.Clear();
            this.m_Background.AddSpriteState(sprite);
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
        }
    }
}