namespace JoyLib.Code.Entities.Sexuality
{
    public interface ISexuality
    {
        bool WillMateWith(Entity me, Entity them);

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
