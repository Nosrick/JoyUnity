﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JoyLib.Code.Unity.GUI
{
    public class GUIManager : IGUIManager
    {
        private List<GUIData> m_GUIs;
        private List<GUIData> m_ActiveGUIs;

        public GUIManager()
        {
            Initialise();
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
            GUIData toFront = m_ActiveGUIs.First(g => g.name.Equals(name, StringComparison.OrdinalIgnoreCase));
            foreach (GUIData gui in m_ActiveGUIs)
            {
                if (toFront.Equals(gui))
                {
                    continue;
                }

                gui.transform.parent.GetComponent<Canvas>().sortingOrder = int.MinValue;
            }

            toFront.transform.parent.GetComponent<Canvas>().sortingOrder = 1;
        }

        public void CloseAllOtherGUIs(string activeName = "")
        {
            GUIData[] toClose = m_ActiveGUIs
                .Where(gui => gui.name.Equals(activeName, StringComparison.OrdinalIgnoreCase) == false).ToArray();

            foreach (GUIData data in toClose)
            {
                data.gameObject.SetActive(false);
                m_ActiveGUIs.Remove(data);
            }
        }

        public bool RemovesControl()
        {
            return m_ActiveGUIs.Any(gui => gui.RemovesControl);
        }

        public GameObject GetGUI(string name)
        {
            return m_GUIs.First(gui => gui.name.Equals(name, StringComparison.OrdinalIgnoreCase)).gameObject;
        }

        public bool IsActive(string name)
        {
            return m_ActiveGUIs.Any(gui => gui.name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public bool AreAnyOpen()
        {
            return m_ActiveGUIs.Count > 0;
        }
    }
}
