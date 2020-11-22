using JoyLib.Code.Entities.Relationships;

namespace JoyLib.Code.Entities.Sexuality
{
    public interface ISexuality : ITagged
    {
        bool WillMateWith(Entity me, Entity them, IRelationship[] relationships);

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
