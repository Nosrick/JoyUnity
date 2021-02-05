using System;
using Sirenix.OdinSerializer;

namespace JoyLib.Code.Entities.Statistics
{
    [Serializable]
    public class ConcreteBasicFloatValue : IBasicValue<float>
    {
        [OdinSerialize]
        public string Name { get; protected set; }
        [OdinSerialize]
        public float Value { get; protected set; }

        public ConcreteBasicFloatValue()
        {
        }

        public ConcreteBasicFloatValue(string name, float value)
        {
            this.Name = name;
            this.Value = value;
        }
        public float ModifyValue(float value)
        {
            this.Value += value;
            return this.Value;
        }

        public float SetValue(float value)
        {
            this.Value = value;
            return this.Value;
        }
    }
}