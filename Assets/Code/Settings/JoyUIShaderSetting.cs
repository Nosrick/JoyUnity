using GameSettings;

namespace JoyLib.Code.Settings
{
    public class JoyUIShaderSetting : BoolSetting
    {
        public override string settingName => "Joy UI Shader";
        public override bool value { get; set; }

        public void Awake()
        {
            this.name = this.settingName;
        }
    }
}