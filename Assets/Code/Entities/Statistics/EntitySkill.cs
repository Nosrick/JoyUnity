using System;
using JoyLib.Code.Collections;
using JoyLib.Code.Entities.Needs;

namespace JoyLib.Code.Entities.Statistics
{
    public class EntitySkill : IEntitySkill
    {
        protected const int MAXIMUM_XP = 100;

        public EntitySkill()
        {
        }
        
        public EntitySkill(
            string name, 
            int value, 
            int successThreshold, 
            NonUniqueDictionary<INeed, float> governingNeeds)
        {
            Name = name;
            Value = value;
            SuccessThreshold = successThreshold;
            GoverningNeeds = governingNeeds;
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
            set;
        }

        public int Value
        {
            get;
            set;
        }

        public int SuccessThreshold
        {
            get;
            set;
        }

        public NonUniqueDictionary<INeed, float> GoverningNeeds
        {
            get;
            protected set;
        }
    }
}