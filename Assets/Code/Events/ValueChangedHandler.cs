using System;

namespace JoyLib.Code.Events
{
    public delegate void ValueChangedEventHandler(object sender, ValueChangedEventArgs args);
    
    public class ValueChangedEventArgs : EventArgs
    {
        public int NewValue { get; set; }
        public int Delta { get; set; }
    }
}