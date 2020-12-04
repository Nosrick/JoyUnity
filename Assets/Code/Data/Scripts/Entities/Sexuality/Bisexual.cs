using JoyLib.Code.Entities.Relationships;
using System.Collections.Generic;
using System.Linq;

namespace JoyLib.Code.Entities.Sexuality
{
    public class Bisexual : AbstractSexuality
    {
        public override string Name
        {
            get
            {
                return "bisexual";
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

        public Bisexual()
        {
            AddTag("bi");
        }

        public override bool WillMateWith(IEntity me, IEntity them, IRelationship[] relationships)
        {
            int highestValue = relationships.Max(relationship => relationship.GetRelationshipValue(me.GUID, them.GUID));
            if(highestValue < MatingThreshold)
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
