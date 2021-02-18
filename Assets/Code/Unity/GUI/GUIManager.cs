﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Castle.Core.Internal;
using Code.Unity.GUI.Managed_Assets;
using JoyLib.Code.Events;
using JoyLib.Code.Graphics;
using JoyLib.Code.Helpers;
using JoyLib.Code.Settings;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SpriteState = JoyLib.Code.Graphics.SpriteState;

namespace JoyLib.Code.Unity.GUI
{
    public class GUIManager : IGUIManager
    {
        protected HashSet<GUIData> GUIs { get; set; }
        protected HashSet<GUIData> ActiveGUIs { get; set; }

        protected Canvas MainUI { get; set; }

        public IDictionary<string, ISpriteState> UISprites { get; protected set; }
        public IDictionary<string, ISpriteState> Cursors { get; protected set; }

        protected IDictionary<string, TMP_FontAsset> LoadedFonts { get; set; }

        public IDictionary<string, TMP_FontAsset> FontsInUse =>
            this.DyslexicMode ? this.DyslexicModeFonts : this.LoadedFonts;

        public IDictionary<string, Tuple<float, float>> FontSizesInUse =>
            this.DyslexicMode ? this.DyslexicModeFontSizes : this.StandardFontSizes;

        protected IDictionary<string, Tuple<float, float>> StandardFontSizes { get; set; }
        protected IDictionary<string, Tuple<float, float>> DyslexicModeFontSizes { get; set; }
        public IDictionary<string, IDictionary<string, Color>> CursorColours { get; protected set; }
        public IDictionary<string, IDictionary<string, Color>> UISpriteColours { get; protected set; }

        protected bool DyslexicMode { get; set; }
        protected IDictionary<string, TMP_FontAsset> DyslexicModeFonts { get; set; }

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

                this.UISprites = new Dictionary<string, ISpriteState>();

                this.Cursors = GlobalConstants.GameManager.ObjectIconHandler.GetTileSet("Cursors")
                    .Select(data => new SpriteState(data.m_Name, data))
                    .Cast<ISpriteState>()
                    .ToDictionary(state => state.Name, state => state);

                this.CursorColours = new Dictionary<string, IDictionary<string, Color>>();
                this.UISpriteColours = new Dictionary<string, IDictionary<string, Color>>();
                this.LoadedFonts = new Dictionary<string, TMP_FontAsset>
                {
                    {"default", Resources.Load<TMP_FontAsset>("Fonts/OpenDyslexic3")}
                };

                this.DyslexicModeFonts = new Dictionary<string, TMP_FontAsset>
                {
                    {"default", this.LoadedFonts["default"]}
                };
                this.FontColours = new Dictionary<string, Color>
                {
                    {"default", Color.black}
                };
                this.StandardFontSizes = new Dictionary<string, Tuple<float, float>>
                {
                    {"default", new Tuple<float, float>(8f, 36f)}
                };
                this.DyslexicModeFontSizes = new Dictionary<string, Tuple<float, float>>
                {
                    {"default", new Tuple<float, float>(8f, 36f)}
                };
                this.LoadDefaults();
                this.LoadDefinitions();

