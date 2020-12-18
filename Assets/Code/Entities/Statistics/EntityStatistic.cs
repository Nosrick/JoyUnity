using System;

namespace JoyLib.Code.Entities.Statistics
{
    public class EntityStatistic : IRollableValue<int>
    {
        public const string STRENGTH    =   "strength";
        public const string AGILITY     =   "agility";
        public const string ENDURANCE   =   "endurance";

        public const string INTELLECT   =   "intellect";
        public const string CUNNING     =   "cunning";
        public const string FOCUS       =   "focus";

        public const string PERSONALITY =   "personality";
        public const string SUAVITY     =   "suavity";
        public const string WIT         =   "wit";

        public static readonly string[] NAMES = new string[] { STRENGTH, AGILITY, ENDURANCE,
                                                    INTELLECT, CUNNING, FOCUS,
                                                    PERSONALITY, SUAVITY, WIT };

        public EntityStatistic()
        {
        }
        
        public EntityStatistic(string name, int value, int successThreshold)
        {
            Name = name;
            Value = value;
            SuccessThreshold = successThreshold;
        }

        public int ModifyValue(int value)
        {
            Value = Math.Max(1, Value + value);
            return Value;
        }

        public int SetValue(int value)
        {
            Value = Math.Max(1, value);
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
    }
}
