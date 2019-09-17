namespace JoyLib.Code.Entities.Sexuality
{
    public interface ISexuality
    {
        bool WillMateWith(Entity me, Entity them);

        bool FindMate(Entity me);

        string Name
        {
            get;
        }

        bool DecaysNeed
        {
            get;
        }

        int MatingThreshold
        {
            get;
            set;
        }
    }
}
