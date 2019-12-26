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
                return "Bisexual";
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

        public override bool WillMateWith(Entity me, Entity them)
        {

            List<long> participants = new List<long>() { me.GUID, them.GUID };
            List<string> tags = new List<string>() { "sexual" };
            List<IRelationship> relationships = EntityRelationshipHandler.Get(participants.ToArray(), tags.ToArray());

            foreach (IRelationship relationship in relationships)
            {
                if (relationship.GetRelationshipValue(me.GUID, them.GUID) > MatingThreshold && me.Sentient == them.Sentient)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
