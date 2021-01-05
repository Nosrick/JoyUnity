using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Rollers;
using UnityEngine;

namespace JoyLib.Code.Graphics
{
    public class SpriteState : ISpriteState
    {
        public List<Sprite> SpriteParts => this.m_SpriteParts;
        public List<Color> SpriteColours => this.m_SpriteColours;
        public string Name { get; protected set; }

        protected List<Sprite> m_SpriteParts;
        protected List<Color> m_SpriteColours;

        public SpriteState(
            string name,
            IEnumerable<Sprite> spriteParts,
            IEnumerable<Color> spriteColours,
            bool randomAdditionsIfNotEnoughColours = true)
        {
            this.Name = name;
            this.m_SpriteParts = spriteParts.ToList();
            this.m_SpriteColours = spriteColours.ToList();

            if (randomAdditionsIfNotEnoughColours)
            {
                List<int> results = new List<int>();
                for (int i = this.m_SpriteColours.Count; i < this.m_SpriteParts.Count; i++)
                {
                    results.Add(GlobalConstants.GameManager.Roller.Roll(0, this.m_SpriteColours.Count));
                }

                foreach (int result in results)
                {
                    this.m_SpriteColours.Add(this.m_SpriteColours[result]);
                }
            }

            for (int i = 0; i < this.m_SpriteColours.Count; i++)
            {
                
            }
        }
    }
}