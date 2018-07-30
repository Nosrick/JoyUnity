using System;

namespace JoyLib.Code.Entities
{
    public class EntityStatistic
    {
        public EntityStatistic(int value, int successThreshold)
        {
            Value = value;
            SuccessThreshold = successThreshold;
        }

        public void Modify(int value)
        {
            Value = Math.Max(1, Value + value);
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
    }
}
