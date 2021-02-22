using System;
using System.Collections.Generic;

namespace JoyLib.Code.Entities
{
    public interface ILiveEntityHandler : IHandler<IEntity, Guid>
    {
        IEnumerable<IEntity> Values { get; }
        
        bool AddEntity(IEntity created);

        bool Remove(Guid GUID);

        IEntity GetPlayer();

        void SetPlayer(IEntity entity);

        void ClearLiveEntities();
    }
}