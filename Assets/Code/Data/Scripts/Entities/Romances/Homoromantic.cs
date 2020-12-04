using System;
using System.Linq;
using JoyLib.Code.Entities.Relationships;
using JoyLib.Code.Entities.Romance;

namespace JoyLib.Code.Entities.Romances
{
    public class Homoromantic : AbstractRomance
    {
        public override string Name => "homoromantic";

        public override bool DecaysNeed => true;

        public Homoromantic()
        {
            AddTag("homo");
        }

        public override bool WillRomance(IEntity me, IEntity them, IRelationship[] relationships)
        {
            int highestValue = relationships.Max(relationship => relationship.GetRelationshipValue(me.GUID, them.GUID));
            if(highestValue < RomanceThreshold 
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