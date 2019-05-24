//This identifies whether a creature can give birth or not, etc
namespace JoyLib.Code.Entities.Sexes
{
    public interface IBioSex
    {
        bool CanBirth
        {
            get;
        }

        string Name
        {
            get;
        }

        Entity CreateChild(Entity[] parents);
    }
}
