using System.Collections.Generic;
using UnityEngine;
using System;

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
                Entities.Add(created.GUID, created);

                if(created.PlayerControlled)
                {
                    m_Player = created;
                }

                return true;
            }
            catch(Exception e)
            {
                Debug.Log(e.Message);
                Debug.Log(e.StackTrace);
                return false;
            }
        }

        public bool Remove(long GUID)
        {
            if(Entities.ContainsKey(GUID))
            {
                return Entities.Remove(GUID);
            }

            return false;
        }

        public IEntity Get(long GUID)
        {
            if(Entities.ContainsKey(GUID))
            {
                return Entities[GUID];
            }
            return null;
        }

        public IEntity GetPlayer()
        {
            return m_Player;
        }

        public void SetPlayer(IEntity entity)
        {
            m_Player = entity;
            GlobalConstants.GameManager.Player = m_Player;
        }

        protected Dictionary<long, IEntity> Entities
        {
            get
            {
                if(m_Entities is null)
                {
                    m_Entities = new Dictionary<long, IEntity>();
                }

                return m_Entities;
            }
        }
    }
}
