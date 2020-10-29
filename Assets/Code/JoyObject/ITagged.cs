﻿using System.Collections.Generic;

namespace JoyLib.Code
{
    public interface ITagged
    {
        List<string> Tags { get; }
        bool HasTag(string tag);
        bool AddTag(string tag);
        bool RemoveTag(string tag);
    }
}