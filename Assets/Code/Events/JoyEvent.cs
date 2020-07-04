using System.Collections.Generic;

namespace JoyLib.Code.Events
{
    public class JoyEvent
    {
        protected List<JoyEventListener> m_Listeners = new List<JoyEventListener>();

        public void Raise()
        {
            for (int i = m_Listeners.Count - 1; i >= 0; i--)
            {
                m_Listeners[i].OnEventRaised();
            }
        }

        public void RegisterListener(JoyEventListener listener) // 7
        {
            m_Listeners.Add(listener);
        }

        public void UnregisterListener(JoyEventListener listener) // 8
        {
            m_Listeners.Remove(listener);
        }
    }
}