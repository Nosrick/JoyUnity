using UnityEngine;

namespace JoyLib.Code.Graphics
{
    public interface IAnimated
    {
        Sprite[] Sprites { get; set; }
        Sprite Sprite { get; }
        int LastIndex { get; }
        int ChosenSprite { get; }
        string TileSet { get; }
        int FramesSinceLastChange { get; }
        bool IsAnimated { get; }
    }
}