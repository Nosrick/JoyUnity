using System.Collections.Generic;
using JoyLib.Code.Collections;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Graphics;
using JoyLib.Code.Scripting;
using JoyLib.Code.Unity;
using JoyLib.Code.World;

namespace JoyLib.Code
{
    public interface IJoyObject : ITagged, IPosition, IAnimated, IDerivedValueContainer, IDataContainer
    {
        BasicValueContainer<IDerivedValue> DerivedValues { get; }
        
        bool IsDestructible { get; }
        bool IsWall { get; }
        string JoyName { get; }
        long GUID { get; }
        
        WorldInstance MyWorld { get; set; }
        
        MonoBehaviourHandler MonoBehaviourHandler { get; }
        
        List<IJoyAction> CachedActions { get; }
        
        void Update();

        IJoyAction FetchAction(string name);

        void AttachMonoBehaviourHandler(MonoBehaviourHandler behaviourHandler);
    }
}