using JoyLib.Code.Entities.Statistics;

namespace JoyLib.Code.Entities.Needs
{
    public interface INeed : IBasicValue
    {
        bool FindFulfilmentObject(Entity actor);

        bool Interact(Entity user, JoyObject obj);

        INeed Copy();

        INeed Randomise();

        bool Initialise(string nameRef, int decayRef, int decayCounterRef, bool doesDecayRef, int priorityRef, int happinessThresholdRef,
            int valueRef, int maxValueRef, int averageForDayRef, int averageForWeekRef);

        bool Tick();

        int Fulfill(int value);

        int Decay(int value);

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
