﻿using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinSerializer;
using UnityEngine;

namespace JoyLib.Code.Graphics
{
    [Serializable]
    public class SpriteState : ISpriteState
    {
        public SpriteData SpriteData
        {
            get => this.m_SpriteData;
            protected set => this.m_SpriteData = value;
        }

        [SerializeField]
        protected SpriteData m_SpriteData;

        [OdinSerialize]
        public string Name { get; protected set; }

        [OdinSerialize]
        public AnimationType AnimationType { get; protected set; }

        [OdinSerialize]
        public bool Looping { get; protected set; }

        [OdinSerialize]
        public bool IsAnimated { get; set; }

        public SpriteState(
            string name,
            SpriteData spriteData,
            AnimationType animationType = AnimationType.PingPong,
            bool animated = true,
            bool looping = true,
            bool randomiseColours = false)
        {
            this.SpriteData = spriteData;
            
            this.Name = name;

            this.AnimationType = animationType;

            this.IsAnimated = animated;
            this.Looping = looping;

            if (this.SpriteData.m_Parts.Max(part => part.m_Frames) == 1)
            {
                this.IsAnimated = false;
            }
            else
            {
                this.IsAnimated = true;
            }

            if (randomiseColours)
            {
                this.RandomiseColours();
            }
        }
        public List<Tuple<Color, Sprite>> GetSpriteForFrame(int frame)
        {
            int maxFrames = this.SpriteData.m_Parts.Max(part => part.m_Frames);
            if (maxFrames == 0)
            {
                throw new InvalidOperationException("The sprite data for " + this.Name + " is empty!");
            }
            
            if (frame < maxFrames)
            {
                return this.SpriteData.m_Parts.Select(part =>
                    new Tuple<Color, Sprite>(
                        part.m_PossibleColours[part.m_SelectedColour],
                        part.m_FrameSprites[frame])).ToList();
            }

            return this.GetSpriteForFrame(0);
        }

        public void OverrideColours(IDictionary<string, Color> colours)
        {
            for(int i = 0; i < this.SpriteData.m_Parts.Count; i++)
            {
                SpritePart part = this.SpriteData.m_Parts[i];
                if (!colours.ContainsKey(part.m_Name))
                {
                    continue;
                }
                part.m_PossibleColours = new List<Color>
                {
                    colours[part.m_Name]
                };
                part.m_SelectedColour = 0;
                this.SpriteData.m_Parts[i] = part;
            }
        }

        public void OverrideWithSingleColour(Color colour)
        {
            for (int i = 0; i < this.SpriteData.m_Parts.Count; i++)
            {
                SpritePart part = this.SpriteData.m_Parts[i];
                part.m_PossibleColours = new List<Color>
                {
                    colour
                };
                part.m_SelectedColour = 0;
                this.SpriteData.m_Parts[i] = part;
            }
        }

        public List<int> GetIndices()
        {
            return this.SpriteData.m_Parts.Select(part => part.m_SelectedColour).ToList();
        }
        
        public void RandomiseColours()
        {
            for(int i = 0; i < this.SpriteData.m_Parts.Count; i++)
            {
                SpritePart part = this.SpriteData.m_Parts[i];
                part.m_SelectedColour = GlobalConstants.GameManager.Roller.Roll(0, part.m_PossibleColours.Count);
                this.SpriteData.m_Parts[i] = part;
            }
        }

        public void SetColourIndices(List<int> indices)
        {
            for (int i = 0; i < indices.Count; i++)
            {
                for (int j = 0; j < this.SpriteData.m_Parts.Count; j++)
                {
                    SpritePart part = this.SpriteData.m_Parts[j];
                    part.m_SelectedColour = indices[i];
                    this.SpriteData.m_Parts[j] = part;
                }
            }
        }
    }
}