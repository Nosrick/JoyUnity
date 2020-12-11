namespace JoyLib.Code.Entities.Statistics
{
    public class ConcreteBasicFloatValue : IBasicValue<float>
    {
        public string Name { get; set; }
        public float Value { get; set; }

        public ConcreteBasicFloatValue()
        {
        }

        public ConcreteBasicFloatValue(string name, float value)
        {
            this.Name = name;
            this.Value = value;
        }
    }
}