using JoyLib.Code.Collections;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Rollers;
using System;
using System.Collections.Generic;

namespace JoyLib.Code.Entities.Statistics
{
    public class ConcreteGrowingValue : IGrowingValue
    {
        public ConcreteGrowingValue(string name, int value, int threshold, float experience,
            int successThreshold, IRollable roller, NonUniqueDictionary<INeed, float> governingNeeds)
        {
            Name = name;
            Value = value;
            Threshold = threshold;
            Experience = experience;
            SuccessThreshold = successThreshold;
            Roller = roller;
            GoverningNeeds = governingNeeds;
        }

        protected NonUniqueDictionary<INeed, float> m_GoverningNeeds;

        public float Experience
        {
            get;
            protected set;
        }

        public NonUniqueDictionary<INeed, float> GoverningNeeds
        {
            get
            {
                return new NonUniqueDictionary<INeed, float>(m_GoverningNeeds);
            }

            protected set
            {
                m_GoverningNeeds = value;
            }
        }

        public int SuccessThreshold
        {
            get;
            protected set;
        }

        public IRollable Roller
        {
            get;
            protected set;
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

        public int Threshold
        {
            get;
            protected set;
        }

        public int AddExperience(float value)
        {
            Experience += value;
            while(Experience >= Threshold)
            {
                Experience -= Threshold;
                Value += 1;
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
    }
}
