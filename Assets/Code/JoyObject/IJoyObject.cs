using System;
using System.Collections.Generic;
using JoyLib.Code.Rollers;
using JoyLib.Code.Scripting;
using JoyLib.Code.Unity;
using JoyLib.Code.World;

namespace JoyLib.Code
{
    public interface IJoyObject : ITagged, IPosition, IDerivedValueContainer, IDataContainer
    {
        bool IsDestructible { get; }
        bool IsWall { get; }
        string JoyName { get; }
        long GUID { get; }
        
        IRollable Roller { get; }
        
        IWorldInstance MyWorld { get; set; }
        
        MonoBehaviourHandler MonoBehaviourHandler { get; }
        
        List<IJoyAction> CachedActions { get; }
        
        IEnumerable<Tuple<string, string>> Tooltip { get; }
        
        void Update();

        IJoyAction FetchAction(string name);

        void AttachMonoBehaviourHandler(MonoBehaviourHandler behaviourHandler);
    }
}