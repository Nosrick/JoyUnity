using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using GameSettings;
using JoyLib.Code.Events;
using JoyLib.Code.Helpers;
using Newtonsoft.Json;
using UnityEngine;

namespace JoyLib.Code.Settings
{
    public class SettingsManager
    {
        public event SettingChangedEventHandler OnSettingChange;
        
        public IDictionary<string, GameSetting> Settings { get; protected set; }

        public SettingsManager()
        {
            this.Settings = new Dictionary<string, GameSetting>();
            this.LoadDefaults();
        }

        protected void LoadDefaults()
        {
            string file = Directory.GetCurrentDirectory() + GlobalConstants.DATA_FOLDER + "Settings.json";
            if (File.Exists(file))
            {
                try
                {
                    JsonSerializer serializer = JsonSerializer.CreateDefault();
                    JsonReader reader = new JsonTextReader(new StreamReader(file));
                    while (reader.Read())
                    {
                        GameSetting settingData = serializer.Deserialize<GameSetting>(reader);
                        if (settingData is null)
                        {
                            continue;
                        }
                        this.Settings.Add(settingData.settingName, settingData);
                    }
                }
                catch
                {
                    GlobalConstants.ActionLog.AddText("Could not load Settings.xml!", LogLevel.Error);
                }
            }
            else
            {
                GlobalConstants.ActionLog.AddText("No Settings.json! Creating default.", LogLevel.Warning);
                this.Settings.Add(
                    "dyslexic",
                    ScriptableObject.CreateInstance<DyslexicModeSetting>());
                
                JsonSerializer serializer = JsonSerializer.CreateDefault();
                JsonWriter writer = new JsonTextWriter(new StreamWriter(file));
                foreach (GameSetting settingData in this.Settings.Values)
                {
                    serializer.Serialize(writer, settingData);
                }
            }
        }

        public GameSetting GetSetting(string name)
        {
            return this.Settings.TryGetValue(name, out GameSetting data) ? data : null;
        }

        public bool SetSetting(GameSetting data)
        {
            if (!this.Settings.ContainsKey(data.settingName))
            {
                return false;
            }
            this.Settings[data.settingName] = data;
            this.OnSettingChange?.Invoke(new SettingChangedEventArgs
            {
                Setting = data
            });
            return true;

        }

        public void Save()
        {
            try
            {
                string file = Directory.GetCurrentDirectory() + GlobalConstants.DATA_FOLDER + "Settings.xml";
                XmlSerializer serializer = new XmlSerializer(typeof(GameSetting));
                FileStream fileStream = File.Create(file);
                foreach (GameSetting settingData in this.Settings.Values)
                {
                    settingData.Save();
                    serializer.Serialize(fileStream, settingData);
                }
                fileStream.Close();
            }
            catch
            {
                GlobalConstants.ActionLog.AddText("Could not save settings!", LogLevel.Error);
            }
        }
    }
}