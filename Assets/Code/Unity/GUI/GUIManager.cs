using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using JoyLib.Code.Events;
using JoyLib.Code.Graphics;
using JoyLib.Code.Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JoyLib.Code.Unity.GUI
{
    public class GUIManager : IGUIManager
    {
        protected HashSet<GUIData> GUIs { get; set; }
        protected HashSet<GUIData> ActiveGUIs { get; set; }

        protected Canvas MainUI { get; set; }

        public IDictionary<string, ISpriteState> Backgrounds { get; protected set; }
        public IDictionary<string, ISpriteState> Cursors { get; protected set; }

        public IDictionary<string, TMP_FontAsset> LoadedFonts { get; protected set; }
        
        protected IDictionary<string, TMP_FontAsset> FontsInUse { get; set; }
        
        protected TMP_FontAsset DyslexicModeFont { get; set; }

        public IDictionary<string, IDictionary<string, Color>> CursorColours { get; protected set; }
        public IDictionary<string, IDictionary<string, Color>> BackgroundColours { get; protected set; }

        public float MinFontSize { get; protected set; }
        public float MaxFontSize { get; protected set; }

        public IDictionary<string, Color> FontColours { get; protected set; }

        public GUIManager()
        {
            this.Initialise();
        }

        protected void Initialise()
        {
            if (this.GUIs is null)
            {
                this.MainUI = GameObject.Find("MainUI").GetComponent<Canvas>();
                this.GUIs = new HashSet<GUIData>();
                this.ActiveGUIs = new HashSet<GUIData>();
                this.Backgrounds = GlobalConstants.GameManager.ObjectIconHandler.GetTileSet("Windows")
                    .Select(data => new SpriteState(data.m_Name, data))
                    .Cast<ISpriteState>()
                    .ToDictionary(state => state.Name, state => state);


                this.Cursors = GlobalConstants.GameManager.ObjectIconHandler.GetTileSet("Cursors")
                    .Select(data => new SpriteState(data.m_Name, data))
                    .Cast<ISpriteState>()
                    .ToDictionary(state => state.Name, state => state);

                this.CursorColours = new Dictionary<string, IDictionary<string, Color>>();
                this.BackgroundColours = new Dictionary<string, IDictionary<string, Color>>();
                this.LoadedFonts = new Dictionary<string, TMP_FontAsset>
                {
                    {"default", Resources.Load<TMP_FontAsset>("Fonts/OpenDyslexic3")}
                };
                this.DyslexicModeFont = this.LoadedFonts["default"];
                this.FontColours = new Dictionary<string, Color>
                {
                    {"default", Color.black}
                };
                this.MinFontSize = 10f;
                this.MaxFontSize = 36f;
                this.LoadDefaults();

                GlobalConstants.GameManager.SettingsManager.OnSettingChange -= this.SettingChanged;
                GlobalConstants.GameManager.SettingsManager.OnSettingChange += this.SettingChanged;
            }
        }

        protected void SettingChanged(SettingChangedEventArgs args)
        {
            if (args.Setting.settingName.Equals("dyslexic", StringComparison.OrdinalIgnoreCase))
            {
                if ((bool)args.Setting.objectValue)
                {
                    foreach (string key in this.FontsInUse.Keys)
                    {
                        this.FontsInUse[key] = this.DyslexicModeFont;
                    }
                }
                else
                {
                    foreach (string key in this.FontsInUse.Keys)
                    {
                        this.FontsInUse[key] = this.LoadedFonts[key];
                    }
                }
                
                this.RecolourGUIs();
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
                    this.LoadedFonts.Add(
                        data.Element("Name").GetAs<string>(),
                        Resources.Load<TMP_FontAsset>("Fonts/" + data.Element("Value").GetAs<string>()));
                    this.MinFontSize = data.Element("MinFontSize").DefaultIfEmpty(10f);
                    this.MaxFontSize = data.Element("MaxFontSize").DefaultIfEmpty(36f);
                    this.FontColours["default"] =
                        GraphicsHelper.ParseHTMLString(data.Element("FontColour").DefaultIfEmpty("#000000ff"));
                }
            }
            else
            {
                GlobalConstants.ActionLog.AddText("COULD NOT FIND GUI DEFAULTS.", LogLevel.Error);
            }
        }

        public void SetUIColours(IDictionary<string, IDictionary<string, Color>> background,
            IDictionary<string, IDictionary<string, Color>> cursor,
            IDictionary<string, Color> mainFontColours,
            bool recolour = true, 
            bool crossFade = false,
            float duration = 0.1f)
        {
            this.BackgroundColours = background;
            this.CursorColours = cursor;
            this.FontColours = mainFontColours;

            if (recolour)
            {
                this.RecolourGUIs(crossFade, duration);
            }
        }

        public void Clear()
        {
            this.ActiveGUIs.Clear();
            this.GUIs.Clear();
        }

        public void FindGUIs()
        {
            Canvas mainUI = null;
            SceneManager.GetActiveScene().GetRootGameObjects().First(o => o.TryGetComponent(out mainUI));
            this.MainUI = mainUI;

            GUIData[] guiData = this.MainUI.GetComponentsInChildren<GUIData>(true);
            foreach (GUIData data in guiData)
            {
                this.AddGUI(data);
            }

            Cursor cursor = this.GUIs.First(data => data.name.Equals(GUINames.CURSOR)).GetComponent<Cursor>();
            if (cursor is null)
            {
                return;
            }
            cursor.SetCursorSprites(this.Cursors["DefaultCursor"]);
            cursor.SetCursorColours(this.CursorColours["DefaultCursor"]);
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

            this.SetupManagedComponents(gui);

            this.GUIs.Add(gui);
        }

        public void RecolourGUIs(bool crossFade = false, float duration = 0.1f)
        {
            foreach (GUIData gui in this.GUIs)
            {
                this.SetupManagedComponents(gui, crossFade, duration);
            }

            Cursor cursor = null;
            this.GUIs.FirstOrDefault(data => data.TryGetComponent(out cursor));
            if (cursor is null == false)
            {
                cursor.SetCursorSprites(this.Cursors["DefaultCursor"]);
                cursor.SetCursorColours(this.CursorColours["DefaultCursor"]);
            }
        }

        public void SetupManagedComponents(GUIData gui, bool crossFade = false, float duration = 0.1f)
        {
            ManagedBackground[] backgrounds = gui.GetComponentsInChildren<ManagedBackground>(true);
            foreach (ManagedBackground background in backgrounds)
            {
                if (this.Backgrounds.TryGetValue(background.ElementName, out ISpriteState state))
                {
                    background.SetBackground(state);
                }
                else
                {
                    GlobalConstants.ActionLog.AddText(
                        "Could not find background " + background.ElementName + " on element " +
                        background.name, LogLevel.Warning);
                }

                if (this.BackgroundColours.TryGetValue(background.ElementName, out IDictionary<string, Color> colours))
                {
                    background.SetColours(colours, crossFade, duration);
                }
                else
                {
                    GlobalConstants.ActionLog.AddText("Could not find background colours " + background.ElementName +
                                                      " on element " +
                                                      background.name, LogLevel.Warning);
                }
            }

            ManagedFonts[] fonts = gui.GetComponentsInChildren<ManagedFonts>(true);
            foreach (ManagedFonts font in fonts)
            {
                if (this.FontsInUse.TryGetValue(font.ElementName, out TMP_FontAsset fontToUse))
                {
                    font.SetFonts(fontToUse);
                }
                else
                {
                    GlobalConstants.ActionLog.AddText("Could not find font " + font.ElementName + " on element " +
                                                      font.name, LogLevel.Warning);
                }

                font.SetMinMaxFontSizes(this.MinFontSize, this.MaxFontSize);

                if (this.FontColours.TryGetValue(font.ElementName, out Color colour))
                {
                    font.SetFontColour(colour);
                }
                else
                {
                    GlobalConstants.ActionLog.AddText("Could not find font colour " + font.ElementName +
                                                      " on element " + font.name,
                        LogLevel.Warning);
                }
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
                GUIData openGUI = this.ActiveGUIs.First(ui => ui.name.Equals(name, StringComparison.OrdinalIgnoreCase));
                if (bringToFront)
                {
                    this.BringToFront(openGUI.name);
                }

                return openGUI;
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
                    background.SetBackground(this.Backgrounds[background.ElementName]);
                }

                if (GlobalConstants.GameManager.Player is null == false
                    && background.HasColours == false)
                {
                    background.SetColours(this.BackgroundColours[background.ElementName]);
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
            GUIData[] toClose = this.ActiveGUIs
                .Where(gui => gui.name.Equals(activeName, StringComparison.OrdinalIgnoreCase) == false
                              && gui.m_AlwaysOpen == false)
                .ToArray();

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
                data.Close();
            }

            this.ActiveGUIs.RemoveWhere(guiData => guiData.m_AlwaysOpen == false);
        }

        public bool RemovesControl()
        {
            return this.ActiveGUIs.Any(gui => gui.m_RemovesControl);
        }

        public GUIData GetGUI(string name)
        {
            return this.GUIs.FirstOrDefault(gui => gui.name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public bool IsActive(string name)
        {
            return this.ActiveGUIs.Any(gui => gui.name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public bool AreAnyOpen(bool includeAlwaysOpen = false)
        {
            if (includeAlwaysOpen)
            {
                return this.ActiveGUIs.Count > 0;
            }

            return this.ActiveGUIs.Count(data => data.m_AlwaysOpen == false) > 0;
        }
    }
}