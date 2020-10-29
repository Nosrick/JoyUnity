using JoyLib.Code.Graphics;

namespace JoyLib.Code
{
    public interface IJoyObject : ITagged, IPosition, IAnimated, IDerivedValueContainer
    {
        bool IsDestructible { get; }
        bool IsWall { get; }
        string JoyName { get; }
        long GUID { get; }
        
        void Update();
    }
}