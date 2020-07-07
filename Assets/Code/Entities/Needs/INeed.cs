using JoyLib.Code.Entities.Statistics;

namespace JoyLib.Code.Entities.Needs
{
    public interface INeed : IBasicValue
    {

        bool FindFulfilmentObject(Entity actor);

        bool Interact(Entity user, JoyObject obj);

        INeed Copy();

        INeed Randomise();

        bool Tick();

        int Fulfill(int value);

        int Decay(int value);

        //Name and Value come from IBasicValue

        int Priority
        {
            get;
        }

        bool ContributingHappiness
        {
            get;
        }

        int AverageForDay
        {
            get;
        }

        int AverageForWeek
        {
            get;
        }

        int AverageForMonth
        {
            get;
        }
    }
}
