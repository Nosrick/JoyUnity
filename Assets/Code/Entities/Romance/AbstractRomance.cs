using System;
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

        public List<string> Tags { get; protected set; }

        public AbstractRomance()
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

        public virtual bool Compatible(Entity me, Entity them, IRelationship[] relationships)
        {
            throw new NotImplementedException("Someone forgot to override Compatible in " + this.GetType().Name);
        }
    }
}