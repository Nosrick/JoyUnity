using JoyLib.Code.Rollers;

namespace JoyLib.Code.Entities.Needs
{
    public class Morality : AbstractNeed
    {
        public override string Name => "morality";

        protected const int DECAY_MIN = 4;
        protected const int DECAY_MAX = 128;

        protected const int PRIORITY_MIN = 0;
        protected const int PRIORITY_MAX = 12;

        protected const int HAPPINESS_THRESHOLD_MIN = 0;
        protected const int HAPPINESS_THRESHOLD_MAX = 24;

        protected const int MAX_VALUE_MIN = HAPPINESS_THRESHOLD_MAX;
        protected const int MAX_VALUE_MAX = MAX_VALUE_MIN * 4;
        
        public Morality()
        : base(
            1,
            1,
            true,
            1,
            1,
            1,
            1,
            new string[0])
        {
            
        }
        
        public Morality(
            int decayRef, 
            int decayCounterRef, 
            bool doesDecayRef, 
            int priorityRef, 
            int happinessThresholdRef, 
            int valueRef, 
            int maxValueRef,
            int averageForDayRef = 0, 
            int averageForWeekRef = 0) 
            : base(
                decayRef, 
                decayCounterRef, 
                doesDecayRef, 
                priorityRef,
                happinessThresholdRef,
                valueRef,
                maxValueRef,
                new string[0],
                averageForDayRef,
                averageForWeekRef)
        {
        }

        //Things to find for morality:
        //People to talk with
        //People to help
        //People to give to charitably
        public override bool FindFulfilmentObject(Entity actor)
        {
            return false;
        }

        public override bool Interact(Entity actor, IJoyObject obj)
        {
            return false;
        }

        public override INeed Copy()
        {
            return new Morality(
                this.m_Decay,
                this.m_DecayCounter,
                this.m_DoesDecay,
                this.m_Priority,
                this.m_HappinessThreshold,
                this.m_Value,
                this.m_MaximumValue,
                this.AverageForWeek);
        }

        public override INeed Randomise()
        {
            int decay = RNG.instance.Roll(DECAY_MIN, DECAY_MAX);
            int decayCounter = RNG.instance.Roll(0, DECAY_MAX);
            int priority = RNG.instance.Roll(PRIORITY_MIN, PRIORITY_MAX);
            int happinessThreshold = RNG.instance.Roll(HAPPINESS_THRESHOLD_MIN, HAPPINESS_THRESHOLD_MAX);
            int value = RNG.instance.Roll(0, HAPPINESS_THRESHOLD_MAX);
            int maxValue = RNG.instance.Roll(MAX_VALUE_MIN, MAX_VALUE_MAX);
            
            return new Morality(
                decay,
                decayCounter,
                true,
                priority,
                happinessThreshold,
                value,
                maxValue);
        }
    }
}