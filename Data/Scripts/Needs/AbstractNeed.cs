using System;
using UnityEngine;

namespace JoyLib.Code.Entities.Needs
{
    public abstract class AbstractNeed : INeed
    {
        protected string m_Name;

        //How quickly the need decays
        //The higher the number, the slower it decays
        protected int m_Decay;
        protected int m_DecayCounter;
        protected bool m_DoesDecay;

        //How much of an impacy this need has on overall happiness
        protected int m_Priority;

        //How high the value has to be before it contributes to happiness
        protected int m_HappinessThreshold;

        //Current value
        protected int m_Value;
        protected int m_MaximumValue;

        //Average for the day
        //Will be calculated by adding the value every hour, then dividing by 24 when the day is up
        protected int m_AverageForDay;

        //Average for the week
        //Calculated by adding value for the day every day, then dividing by 7 when the week is up
        protected int m_AverageForWeek;
        protected int m_AverageForMonth;

        //Bonus to health for having no diseases
        protected const int CLEAN_BONUS = 50;

        public AbstractNeed(string nameRef, int decayRef, int decayCounterRef, bool doesDecayRef, int priorityRef, int happinessThresholdRef,
            int valueRef, int maxValueRef, int averageForDayRef = 0, int averageForWeekRef = 0)
        {
            m_Name = nameRef;
            m_Decay = decayRef;
            m_DecayCounter = decayCounterRef;
            m_DoesDecay = doesDecayRef;

            m_Priority = priorityRef;

            m_HappinessThreshold = happinessThresholdRef;

            m_Value = valueRef;
            m_MaximumValue = maxValueRef;

            m_AverageForDay = averageForDayRef;
            m_AverageForWeek = averageForWeekRef;
        }

        public abstract bool FindFulfilmentObject(Entity actor);

        public abstract INeed Copy();

        public abstract INeed Randomise();

        //This will be called once per in-game minute
        public virtual bool Tick()
        {
            m_DecayCounter -= 1;
            if (m_DecayCounter == 0 && m_DoesDecay)
            {
                m_DecayCounter = m_Decay;
                Decay(1);
                return true;
            }
            return false;
        }

        public virtual int Fulfill(int value)
        {
            return ModifyValue(value);
        }

        public virtual int Decay(int value)
        {
            return ModifyValue(-value);
        }

        public int ModifyValue(int value)
        {
            m_Value = Math.Max(0, Math.Min(m_MaximumValue, m_Value + value));
            return m_Value;
        }

        public abstract bool Interact(Entity user, JoyObject obj);

        public int SetValue(int value)
        {
            m_Value = Math.Max(0, Math.Min(m_MaximumValue, value));
            return m_Value;
        }

        public string Name
        {
            get
            {
                return m_Name;
            }
            protected set
            {
                m_Name = value;
            }
        }

        public int Priority
        {
            get
            {
                return m_Priority;
            }
            protected set
            {
                m_Priority = value;
            }
        }

        public bool ContributingHappiness
        {
            get
            {
                return m_Value >= m_HappinessThreshold;
            }
        }

        public int Value
        {
            get
            {
                return m_Value;
            }
            protected set
            {
                m_Value = value;
            }
        }

        public int AverageForDay
        {
            get
            {
                return m_AverageForDay;
            }
            protected set
            {
                m_AverageForDay = value;
            }
        }

        public int AverageForWeek
        {
            get
            {
                return m_AverageForWeek;
            }
            protected set
            {
                m_AverageForWeek = value;
            }
        }

        public int AverageForMonth
        {
            get
            {
                return m_AverageForMonth;
            }
            protected set
            {
                m_AverageForMonth = value;
            }
        }

        /*
        public static Dictionary<NeedIndex, EntityNeed> GetBasicRandomisedNeeds()
        {
            Dictionary<NeedIndex, EntityNeed> needs = new Dictionary<NeedIndex, EntityNeed>();

            //Add tier 1 needs
            needs.Add(NeedIndex.Hunger, new EntityNeed(60, 200, true, RNG.Roll(5, 24),
                24, 12, 0, 0, "Hunger"));
            needs.Add(NeedIndex.Drink, new EntityNeed(50, 200, true, RNG.Roll(5, 20),
                20, 10, 0, 0, "Drink"));
            needs.Add(NeedIndex.Sleep, new EntityNeed(60, 200, true, 12, 24, 8, 0, 0, "Sleep"));
            needs.Add(NeedIndex.Sex, new EntityNeed(240, RNG.Roll(10, 240), true, RNG.Roll(50, 200),
                240, RNG.Roll(0, 240), 0, 0, "Sex"));

            return needs;
        }

        public static Dictionary<NeedIndex, EntityNeed> GetFullRandomisedNeeds()
        {
            Dictionary<NeedIndex, EntityNeed> needs = GetBasicRandomisedNeeds();

            //Add tier 2 needs
            //Health is an aggregate of food, drink and sleep, and gets a flat bonus of 50 for having no diseases
            int health = ((needs[NeedIndex.Hunger].value + needs[NeedIndex.Drink].value + needs[NeedIndex.Sleep].value) / 3) + CLEAN_BONUS;

            needs.Add(NeedIndex.Health, new EntityNeed(0, RNG.Roll(50, 150), false, health, 300, RNG.Roll(100, 200), 0, 0, "Health"));
            needs.Add(NeedIndex.Employment, new EntityNeed(RNG.Roll(200, 400), RNG.Roll(10, 150), true, 0, 300, RNG.Roll(50, 200), 0, 0, "Employment"));
            needs.Add(NeedIndex.Property, new EntityNeed(0, RNG.Roll(10, 150), false, 0, 300, RNG.Roll(50, 250), 0, 0, "Property"));

            //Add tier 3 needs
            needs.Add(NeedIndex.Friendship, new EntityNeed(RNG.Roll(24, 72), RNG.Roll(10, 150), true, 0, 300, RNG.Roll(10, 200), 0, 0, "Friendship"));
            needs.Add(NeedIndex.Family, new EntityNeed(0, RNG.Roll(10, 50), false, 0, 300, 100, 0, 0, "Family"));
            needs.Add(NeedIndex.Morality, new EntityNeed(0, RNG.Roll(0, 150), false, 150, 300, 150, 0, 0, "Morality"));

            //Add tier 4 needs
            needs.Add(NeedIndex.Respect, new EntityNeed(0, RNG.Roll(10, 150), false, 0, int.MaxValue, RNG.Roll(10, 200), 0, 0, "Respect"));
            int confidence = needs.Sum(x => x.Value.value) / needs.Count;
            needs.Add(NeedIndex.Confidence, new EntityNeed(RNG.Roll(24, 148), RNG.Roll(50, 150), true, confidence, 300, RNG.Roll(50, 200), 0, 0, "Confidence"));

            return needs;
        }
        */
    }
}
