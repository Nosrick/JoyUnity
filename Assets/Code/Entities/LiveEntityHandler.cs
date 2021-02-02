using System;
using System.Collections.Generic;

namespace JoyLib.Code.Entities
{
    public class LiveEntityHandler : ILiveEntityHandler
    {
        protected Dictionary<long, IEntity> m_Entities;
        protected IEntity m_Player;

        public bool AddEntity(IEntity created)
        {
            try
            {
                this.Entities.Add(created.GUID, created);

                if(created.PlayerControlled)
                {
                    this.m_Player = created;
                }

                return true;
            }
            catch(Exception e)
            {
                GlobalConstants.ActionLog.AddText(e.Message);
                GlobalConstants.ActionLog.AddText(e.StackTrace);
                return false;
            }
        }

        public bool Remove(long GUID)
        {
            if(this.Entities.ContainsKey(GUID))
            {
                return this.Entities.Remove(GUID);
            }

            return false;
        }

        public IEntity Get(long GUID)
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

        protected Dictionary<long, IEntity> Entities
        {
            get
            {
                if(this.m_Entities is null)
                {
                    this.m_Entities = new Dictionary<long, IEntity>();
                }

                return this.m_Entities;
            }
        }
    }
}
