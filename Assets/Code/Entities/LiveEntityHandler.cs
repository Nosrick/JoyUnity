using System;
using System.Collections.Generic;
using System.Linq;

namespace JoyLib.Code.Entities
{
    public class LiveEntityHandler : ILiveEntityHandler
    {
        protected Dictionary<Guid, IEntity> m_Entities;
        protected IEntity m_Player;

        public IEnumerable<IEntity> AllEntities => this.m_Entities.Values.ToList();

        public bool AddEntity(IEntity created)
        {
            try
            {
                if (this.Entities.ContainsKey(created.Guid))
                {
                    return false;
                }
                
                this.Entities.Add(created.Guid, created);

                if(created.PlayerControlled)
                {
                    this.m_Player = created;
                }

                return true;
            }
            catch(Exception e)
            {
                GlobalConstants.ActionLog.StackTrace(e);
                return false;
            }
        }

        public bool Remove(Guid GUID)
        {
            if(this.Entities.ContainsKey(GUID))
            {
                return this.Entities.Remove(GUID);
            }

            return false;
        }

        public IEntity Get(Guid GUID)
        {
            if(this.Entities.ContainsKey(GUID))
            {
                return this.Entities[GUID];
            }
            return null;
        }

        public IEntity GetPlayer()
        {
            return this.m_Player;
        }

        public void SetPlayer(IEntity entity)
        {
            this.m_Player = entity;
        }

        public void ClearLiveEntities()
        {
            this.m_Entities = new Dictionary<Guid, IEntity>();
        }

        protected Dictionary<Guid, IEntity> Entities
        {
            get
            {
                if(this.m_Entities is null)
                {
                    this.m_Entities = new Dictionary<Guid, IEntity>();
                }

                return this.m_Entities;
            }
        }
    }
}
