using JoyLib.Code.Collections;
using JoyLib.Code.Entities.Statistics;

namespace JoyLib.Code
{
    public interface IDerivedValueContainer
    {
        BasicValueContainer<IDerivedValue> DerivedValues { get; }
        int DamageValue(string name, int value);
        int RestoreValue(string name, int value);
        int ModifyValue(string name, int value);
        int GetValue(string name);
        int GetMaximum(string name);
        int ModifyMaximum(string name, int value);
    }
}