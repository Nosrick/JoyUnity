using System.Collections.Generic;
using UnityEngine;

namespace JoyLib.Code.Unity.GUI
{
    public class GUIManager
    {
        private List<GameObject> m_GUIs;
        private GameObject m_ActiveGUI;

        public GUIManager()
        {
            m_GUIs = new List<GameObject>();
            m_ActiveGUI = null;
        }

        public void AddGUI(GameObject gui)
        {
            gui.SetActive(false);
            m_GUIs.Add(gui);
        }

        public void OpenGUI(string name)
        {
            foreach(GameObject gui in m_GUIs)
            {
                if(gui.name == name)
                {
                    CloseGUI();
                    m_ActiveGUI = gui;
                    m_ActiveGUI.SetActive(true);
                    break;
                }
            }
        }

        public void CloseGUI()
        {
            if(m_ActiveGUI != null)
            {
                m_ActiveGUI.SetActive(false);
                m_ActiveGUI = null;
            }
        }
    }
}
