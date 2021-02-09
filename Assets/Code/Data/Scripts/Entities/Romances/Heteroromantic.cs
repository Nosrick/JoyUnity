using System;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Entities.Relationships;
using JoyLib.Code.Entities.Romance;

namespace JoyLib.Code.Entities.Romances
{
    public class Heteroromantic : AbstractRomance
    {
        public override string Name => "heteroromantic";

        public override bool DecaysNeed => true;

        public Heteroromantic()
        {
            this.AddTag("hetero");
        }

        public override bool WillRomance(IEntity me, IEntity them, IEnumerable<IRelationship> relationships)
        {
            if (relationships.Any() == false)
            {
                return false;
            }
            
            int highestValue = relationships.Max(relationship => relationship.GetRelationshipValue(me.Guid, them.Guid));
            if(highestValue < this.RomanceThreshold || me.Gender.Name.Equals(them.Gender.Name, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return me.Sentient == them.Sentient;
        }
        public override bool Compatible(IEntity me, IEntity them)
        {
            return me.Sentient == them.Sentient
                   && me.Gender.Name.Equals(them.Gender.Name, StringComparison.OrdinalIgnoreCase) == false;
        }
    }
}