                GlobalConstants.GameManager.SettingsManager.OnSettingChange -= this.SettingChanged;
                GlobalConstants.GameManager.SettingsManager.OnSettingChange += this.SettingChanged;
            }
        }

        protected void SettingChanged(SettingChangedEventArgs args)
        {
            if (args.Setting is DyslexicModeSetting dyslexicModeSetting)
            {
                this.DyslexicMode = dyslexicModeSetting.value;
                this.RecolourGUIs();
                LayoutRebuilder.MarkLayoutForRebuild(this.MainUI.GetComponent<RectTransform>());
            }
        }

        protected void LoadDefaults()
        {
            string file = Directory.GetCurrentDirectory() + GlobalConstants.SETTINGS_FOLDER + "/GUIDefaults.json";

            if (File.Exists(file))
            {
                this.LoadFontSettings(
                    file,
                    this.StandardFontSizes,
                    this.LoadedFonts);
            }
            else
            {
                GlobalConstants.ActionLog.AddText("COULD NOT FIND GUI DEFAULTS.", LogLevel.Error);
            }

            file = Directory.GetCurrentDirectory() + GlobalConstants.SETTINGS_FOLDER + "/DyslexicMode.json";

            if (File.Exists(file))
            {
                this.LoadFontSettings(
                    file,
                    this.DyslexicModeFontSizes,
                    this.DyslexicModeFonts);
            }

            this.DyslexicMode = (bool) GlobalConstants.GameManager.SettingsManager
                .GetSetting(SettingNames.DYSLEXIC_MODE).objectValue;
        }

        protected void LoadFontSettings(
            string file, 
            IDictionary<string, Tuple<float, float>> sizes, 
            IDictionary<string, TMP_FontAsset> fonts)
        {
            using (StreamReader reader = new StreamReader(file))
            {
                using (JsonTextReader jsonReader = new JsonTextReader(reader))
                {
                    try
                    {
                        JObject jToken = JObject.Load(jsonReader);

                        if (jToken.IsNullOrEmpty())
                        {
                            return;
                        }

                        foreach (JToken child in jToken["GUIData"])
                        {
                            string name = (string) child["Name"] ?? "default";
                            TMP_FontAsset font = child["Value"] is null
                                ? this.LoadedFonts["default"]
                                : Resources.Load<TMP_FontAsset>("Fonts/" + child["Value"]);

                            float minSize = (float) (child["MinFontSize"] ?? 8);
                            float maxSize = (float) (child["MaxFontSize"] ?? 24);

                            sizes.Add(
                                name,
                                new Tuple<float, float>(
                                    minSize,
                                    maxSize));

                            fonts.Add(name, font);
                        }
                    }
                    catch (Exception e)
                    {
                        GlobalConstants.ActionLog.AddText("Failed loading default GUI settings in " + file);
                        GlobalConstants.ActionLog.StackTrace(e);
                    }
                }
            }
        }

        protected void LoadDefinitions()
        {
            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory() + GlobalConstants.DATA_FOLDER +
                                                "Sprite Definitions/GUI/",
                "*.json",
                SearchOption.AllDirectories);

            foreach (string file in files)
            {
                using (StreamReader reader = new StreamReader(file))
                {
                    using (JsonTextReader jsonReader = new JsonTextReader(reader))
                    {
                        try
                        {
                            JObject jToken = JObject.Load(jsonReader);

                            if (jToken.IsNullOrEmpty())
                            {
                                continue;
                            }

                            JToken child = jToken["Objects"]["TileSet"];

                            string tileSetName = (string) child["Name"];
                            var spriteData = GlobalConstants.GameManager.ObjectIconHandler.GetTileSet(tileSetName);
                            foreach (SpriteData data in spriteData)
                            {
                                if (this.UISprites.ContainsKey(data.m_Name))
                                {
                                    continue;
                                }
                                this.UISprites.Add(data.m_Name, new SpriteState(data.m_Name, data));
                            }
                        }
                        catch (Exception e)
                        {
                            GlobalConstants.ActionLog.AddText("Could not load GUI definitions from " + file);
                            GlobalConstants.ActionLog.StackTrace(e);
                        }
                    }
                }
            }
        }

        public void SetUIColours(IDictionary<string, IDictionary<string, Color>> background,
            IDictionary<string, IDictionary<string, Color>> cursor,
            IDictionary<string, Color> mainFontColours,
            bool recolour = true,
            bool crossFade = false,
            float duration = 0.1f)
        {
            this.UISpriteColours = background;
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
                if (this.UISprites.TryGetValue(background.ElementName, out ISpriteState state))
                {
                    background.SetBackground(state);
                }
                else
                {
                    GlobalConstants.ActionLog.AddText(
                        "Could not find background " + background.ElementName + " on element " +
                        background.name, LogLevel.Warning);
                }

                if (this.UISpriteColours.TryGetValue(background.ElementName, out IDictionary<string, Color> colours))
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

                (float minFontSize, float maxFontSize) = this.FontSizesInUse[font.ElementName];
                font.SetMinMaxFontSizes(minFontSize, maxFontSize);

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

            ManagedIcon[] icons = gui.GetComponentsInChildren<ManagedIcon>(true);
            foreach (ManagedIcon icon in icons)
            {
                if (this.UISprites.TryGetValue(icon.ElementName, out ISpriteState state))
                {
                    icon.Clear();
                    icon.AddSpriteState(state);
                }
                else
                {
                    GlobalConstants.ActionLog.AddText(
                        "Could not find background " + icon.ElementName + " on element " +
                        icon.name, LogLevel.Warning);
                }

                if (this.UISpriteColours.TryGetValue(icon.ElementName, out IDictionary<string, Color> colours))
                {
                    icon.OverrideAllColours(colours, crossFade, duration);
                }
                else
                {
                    GlobalConstants.ActionLog.AddText("Could not find background colours " + icon.ElementName +
                                                      " on element " +
                                                      icon.name, LogLevel.Warning);
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
                    background.SetBackground(this.UISprites[background.ElementName]);
                }

                if (GlobalConstants.GameManager.Player is null == false
                    && background.HasColours == false)
                {
                    background.SetColours(this.UISpriteColours[background.ElementName]);
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
            GUIData[] toClose = this.ActiveGUIs
                .Where(gui => gui.m_AlwaysOpen == false)
                .ToArray();

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