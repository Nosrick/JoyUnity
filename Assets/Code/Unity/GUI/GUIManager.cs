using System;
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

        public ISpriteState Background { get; protected set; }
        public ISpriteState Cursor { get; protected set; }

        public ISpriteState AccentBackground { get; protected set; }

        public TMP_FontAsset FontToUse { get; protected set; }

        public IDictionary<string, Color> CursorColours { get; protected set; }
        public IDictionary<string, Color> BackgroundColours { get; protected set; }

        public IDictionary<string, Color> AccentColours { get; protected set; }
        public Color AccentFontColour { get; protected set; }

        public float MinFontSize { get; protected set; }
        public float MaxFontSize { get; protected set; }

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
                    "WindowBackground",
                    GlobalConstants.GameManager.ObjectIconHandler.GetSprites(
                            "WindowBackground",
                            "WindowBackground")
                        .First());

                this.Cursor = new SpriteState(
                    "Cursor",
                    GlobalConstants.GameManager.ObjectIconHandler.GetFrame(
                        "DefaultCursor",
                        "DefaultCursor"));

                this.AccentBackground = new SpriteState(
                    "AccentBackground",
                    GlobalConstants.GameManager.ObjectIconHandler.GetFrame(
                        "AccentBackground",
                        "AccentBackground"));

                this.CursorColours = new Dictionary<string, Color>();
                this.BackgroundColours = new Dictionary<string, Color>();
                this.AccentColours = new Dictionary<string, Color>();
                this.FontToUse = Resources.Load<TMP_FontAsset>("Fonts/OpenDyslexic3");
                this.AccentFontColour = Color.black;
                this.MinFontSize = 10f;
                this.MaxFontSize = 36f;
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
                            this.MinFontSize = data.Element("MinFontSize").DefaultIfEmpty(10f);
                            this.MaxFontSize = data.Element("MaxFontSize").DefaultIfEmpty(36f);
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

        public void SetUIColours(
            IDictionary<string, Color> background,
            IDictionary<string, Color> cursor,
            IDictionary<string, Color> accentColours,
            Color accentFontColour)
        {
            this.BackgroundColours = background;
            this.CursorColours = cursor;
            this.AccentColours = accentColours;
            this.AccentFontColour = accentFontColour;

            this.RecolourGUIs();
        }

        public void SetFont(TMP_FontAsset font)
        {
            this.FontToUse = font;
        }

        public void Clear()
        {
            this.ActiveGUIs.Clear();
            this.GUIs.Clear();
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

            foreach (ManagedBackground background in gui.GetComponentsInChildren<ManagedBackground>(true))
            {
                if (background.HasBackground == false)
                {
                    background.SetBackground(this.Background);
                }

                if (background.HasColours == false)
                {
                    background.SetColours(this.BackgroundColours);
                }
            }

            foreach (ManagedFonts font in gui.GetComponentsInChildren<ManagedFonts>(true))
            {
                if (font.HasFont == false)
                {
                    font.SetFonts(this.FontToUse);
                    font.SetMinMaxFontSizes(this.MinFontSize, this.MaxFontSize);
                }
            }

            foreach (ManagedAccent accent in gui.GetComponentsInChildren<ManagedAccent>(true))
            {
                if (accent.HasFont == false)
                {
                    accent.SetFonts(this.FontToUse);
                    accent.SetMinMaxFontSizes(this.MinFontSize, this.MaxFontSize);
                }

                if (accent.HasFontColours == false)
                {
                    accent.SetFontColour(this.AccentFontColour);
                }

                if (accent.HasBackgroundImage == false)
                {
                    accent.SetBackgrounds(this.AccentBackground);
                }

                if (accent.HasBackgroundColours == false)
                {
                    accent.SetBackgroundColours(this.AccentColours);
                }
            }

            this.GUIs.Add(gui);
        }

        public void RecolourGUIs()
        {
            foreach (GUIData gui in this.GUIs)
            {
                foreach (ManagedBackground background in gui.GetComponentsInChildren<ManagedBackground>(true))
                {
                    background.SetBackground(this.Background);
                    background.SetColours(this.BackgroundColours);
                }

                foreach (ManagedFonts font in gui.GetComponentsInChildren<ManagedFonts>(true))
                {
                    font.SetFonts(this.FontToUse);
                    font.SetMinMaxFontSizes(this.MinFontSize, this.MaxFontSize);
                }

                foreach (ManagedAccent accent in gui.GetComponentsInChildren<ManagedAccent>(true))
                {
                    accent.SetFonts(this.FontToUse);
                    accent.SetMinMaxFontSizes(this.MinFontSize, this.MaxFontSize);

                    accent.SetFontColour(this.AccentFontColour);

                    accent.SetBackgrounds(this.AccentBackground);
                    accent.SetBackgroundColours(this.AccentColours);
                }
            }

            Cursor cursor = null;
            this.GUIs.FirstOrDefault(data => data.TryGetComponent(out cursor));
            if (cursor is null == false)
            {
                cursor.SetCursorColours(this.CursorColours);
            }
        }

        public void ToggleGUI(string name)
        {
            if (this.ActiveGUIs.Any(gui => gui.name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                GUIData[] toToggle = this.ActiveGUIs
                    .Where(gui => gui.name.Equals(name, StringComparison.OrdinalIgnoreCase))
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

        public GUIData OpenGUI(string name, bool bringToFront = false)
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

            toOpen.Show();

            this.ActiveGUIs.Add(toOpen);

            if (toOpen.TryGetComponent(out ManagedBackground background))
            {
                if (background.HasBackground == false)
                {
                    background.SetBackground(this.Background);
                }

                if (GlobalConstants.GameManager.Player is null == false
                    && background.HasColours == false)
                {
                    background.SetColours(this.BackgroundColours);
                }
            }

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

            GUIData[] found = this.ActiveGUIs
                .Where(data => data.m_AlwaysOpen == false)
                .ToArray();
            if (found.Any())
            {
                toFront.MyCanvas.sortingOrder = found.Max(data => data.DefaultSortingOrder) + 1;
            }
        }

        public void CloseAllOtherGUIs(string activeName = "")
        {
            IEnumerable<GUIData> toClose = this.ActiveGUIs
                .Where(gui => gui.name.Equals(activeName, StringComparison.OrdinalIgnoreCase) == false
                              && gui.m_AlwaysOpen == false);

            foreach (GUIData data in toClose)
            {
                this.ActiveGUIs.Remove(data);
                data.Close();
            }
        }

        public void CloseAllGUIs()
        {
            IEnumerable<GUIData> toClose = this.ActiveGUIs
                .Where(gui => gui.m_AlwaysOpen == false);

            foreach (GUIData data in toClose)
            {
                this.ActiveGUIs.Remove(data);
                data.Close();
            }
        }

        public bool RemovesControl()
        {
            IEnumerable<GUIData> data = this.ActiveGUIs.Where(gui => gui.GetType().Equals(typeof(GUIData)))
                .Cast<GUIData>();
            return data.Any(gui => gui.m_RemovesControl);
        }

        public GUIData GetGUI(string name)
        {
            return this.GUIs.FirstOrDefault(gui => gui.name.Equals(name, StringComparison.OrdinalIgnoreCase));
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