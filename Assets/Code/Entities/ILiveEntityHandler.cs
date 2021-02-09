using System;
using System.Collections.Generic;

namespace JoyLib.Code.Entities
{
    public interface ILiveEntityHandler
    {
        IEnumerable<IEntity> AllEntities { get; }
        
        bool AddEntity(IEntity created);

        bool Remove(Guid GUID);

        IEntity Get(Guid GUID);

        IEntity GetPlayer();

        void SetPlayer(IEntity entity);
        
        
    }
}