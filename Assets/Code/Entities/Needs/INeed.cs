namespace JoyLib.Code.Entities.Needs
{
    public interface INeed
    {
        bool FindFulfilmentObject(Entity actor);

        INeed Copy();

        INeed Randomise();

        bool Initialise(string nameRef, int decayRef, int decayCounterRef, bool doesDecayRef, int priorityRef, int happinessThresholdRef,
            int valueRef, int maxValueRef, int averageForDayRef, int averageForWeekRef);

        bool Tick();

        void Fulfill(int value);

        void Decay(int value);

        string GetName();

        int GetPriority();

        bool GetContributingHappiness();

        int GetValue();

        int GetAverageForDay();

        int GetAverageForWeek();

        int GetAverageForMonth();
    }
}
