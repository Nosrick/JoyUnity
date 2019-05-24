namespace JoyLib.Code.Entities.Sexuality
{
    public class Asexual : AbstractSexuality
    {
        public override string Name
        {
            get
            {
                return "Asexual";
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
                return 0;
            }
        }

        public override bool FindMate(Entity me)
        {
            return false;
        }

        public override bool WillMateWith(Entity me, Entity them)
        {
            return false;
        }
    }
}
