using System;
using System.Linq;
using JoyLib.Code.Collections;
using JoyLib.Code.Rollers;

namespace JoyLib.Code.Entities.Needs
{
    public class Confidence : AbstractNeed
    {
        public override string Name => "confidence";

        protected const int DECAY_MIN = 4;
        protected const int DECAY_MAX = 128;

        protected const int PRIORITY_MIN = 4;
        protected const int PRIORITY_MAX = 12;

        protected const int HAPPINESS_THRESHOLD_MIN = 8;
        protected const int HAPPINESS_THRESHOLD_MAX = 24;

        protected const int MAX_VALUE_MIN = HAPPINESS_THRESHOLD_MAX;
        protected const int MAX_VALUE_MAX = MAX_VALUE_MIN * 4;

        public Confidence()
            : base(
                0,
                1,
                true,
                1,
                1,
                1,
                1,
                new string[0])
        {
        }
        
        public Confidence(
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

        //This is an aggregate need of all other needs
        //So go for your lowest need
        public override bool FindFulfilmentObject(Entity actor)
        {
            BasicValueContainer<INeed> needs = actor.Needs;

            INeed chosenNeed = null;
            int bestMatch = int.MaxValue;
            foreach (INeed need in needs)
            {
                if (need.ContributingHappiness == false && need.Value < bestMatch)
                {
                    bestMatch = need.Value;
                    chosenNeed = need;
                }
            }

            //This means all of the needs are contributing happiness
            if (chosenNeed is null)
            {
                return true;
            }

            return chosenNeed.FindFulfilmentObject(actor);
        }

        public override bool Interact(Entity actor, JoyObject obj)
        {
            return false;
        }

        public override INeed Copy()
        {
            return new Confidence(
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
            
            return new Confidence(
                decay,
                decayCounter,
                true,
                priority,
                happinessThreshold,
                value,
                maxValue);
        }

        public override bool Tick(Entity actor)
        {
            if (this.m_DecayCounter == 0 && this.m_DoesDecay)
            {
                BasicValueContainer<INeed> needs = actor.Needs;

                int average = (int) Math.Ceiling(
                    needs.Where(need => need.ContributingHappiness)
                        .Average(need => need.Value));

                this.Fulfill(average);
                base.Tick(actor);
                return true;
            }
            
            base.Tick(actor);
            return false;
        }
    }
}