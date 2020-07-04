using JoyLib.Code.Entities;
using UnityEngine;

namespace JoyLib.Code.Events
{
    public class EntityCreationListener : JoyEventListener
    {
        private LiveEntityHandler m_EntityHandler;

        public void Start()
        {
            m_EntityHandler = GameObject.Find("WorldEntities").GetComponent<LiveEntityHandler>();
        }

        public override void OnEventRaised()
        {
            if(!(m_JoyEvent is EntityCreatedEvent createdEvent))
            {
                return;
            }

            m_EntityHandler.AddEntity(createdEvent.Created);
        }
    }
}