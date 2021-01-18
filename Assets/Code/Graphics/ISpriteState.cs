using System;
using System.Collections.Generic;
using UnityEngine;

namespace JoyLib.Code.Graphics
{
    public interface ISpriteState
    {
        string Name { get; }

        List<Tuple<Color, Sprite>> GetSpriteForFrame(int frame);
        
        SpriteData SpriteData { get; }

        void RandomiseColours();
        void SetColourIndices(List<int> indices);
        void OverrideColours(IDictionary<string, Color> colours);
        void OverrideWithSingleColour(Color colour);

        List<int> GetIndices();
    }
}