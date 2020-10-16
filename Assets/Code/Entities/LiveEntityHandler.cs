using System.Collections.Generic;
using UnityEngine;
using System;

namespace JoyLib.Code.Entities
{
    public class LiveEntityHandler : MonoBehaviour
    {
        protected Dictionary<long, Entity> m_Entities;
        protected Entity m_Player;

        public bool AddEntity(Entity created)
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

        public void Remove(long GUID)
        {
            if(Entities.ContainsKey(GUID))
            {
                Entities.Remove(GUID);
            }
        }

        public Entity Get(long GUID)
        {
            if(Entities.ContainsKey(GUID))
            {
                return Entities[GUID];
            }
            return null;
        }

        public Entity GetPlayer()
        {
            return m_Player;
        }

        public void SetPlayer(Entity entity)
        {
            m_Player = entity;
        }

        protected Dictionary<long, Entity> Entities
        {
            get
            {
                if(m_Entities is null)
                {
                    m_Entities = new Dictionary<long, Entity>();
                }

                return m_Entities;
            }
        }
    }
}
