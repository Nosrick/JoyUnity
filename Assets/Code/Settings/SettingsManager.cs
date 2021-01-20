using System.Collections.Generic;

namespace JoyLib.Code.Settings
{
    public class SettingsManager
    {
        public IDictionary<string, SettingData> Settings { get; protected set; }

        public SettingsManager()
        {
            this.Settings = new Dictionary<string, SettingData>();
        }

        protected void LoadDefaults()
        {
            this.Settings.Add(
                "dyslexic",
                new SettingData
                {
                    Data = false,
                    Max = true,
                    Min = false,
                    RangeType = RangeType.Boolean
                });
        }

        public SettingData GetSetting(string name)
        {
            return this.Settings.TryGetValue(name, out SettingData data) ? data : new SettingData();
        }

        public bool SetSetting(string name, dynamic data)
        {
            if (!this.Settings.ContainsKey(name))
            {
                return false;
            }
            
            this.Settings[name].Data = data;
            return true;
        }
    }

    public class SettingData
    {
        public dynamic Data { get; set; }
        public dynamic Min { get; set; }
        public dynamic Max { get; set; }
        
        public RangeType RangeType { get; set; }

        public SettingData()
        {
            this.Data = false;
            this.Min = false;
            this.Max = true;
            this.RangeType = RangeType.Boolean;
        }
    }

    public enum RangeType
    {
        Boolean,
        Slider,
        ValueSelect
    }
}