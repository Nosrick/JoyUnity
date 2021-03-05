using System.Collections.Generic;
using GameSettings;
using JoyLib.Code.Events;
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
            DyslexicModeSetting dyslexicModeSetting = ScriptableObject.CreateInstance<DyslexicModeSetting>();
            dyslexicModeSetting.Load();
            this.Settings.Add(dyslexicModeSetting.settingName, dyslexicModeSetting);

            JoyWorldShaderSetting joyWorldShaderSetting = ScriptableObject.CreateInstance<JoyWorldShaderSetting>();
            joyWorldShaderSetting.Load();
            this.Settings.Add(joyWorldShaderSetting.settingName, joyWorldShaderSetting);

            JoyUIShaderSetting joyUIShaderSetting = ScriptableObject.CreateInstance<JoyUIShaderSetting>();
            joyUIShaderSetting.Load();
            this.Settings.Add(joyUIShaderSetting.settingName, joyUIShaderSetting);

            JoyCursorShaderSetting joyCursorShaderSetting = ScriptableObject.CreateInstance<JoyCursorShaderSetting>();
            joyCursorShaderSetting.Load();
            this.Settings.Add(joyCursorShaderSetting.settingName, joyCursorShaderSetting);
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
            foreach (GameSetting gameSetting in this.Settings.Values)
            {
                gameSetting.Save();
            }
        }
    }
}