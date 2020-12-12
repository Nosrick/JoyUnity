using System.Collections.Generic;
using UnityEngine;

namespace JoyLib.Code.Entities.Statistics
{
    public interface IDerivedValueHandler
    {
        IDerivedValue<K> Calculate<T, K>(string name, IEnumerable<IBasicValue<T>> components) 
            where T : struct
            where K : struct;

        IDerivedValue<T> Calculate<T>(string name, IEnumerable<IBasicValue<T>> components)
            where T : struct;

        T Calculate<T>(IEnumerable<IBasicValue<T>> components, string formula)
            where T : struct;

        Dictionary<string, IDerivedValue<int>> GetEntityStandardBlock(IEnumerable<IBasicValue<int>> components);

        Dictionary<string, IDerivedValue<int>> GetItemStandardBlock(IEnumerable<IBasicValue<float>> components);

        bool AddFormula(string name, string formula);

        Color GetColour(string name);
    }
}