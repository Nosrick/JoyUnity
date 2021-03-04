using GameSettings;

namespace JoyLib.Code.Settings
{
    public class JoyShaderSetting : BoolSetting
    {
        public override string settingName => "Joy Shader";
        public override bool value { get; set; }

        public void Awake()
        {
            this.name = this.settingName;
        }
    }
}