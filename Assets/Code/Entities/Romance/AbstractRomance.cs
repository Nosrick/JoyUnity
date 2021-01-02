﻿using System;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Entities.Relationships;

namespace JoyLib.Code.Entities.Romance
{
    public abstract class AbstractRomance : IRomance
    {
        public virtual string Name => throw new NotImplementedException("Someone forgot to override Name in " + this.GetType().Name);

        public virtual bool DecaysNeed => throw new NotImplementedException("Someone forgot to override DecaysNeed in " + this.GetType().Name);

        public virtual int RomanceThreshold { get; set; }

        public virtual int BondingThreshold { get; set; }

        public IEnumerable<string> Tags
        {
            get => this.m_Tags;
            protected set => this.m_Tags = new List<string>(value);
        }

        protected List<string> m_Tags;

        public AbstractRomance()
        {
            this.Tags = new List<string>();
        }
        
        public bool HasTag(string tag)
        {
            return this.m_Tags.Any(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase));
        }

        public bool AddTag(string tag)
        {
            if (this.HasTag(tag))
            {
                return false;
            }
            this.m_Tags.Add(tag);
            return true;
        }

        public bool RemoveTag(string tag)
        {
            if (this.HasTag(tag))
            {
                this.m_Tags.Remove(tag);
                return true;
            }

            return false;
        }

        public abstract bool WillRomance(IEntity me, IEntity them, IEnumerable<IRelationship> relationships);
        public abstract bool Compatible(IEntity me, IEntity them);
    }
}