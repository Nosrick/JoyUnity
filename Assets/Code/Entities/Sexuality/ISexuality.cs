using JoyLib.Code.Entities.Relationships;

namespace JoyLib.Code.Entities.Sexuality
{
    public interface ISexuality : ITagged
    {
        bool WillMateWith(IEntity me, IEntity them, IRelationship[] relationships);
        bool Compatible(IEntity me, IEntity them);

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
