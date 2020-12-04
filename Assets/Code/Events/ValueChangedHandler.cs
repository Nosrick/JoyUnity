﻿using System;

namespace JoyLib.Code.Events
{
    public delegate void ValueChangedEventHandler(object sender, ValueChangedEventArgs args);
    
    public class ValueChangedEventArgs : EventArgs
    {
        public int Delta { get; set; }
    }
}