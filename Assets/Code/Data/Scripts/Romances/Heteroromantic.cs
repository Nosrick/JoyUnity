using System;
using System.Collections.Generic;
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
            AddTag("hetero");
        }

        public override bool Compatible(Entity me, Entity them, IRelationship[] relationships)
        {
            foreach (IRelationship relationship in relationships)
            {
                if(relationship.GetRelationshipValue(me.GUID, them.GUID) < RomanceThreshold)
                {
                    return false;
                }

                if(me.Gender.Name.Equals(them.Gender.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                return me.Sentient == them.Sentient;
            }
            return false;
        }
    }
}