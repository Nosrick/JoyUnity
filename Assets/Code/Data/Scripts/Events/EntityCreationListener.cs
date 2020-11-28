using JoyLib.Code.Entities;
using UnityEngine;

namespace JoyLib.Code.Events
{
    public class EntityCreationListener : JoyEventListener
    {
        private ILiveEntityHandler m_EntityHandler;

        public void Awake()
        {
            m_EntityHandler = GlobalConstants.GameManager.EntityHandler;
        }

        public override void OnEventRaised(params object[] args)
        {
            m_EntityHandler.AddEntity((Entity)args[0]);
        }
    }
}