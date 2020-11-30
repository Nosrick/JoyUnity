using JoyLib.Code.Entities.Relationships;
using System.Collections.Generic;

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
            foreach (IRelationship relationship in relationships)
            {
                if (relationship.GetRelationshipValue(me.GUID, them.GUID) > MatingThreshold 
                    && me.Sentient == them.Sentient)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
