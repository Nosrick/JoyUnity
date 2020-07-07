using JoyLib.Code.Entities;
using UnityEngine;

namespace JoyLib.Code.Events
{
    public class EntityCreationListener : JoyEventListener
    {
        private LiveEntityHandler m_EntityHandler;

        public void Awake()
        {
            m_EntityHandler = GameObject.Find("GameManager").GetComponent<LiveEntityHandler>();
        }

        public override void OnEventRaised(params object[] args)
        {
            m_EntityHandler.AddEntity((Entity)args[0]);
        }
    }
}