using UnityEngine;
using UnityEngine.Events;

namespace JoyLib.Code.Events
{
    public class JoyEventListener : MonoBehaviour
    {
        [SerializeField]
        protected JoyEvent m_JoyEvent;
        [SerializeField]
        protected UnityEvent m_Response;

        private void OnEnable()
        {
            m_JoyEvent.RegisterListener(this);
        }

        private void OnDisable()
        {
            m_JoyEvent.UnregisterListener(this);
        }

        public virtual void OnEventRaised()
        {
            m_Response.Invoke();
        }
    }
}