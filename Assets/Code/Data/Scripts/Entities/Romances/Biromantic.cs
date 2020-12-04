using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Entities.Relationships;
using JoyLib.Code.Entities.Romance;

namespace JoyLib.Code.Entities.Romances
{
    public class Biromantic : AbstractRomance
    {
        public override string Name => "biromantic";

        public override bool DecaysNeed => true;

        public Biromantic()
        {
            AddTag("bi");
        }

        public override bool WillRomance(IEntity me, IEntity them, IEnumerable<IRelationship> relationships)
        {
            int highestValue = relationships.Max(relationship => relationship.GetRelationshipValue(me.GUID, them.GUID));
            if(highestValue < RomanceThreshold)
            {
                return false;
            }

            return me.Sentient == them.Sentient;
        }
        public override bool Compatible(IEntity me, IEntity them)
        {
            return me.Sentient == them.Sentient;
        }
    }
}