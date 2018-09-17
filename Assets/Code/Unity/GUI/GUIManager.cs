using System.Collections.Generic;
using UnityEngine;

namespace JoyLib.Code.Unity.GUI
{
    public class GUIManager
    {
        private List<Tuple<GameObject, bool>> m_GUIs;
        private Tuple<GameObject, bool> m_ActiveGUI;

        public GUIManager()
        {
            m_GUIs = new List<Tuple<GameObject, bool>>();
            m_ActiveGUI = null;
        }

        public void AddGUI(GameObject gui, bool removesControl = true)
        {
            gui.SetActive(false);
            m_GUIs.Add(new Tuple<GameObject, bool>(gui, removesControl));
        }

        public void OpenGUI(string name)
        {
            foreach(Tuple<GameObject, bool> gui in m_GUIs)
            {
                if(gui.First.name == name)
                {
                    CloseGUI();
                    m_ActiveGUI = gui;
                    m_ActiveGUI.First.SetActive(true);
                    break;
                }
            }
        }

        public void CloseGUI()
        {
            if(m_ActiveGUI != null)
            {
                m_ActiveGUI.First.SetActive(false);
                m_ActiveGUI = null;
            }
        }

        public bool RemovesControl()
        {
            return m_ActiveGUI.Second;
        }
    }
}
