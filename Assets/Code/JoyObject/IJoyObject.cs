using System.Collections.Generic;
using JoyLib.Code.Graphics;
using JoyLib.Code.Scripting;
using JoyLib.Code.World;

namespace JoyLib.Code
{
    public interface IJoyObject : ITagged, IPosition, IAnimated, IDerivedValueContainer, IDataContainer
    {
        bool IsDestructible { get; }
        bool IsWall { get; }
        string JoyName { get; }
        long GUID { get; }
        
        WorldInstance MyWorld { get; set; }
        
        List<IJoyAction> CachedActions { get; }
        
        void Update();

        IJoyAction FetchAction(string name);
    }
}