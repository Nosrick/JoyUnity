using System.Collections.Generic;
using UnityEngine;

namespace JoyLib.Code.Graphics
{
    public interface ISpriteState
    {
        string Name { get; }
        List<Sprite> SpriteParts { get; }
        List<Color> SpriteColours { get; }
    }
}