﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using JoyLib.Code.Graphics;
using JoyLib.Code.Helpers;
using TMPro;
using UnityEngine;

namespace JoyLib.Code.Unity.GUI
{
    public class GUIManager : IGUIManager
    {
        protected HashSet<GUIData> GUIs { get; set; }
        protected HashSet<GUIData> ActiveGUIs { get; set; }
        
        protected ISpriteState Background { get; set; }
        
        protected TMP_FontAsset FontToUse { get; set; }
        
        protected IDictionary<string, Color> CursorColours { get; set; }
        protected IDictionary<string, Color> BackgroundColours { get; set; }

        public GUIManager()
        {
            this.Initialise();
        }

        protected void Initialise()
        {
            if (this.GUIs is null)
            {
                this.GUIs = new HashSet<GUIData>();
                this.ActiveGUIs = new HashSet<GUIData>();
                this.Background = new SpriteState(
                    "Background",
                    GlobalConstants.GameManager.ObjectIconHandler.GetSprites(
                            "WindowBackground",
                            "WindowBackground")
                        .First());

                this.CursorColours = new Dictionary<string, Color>();
                this.BackgroundColours = new Dictionary<string, Color>();
                this.FontToUse = Resources.Load<TMP_FontAsset>("Fonts/Kenney Pixel SDF");
                this.LoadDefaults();
            }
        }

        protected void LoadDefaults()
        {
            string file = Directory.GetCurrentDirectory() + GlobalConstants.DATA_FOLDER + "/GUIDefaults.xml";

            if (File.Exists(file))
            {
                XElement doc = XElement.Load(file);
                foreach (XElement data in doc.Elements("Data"))
                {
                    switch (data.Element("Name").GetAs<string>())
                    {
                        case "Font":
                            this.FontToUse =
                                Resources.Load<TMP_FontAsset>("Fonts/" + data.Element("Value").GetAs<string>());
                            break;

                        default:
                            break;
                    }
                }
            }
            else
            {
                GlobalConstants.ActionLog.AddText("COULD NOT FIND GUI DEFAULTS.", LogLevel.Error);
            }
        }

        public void SetUIColours(IDictionary<string, Color> background, IDictionary<string, Color> cursor)
        {
            this.BackgroundColours = background;
            this.CursorColours = cursor;
        }

        public void SetFont(TMP_FontAsset font)
        {
            
        }

        public void AddGUI(GUIData gui)
        {
            this.Initialise();
            if (this.GUIs.Contains(gui))
            {
                return;
            }

            gui.Awake();
            gui.GUIManager = this;
            gui.Close();
            if (gui is SkinnableGUI skinnableGUI && skinnableGUI.HasBackground == false)
            {
                skinnableGUI.SetBackground(this.Background);
            }
            this.GUIs.Add(gui);
        }

        public void ToggleGUI(string name)
        {
            if (this.ActiveGUIs.Any(gui => gui.name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                GUIData[] toToggle = this.ActiveGUIs.Where(gui => gui.name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    .ToArray();
                foreach (GUIData data in toToggle)
                {
                    this.CloseGUI(data.name);
                }
            }
            else
            {
                this.OpenGUI(name);
            }
        }

        public GUIData OpenGUI(string name, bool bringToFront = true)
        {
            if (this.ActiveGUIs.Any(widget => widget.name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                return this.ActiveGUIs.First(ui => ui.name.Equals(name, StringComparison.OrdinalIgnoreCase));
            }

            GUIData toOpen = this.GUIs.First(gui =>
                gui.name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (toOpen.m_ClosesOthers)
            {
                List<GUIData> activeCopy = new List<GUIData>(this.ActiveGUIs);
                foreach (GUIData widget in activeCopy)
                {
                    this.CloseGUI(widget.name);
                }
            }
            
            if (toOpen is SkinnableGUI skinnableGUI)
            {
                if (GlobalConstants.GameManager.Player is null == false 
                    && skinnableGUI.HasColours == false)
                {
                    skinnableGUI.SetColours(this.BackgroundColours);
                }

                if (skinnableGUI.HasFont == false)
                {
                    skinnableGUI.SetFont(this.FontToUse);
                }
            }

            toOpen.Show();
            
            this.ActiveGUIs.Add(toOpen);

            if (bringToFront)
            {
                this.BringToFront(toOpen.name);
            }
            return toOpen;
        }

        public void CloseGUI(string activeName)
        {
            if (this.ActiveGUIs.Any(data => data.name.Equals(activeName, StringComparison.OrdinalIgnoreCase)) == false)
            {
                return;
            }

            GUIData toClose = this.ActiveGUIs
                .First(gui => gui.name.Equals(activeName, StringComparison.OrdinalIgnoreCase));

            if (toClose.m_AlwaysOpen)
            {
                return;
            }
            
            toClose.Close();
            this.ActiveGUIs.Remove(toClose);
        }

        public bool RemoveActiveGUI(string name)
        {
            if (this.ActiveGUIs.Any(data => data.name.Equals(name, StringComparison.OrdinalIgnoreCase)) == false)
            {
                return false;
            }

            GUIData toClose = this.ActiveGUIs.First(data => data.name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (toClose.m_AlwaysOpen)
            {
                return false;
            }
            
            toClose.Close();
            return this.ActiveGUIs.Remove(toClose);
        }

        public void BringToFront(string name)
        {
            if (!this.ActiveGUIs.Any(gui => gui.name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            GUIData toFront = this.ActiveGUIs.First(g => g.name.Equals(name, StringComparison.OrdinalIgnoreCase));
            foreach (GUIData gui in this.ActiveGUIs)
            {
                if (toFront.Equals(gui) || gui.m_AlwaysOnTop)
                {
                    continue;
                }

                gui.MyCanvas.sortingOrder = gui.DefaultSortingOrder;
            }

            toFront.MyCanvas.sortingOrder = this.ActiveGUIs.Max(data => data.DefaultSortingOrder) + 1;
        }

        public void CloseAllOtherGUIs(string activeName = "")
        {
            GUIData[] toClose = this.ActiveGUIs
                .Where(gui => gui.name.Equals(activeName, StringComparison.OrdinalIgnoreCase) == false).ToArray();

            foreach (GUIData data in toClose)
            {
                if (data.m_AlwaysOpen)
                {
                    continue;
                }

                this.ActiveGUIs.Remove(data);
                data.Close();
            }
        }

        public bool RemovesControl()
        {
            IEnumerable<GUIData> data = this.ActiveGUIs.Where(gui => gui.GetType().Equals(typeof(GUIData))).Cast<GUIData>();
            return data.Any(gui => gui.m_RemovesControl);
        }

        public GUIData GetGUI(string name)
        {
            return this.GUIs.First(gui => gui.name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public bool IsActive(string name)
        {
            return this.ActiveGUIs.Any(gui => gui.name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public bool AreAnyOpen()
        {
            return this.ActiveGUIs.Count > 0;
        }
    }
}