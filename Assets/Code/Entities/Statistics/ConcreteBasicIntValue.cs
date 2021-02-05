using System;
using Sirenix.OdinSerializer;

namespace JoyLib.Code.Entities.Statistics
{
    [Serializable]
    public class ConcreteBasicIntValue : IBasicValue<int>
    {
        [OdinSerialize]
        public string Name
        {
            get;
            protected set;
        }

        [OdinSerialize]
        public int Value
        {
            get;
            protected set;
        }

        public ConcreteBasicIntValue()
        {}
        
        public ConcreteBasicIntValue(string name, int value)
        {
            this.Name = name;
            this.Value = value;
        }

        public int ModifyValue(int value)
        {
            this.Value += value;
            return this.Value;
        }

        public int SetValue(int value)
        {
            this.Value = value;
            return this.Value;
        }
    }
}
