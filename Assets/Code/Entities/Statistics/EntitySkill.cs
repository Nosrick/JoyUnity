using JoyLib.Code.Collections;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Rollers;
using System;

namespace JoyLib.Code.Entities.Statistics
{
    public class EntitySkill : IGrowingValue
    {
        protected const int MAXIMUM_XP = 100;

        public EntitySkill(string name, int value, int successThreshold, float experience, NonUniqueDictionary<INeed, float> governingNeeds,
            IRollable rollable)
        {
            Name = name;
            Value = value;
            SuccessThreshold = successThreshold;
            Experience = experience;
            GoverningNeeds = governingNeeds;
            Roller = rollable;

            Threshold = MAXIMUM_XP;
        }

        public int AddExperience(float value)
        {
            Experience += value;
            foreach (Tuple<INeed, float> pair in GoverningNeeds)
            {
                if (pair.Item1.ContributingHappiness)
                {
                    Experience += pair.Item2 * value;
                }
            }
            while (Experience / MAXIMUM_XP >= 1)
            {
                Value += 1;
                Experience -= MAXIMUM_XP;
            }
            return Value;
        }

        public int ModifyValue(int value)
        {
            Value = Math.Max(0, Value + value);
            return Value;
        }

        public int SetValue(int value)
        {
            Value = Math.Max(0, value);
            return Value;
        }

        public string Name
        {
            get;
            protected set;
        }

        public int Value
        {
            get;
            protected set;
        }

        public int SuccessThreshold
        {
            get;
            protected set;
        }

        public float Experience
        {
            get;
            protected set;
        }

        public NonUniqueDictionary<INeed, float> GoverningNeeds
        {
            get;
            protected set;
        }

        public IRollable Roller
        {
            get;
            protected set;
        }

        public int Threshold
        {
            get;
            protected set;
        }
    }
}