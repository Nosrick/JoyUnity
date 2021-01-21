using GameSettings;

namespace JoyLib.Code.Settings
{
    public class DyslexicModeSetting : BoolSetting
    {
        public override string settingName => "Dyslexic Mode";
        public override bool value { get; set; }
    }
}