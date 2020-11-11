using JoyLib.Code.Entities.Relationships;
using JoyLib.Code.Entities.Romance;

namespace JoyLib.Code.Entities.Romances
{
    public class Aromantic : AbstractRomance
    {
        public override string Name => "aromantic";

        public override bool DecaysNeed => true;

        public override bool Compatible(Entity me, Entity them, IRelationship[] relationships)
        {
            return false;
        }
    }
}