using GameSettings;

namespace JoyLib.Code.Settings
{
    public abstract class FloatRangeSetting : FloatSetting
    {
        public virtual float Min { get; set; }
        public virtual float Max { get; set; }
    }
}