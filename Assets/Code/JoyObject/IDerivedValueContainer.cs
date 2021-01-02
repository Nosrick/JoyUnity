using System.Collections.Generic;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Events;

namespace JoyLib.Code
{
    public interface IDerivedValueContainer
    {
        IDictionary<string, IDerivedValue> DerivedValues { get; }
        int DamageValue(string name, int value);
        int RestoreValue(string name, int value);
        int ModifyValue(string name, int value);
        int GetValue(string name);
        int GetMaximum(string name);
        int ModifyMaximum(string name, int value);

        event ValueChangedEventHandler OnValueChange;
        event ValueChangedEventHandler OnMaximumChange;
    }
}