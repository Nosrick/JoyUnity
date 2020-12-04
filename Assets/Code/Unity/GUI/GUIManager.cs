using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JoyLib.Code.Unity.GUI
{
    public class GUIManager : IGUIManager
    {
        protected HashSet<GUIData> GUIs { get; set; }
        protected HashSet<GUIData> ActiveGUIs { get; set; }

        public GUIManager()
        {
            Initialise();
        }

        protected void Initialise()
        {
            if (GUIs is null)
            {
                GUIs = new HashSet<GUIData>();
                ActiveGUIs = new HashSet<GUIData>();
            }
        }

        public void AddGUI(GUIData gui)
        {
            Initialise();
            if (GUIs.Contains(gui))
            {
                return;
            }

            gui.GUIManager = this;
            gui.Close();
            GUIs.Add(gui);
        }

        public void ToggleGUI(string name)
        {
            if (ActiveGUIs.Any(gui => gui.name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                GUIData[] toToggle = ActiveGUIs.Where(gui => gui.name.Equals(name, StringComparison.OrdinalIgnoreCase)).ToArray();
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
            if (ActiveGUIs.Any(widget => widget.name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }
            
            GUIData toOpen = GUIs.First(gui => 
                    gui.name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (toOpen.m_ClosesOthers)
            {
                List<GUIData> activeCopy = new List<GUIData>(ActiveGUIs);
                foreach (GUIData widget in activeCopy)
                {
                    CloseGUI(widget.name);
                }
            }
            
            ActiveGUIs.Add(toOpen);
            toOpen.Show();
        }

        public void CloseGUI(string activeName)
        {
            if (ActiveGUIs.Any(data => data.name.Equals(activeName, StringComparison.OrdinalIgnoreCase)) == false)
            {
                return;
            }
            
            GUIData toClose = ActiveGUIs
                .First(gui => gui.name.Equals(activeName, StringComparison.OrdinalIgnoreCase));
            
            toClose.Close();
            ActiveGUIs.Remove(toClose);
        }

        public bool RemoveActiveGUI(string name)
        {
            if (ActiveGUIs.Any(data => data.name.Equals(name, StringComparison.OrdinalIgnoreCase)) == false)
            {
                return false;
            }

            GUIData toClose = ActiveGUIs.First(data => data.name.Equals(name, StringComparison.OrdinalIgnoreCase));
            return ActiveGUIs.Remove(toClose);
            }

        public void BringToFront(string name)
        {
            if (!ActiveGUIs.Any(gui => gui.name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }
            GUIData toFront = ActiveGUIs.First(g => g.name.Equals(name, StringComparison.OrdinalIgnoreCase));
            foreach (GUIData gui in ActiveGUIs)
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
            GUIData[] toClose = ActiveGUIs
                .Where(gui => gui.name.Equals(activeName, StringComparison.OrdinalIgnoreCase) == false).ToArray();

            foreach (GUIData data in toClose)
            {
                data.Close();
                ActiveGUIs.Remove(data);
            }
        }

        public bool RemovesControl()
        {
            IEnumerable<GUIData> data = ActiveGUIs.Where(gui => gui.GetType().Equals(typeof(GUIData))).Cast<GUIData>();
            return data.Any(gui => gui.m_RemovesControl);
        }

        public GUIData GetGUI(string name)
        {
            return GUIs.First(gui => gui.name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public bool IsActive(string name)
        {
            return ActiveGUIs.Any(gui => gui.name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public bool AreAnyOpen()
        {
            return ActiveGUIs.Count > 0;
        }
    }
}
