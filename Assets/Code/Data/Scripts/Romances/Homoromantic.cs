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

        public override bool Compatible(Entity me, Entity them, IRelationship[] relationships)
        {
            foreach (IRelationship relationship in relationships)
            {
                if(relationship.GetRelationshipValue(me.GUID, them.GUID) < RomanceThreshold)
                {
                    return false;
                }

                if(me.Sex.CanBirth != them.Sex.CanBirth)
                {
                    return false;
                }

                return me.Sentient == them.Sentient;
            }
            return false;
        }
    }
}