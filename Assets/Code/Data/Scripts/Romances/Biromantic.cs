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

        public override bool Compatible(Entity me, Entity them, IRelationship[] relationships)
        {
            foreach (IRelationship relationship in relationships)
            {
                if(relationship.GetRelationshipValue(me.GUID, them.GUID) < RomanceThreshold)
                {
                    return false;
                }

                return me.Sentient == them.Sentient;
            }
            return false;
        }
    }
}