using System;
using UnityEngine;

namespace JoyLib.Code.Unity.GUI
{
    public interface IValueContainer
    {
        int DecreaseValue(int value = 1);
        int IncreaseValue(int value = 1);
        
        int Value { get; set; }

        event EventHandler ValueChanged;
    }
}