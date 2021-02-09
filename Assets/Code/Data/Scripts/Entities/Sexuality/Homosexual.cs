using System;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Entities.Relationships;

namespace JoyLib.Code.Entities.Sexuality
{
    public class Homosexual : AbstractSexuality
    {
        public override string Name
        {
            get
            {
                return "homosexual";
            }
        }

        public override bool DecaysNeed
        {
            get
            {
                return true;
            }
        }

        public override int MatingThreshold
        {
            get
            {
                return 0;
            }
        }

        public Homosexual()
        {
            this.AddTag("homo");
        }

        public override bool WillMateWith(IEntity me, IEntity them, IEnumerable<IRelationship> relationships)
        {
            if (relationships.Any() == false)
            {
                return false;
            }
            int highestValue = relationships.Max(relationship => relationship.GetRelationshipValue(me.Guid, them.Guid));
            if(highestValue < this.MatingThreshold
               || me.Gender.Name.Equals(them.Gender.Name, StringComparison.OrdinalIgnoreCase) == false)
            {
                return false;
            }
            return me.Sentient == them.Sentient;
        }

        public override bool Compatible(IEntity me, IEntity them)
        {
            return me.Sentient == them.Sentient
                   && me.Gender.Name.Equals(them.Gender.Name);
        }
    }
}
