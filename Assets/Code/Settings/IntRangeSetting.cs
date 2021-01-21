using GameSettings;

namespace JoyLib.Code.Settings
{
    public abstract class IntRangeSetting : IntSetting
    {
        public virtual int Min { get; set; }
        public virtual int Max { get; set; }
    }
}