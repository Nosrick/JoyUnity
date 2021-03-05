using GameSettings;

namespace JoyLib.Code.Settings
{
    public class JoyCursorShaderSetting : BoolSetting
    {
        public override string settingName => "Joy Cursor Shader";
        public override bool value { get; set; }

        public void Awake()
        {
            this.name = this.settingName;
        }
    }
}