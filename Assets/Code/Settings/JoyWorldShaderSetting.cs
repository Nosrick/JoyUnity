using GameSettings;

namespace JoyLib.Code.Settings
{
    public class JoyWorldShaderSetting : BoolSetting
    {
        public override string settingName => "Joy World Shader";
        public override bool value { get; set; }

        public void Awake()
        {
            this.name = this.settingName;
        }
    }
}