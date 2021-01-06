using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JoyLib.Code.Graphics
{
    public class SpriteState : ISpriteState
    {
        public SpriteData SpriteData { get; protected set; }
        public string Name { get; protected set; }

        public SpriteState(
            string name,
            SpriteData spriteData,
            IDictionary<string, Color> spriteColours,
            bool randomAdditionsIfNotEnoughColours = true)
        {
            this.SpriteData = spriteData;
            
            this.Name = name;

            if (this.SpriteData.m_Parts.All(part => spriteColours.ContainsKey(part.m_Name)))
            {
                return;
            }
            List<Color> colours = spriteColours.Values.ToList();
            for(int i = 0; i < this.SpriteData.m_Parts.Count; i++)
            {
                SpritePart part = this.SpriteData.m_Parts[i];
                if (spriteColours.ContainsKey(part.m_Name))
                {
                    continue;
                }
                part.m_Colour = randomAdditionsIfNotEnoughColours 
                    ? colours[GlobalConstants.GameManager.Roller.Roll(0, colours.Count)] 
                    : Color.magenta;
                this.SpriteData.m_Parts[i] = part;
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
                        part.m_Colour,
                        part.m_FrameSprites[frame])).ToList();
            }

            return this.GetSpriteForFrame(0);
        }

        public static ISpriteState MakeWithDefaultColour(string name, SpriteData data)
        {
            IDictionary<string, Color> colours = (from d in data.m_Parts
                    select new KeyValuePair<string, Color>(
                        d.m_Name,
                        Color.white))
                .ToDictionary(x => x.Key, x => x.Value);

            return new SpriteState(
                name,
                data,
                colours);
        }
    }
}