namespace JoyLib.Code.Graphics
{
    public interface IAnimated : ISpriteStateContainer
    {
        ISpriteState CurrentSpriteState { get; }
        int FrameIndex { get; }
        string ChosenSpriteState { get; }
        string TileSet { get; }
        float TimeSinceLastChange { get; }
        bool IsAnimated { get; }
    }
}