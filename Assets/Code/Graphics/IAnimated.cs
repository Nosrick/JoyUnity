using UnityEngine;

namespace JoyLib.Code.Graphics
{
    public interface IAnimated
    {
        Sprite[] Icons { get; }
        Sprite Icon { get; }
        int LastIcon { get; }
        int ChosenIcon { get; }
        string Tileset { get; }
        int FramesSinceLastChange { get; }
        bool IsAnimated { get; }
    }
}