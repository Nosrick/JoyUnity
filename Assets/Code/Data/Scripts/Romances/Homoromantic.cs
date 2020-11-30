using System;
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

        public override bool Compatible(IEntity me, IEntity them, IRelationship[] relationships)
        {
            foreach (IRelationship relationship in relationships)
            {
                if(relationship.GetRelationshipValue(me.GUID, them.GUID) < RomanceThreshold)
                {
                    return false;
                }

                if(me.Gender.Name.Equals(them.Gender.Name, StringComparison.OrdinalIgnoreCase) == false)
                {
                    return false;
                }

                return me.Sentient == them.Sentient;
            }
            return false;
        }
    }
}