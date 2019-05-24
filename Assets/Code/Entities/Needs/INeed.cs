namespace JoyLib.Code.Entities.Needs
{
    public interface INeed
    {
        bool FindFulfilmentObject(Entity actor);

        bool Interact(Entity user, JoyObject obj);

        INeed Copy();

        INeed Randomise();

        bool Initialise(string nameRef, int decayRef, int decayCounterRef, bool doesDecayRef, int priorityRef, int happinessThresholdRef,
            int valueRef, int maxValueRef, int averageForDayRef, int averageForWeekRef);

        bool Tick();

        void Fulfill(int value);

        void Decay(int value);

        string Name
        {
            get;
        }

        int Priority
        {
            get;
        }

        bool ContributingHappiness
        {
            get;
        }

        int Value
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
