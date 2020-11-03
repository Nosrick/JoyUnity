using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JoyLib.Code.Unity.GUI
{
    public class GUIManager : MonoBehaviour
    {
        private List<GUIData> m_GUIs;
        private List<GUIData> m_ActiveGUIs;

        public void Awake()
        {
            Initialise();
        }

        public void OnGUI()
        {
            for (int i = 0; i < m_ActiveGUIs.Count; i++)
            {
                UnityEngine.GUI.Window(
                    i,
                    m_ActiveGUIs[i].GetComponent<RectTransform>().rect,
                    Empty,
                    m_ActiveGUIs[i].name);
            }
        }

        protected void Empty(int index)
        {
            
        }

        protected void Initialise()
        {
            if (m_GUIs is null)
            {
                m_GUIs = new List<GUIData>();
                m_ActiveGUIs = new List<GUIData>();
            }
        }

        public void AddGUI(
            GameObject gui, 
            bool removesControl = true,
            bool closesOthers = false)
        {
            Initialise();
            gui.SetActive(false);
            GUIData data = gui.AddComponent<GUIData>();
            data.Initialise(removesControl, closesOthers);
            m_GUIs.Add(data);
        }

        public void ToggleGUI(string name)
        {
            if (m_ActiveGUIs.Any(gui => gui.name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                GUIData[] toToggle = m_ActiveGUIs.Where(gui => gui.name.Equals(name, StringComparison.OrdinalIgnoreCase)).ToArray();
                foreach (GUIData data in toToggle)
                {
                    CloseGUI(data.name);
                }
            }
            else
            {
                OpenGUI(name);
            }
        }

        public void OpenGUI(string name)
        {
            if (m_ActiveGUIs.Any(data => data.name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }
            
            GUIData toOpen = m_GUIs.First(gui => 
                    gui.name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (toOpen.ClosesOthers)
            {
                List<GUIData> activeCopy = new List<GUIData>(m_ActiveGUIs);
                foreach (GUIData data in activeCopy)
                {
                    CloseGUI(data.name);
                }
            }
            
            m_ActiveGUIs.Add(toOpen);
            toOpen.gameObject.SetActive(true);
        }

        public void CloseGUI(string activeName)
        {
            if (m_ActiveGUIs.Any(data => data.name.Equals(activeName, StringComparison.OrdinalIgnoreCase)) == false)
            {
                return;
            }
            
            GUIData toClose = m_ActiveGUIs
                .First(gui => gui.name.Equals(activeName, StringComparison.OrdinalIgnoreCase));
            
            toClose.gameObject.SetActive(false);
            m_ActiveGUIs.Remove(toClose);
        }

        public void BringToFront(string name)
        {
            if (!m_ActiveGUIs.Any(gui => gui.name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }
            int index = m_ActiveGUIs.IndexOf(m_GUIs.First(g => g.name.Equals(name, StringComparison.OrdinalIgnoreCase)));
            UnityEngine.GUI.BringWindowToFront(index);
        }

        public void CloseAllOtherGUIs(string activeName = "")
        {
            GUIData[] toClose = m_ActiveGUIs
                .Where(gui => gui.name.Equals(activeName, StringComparison.OrdinalIgnoreCase)).ToArray();

            foreach (GUIData data in toClose)
            {
                data.gameObject.SetActive(false);
                m_ActiveGUIs.Remove(data);
            }
        }

        public bool RemovesControl()
        {
            GUIData[] data = m_ActiveGUIs.Where(gui => gui.RemovesControl).ToArray();
            return data.Length > 0;
        }

        public GameObject GetGUI(string name)
        {
            return m_GUIs.First(gui => gui.name.Equals(name, StringComparison.OrdinalIgnoreCase)).gameObject;
        }

        public bool IsActive(string name)
        {
            return m_ActiveGUIs.Any(gui => gui.name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
