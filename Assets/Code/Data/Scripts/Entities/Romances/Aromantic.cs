using System.Collections.Generic;
using JoyLib.Code.Entities.Relationships;
using JoyLib.Code.Entities.Romance;

namespace JoyLib.Code.Entities.Romances
{
    public class Aromantic : AbstractRomance
    {

        public override string Name => "aromantic";

        public override bool DecaysNeed => true;

        public override bool WillRomance(IEntity me, IEntity them, IEnumerable<IRelationship> relationships)
        {
            return false;
        }
        public override bool Compatible(IEntity me, IEntity them)
        {
            return false;
        }
    }
}