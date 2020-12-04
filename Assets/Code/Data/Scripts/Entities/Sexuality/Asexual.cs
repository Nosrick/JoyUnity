using JoyLib.Code.Entities.Relationships;

namespace JoyLib.Code.Entities.Sexuality
{
    public class Asexual : AbstractSexuality
    {
        public override string Name
        {
            get
            {
                return "asexual";
            }
        }

        public override bool DecaysNeed
        {
            get
            {
                return false;
            }
        }

        public override int MatingThreshold
        {
            get
            {
                return int.MaxValue;
            }
        }

        public override bool WillMateWith(IEntity me, IEntity them, IRelationship[] relationships)
        {
            return false;
        }

        public override bool Compatible(IEntity me, IEntity them)
        {
            return false;
        }
    }
}
