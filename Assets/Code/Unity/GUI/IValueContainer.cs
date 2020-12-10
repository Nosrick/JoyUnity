using JoyLib.Code.Events;

namespace JoyLib.Code.Unity.GUI
{
    public interface IValueContainer
    {
        int DecreaseValue(int delta = 1);
        int IncreaseValue(int delta = 1);

        int DirectValueSet(int newValue);
        
        int Value { get; set; }
        
        string Tooltip { get; set; }
        
        string Name { get; set; }

        event ValueChangedEventHandler ValueChanged;
    }
}