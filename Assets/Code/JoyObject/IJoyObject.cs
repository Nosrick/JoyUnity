using JoyLib.Code.Graphics;
using JoyLib.Code.World;

namespace JoyLib.Code
{
    public interface IJoyObject : ITagged, IPosition, IAnimated, IDerivedValueContainer
    {
        bool IsDestructible { get; }
        bool IsWall { get; }
        string JoyName { get; }
        long GUID { get; }
        
        WorldInstance MyWorld { get; }
        
        void Update();
    }
}