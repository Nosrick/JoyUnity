﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using JoyLib.Code.Entities.Relationships;

namespace JoyLib.Code.Entities.Sexuality
{
    public abstract class AbstractSexuality : ISexuality
    {
        public virtual string Name => throw new NotImplementedException("Someone forgot to override Name in " + this.GetType().Name);

        public virtual bool DecaysNeed => throw new NotImplementedException("Someone forgot to override DecaysNeed in " + this.GetType().Name);

        public virtual int MatingThreshold { get; set; }
        
        public List<string> Tags { get; protected set; }

        public AbstractSexuality()
        {
            Tags = new List<string>();
        }
        
        public bool HasTag(string tag)
        {
            return Tags.Any(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase));
        }

        public bool AddTag(string tag)
        {
            if (HasTag(tag))
            {
                return false;
            }
            Tags.Add(tag);
            return true;
        }

        public bool RemoveTag(string tag)
        {
            if (HasTag(tag))
            {
                Tags.Remove(tag);
                return true;
            }

            return false;
        }

        public abstract bool WillMateWith(IEntity me, IEntity them, IRelationship[] relationships);
        public abstract bool Compatible(IEntity me, IEntity them);
    }
}
