using System;
using JoyLib.Code.Collections;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Rollers;

namespace JoyLib.Code.Entities.Statistics
{
    public class EntitySkill : IRollableValue<int>
    {
        protected const int MAXIMUM_XP = 100;

        public EntitySkill()
        {
            this.Roller = new StandardRoller();
        }
        
        public EntitySkill(
            string name, 
            int value, 
            int successThreshold, 
            NonUniqueDictionary<INeed, float> governingNeeds,
            IRollable rollable)
        {
            Name = name;
            Value = value;
            SuccessThreshold = successThreshold;
            GoverningNeeds = governingNeeds;
            Roller = rollable;
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

        public IRollable Roller
        {
            get;
            protected set;
        }
    }
}