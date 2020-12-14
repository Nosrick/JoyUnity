﻿using JoyLib.Code.Entities.Jobs;

namespace JoyLib.Code.Events
{
    public delegate void JobChangedEventHandler(object sender, JobChangedEventArgs args);
    
    public class JobChangedEventArgs
    {
        public IJob NewJob { get; set; }
        public long GUID { get; set; }
    }
}