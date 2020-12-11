namespace JoyLib.Code.Entities.Statistics
{
    public class ConcreteBasicIntValue : IBasicValue<int>
    {
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
        
        public ConcreteBasicIntValue()
        {}
        
        public ConcreteBasicIntValue(string name, int value)
        {
            Name = name;
            Value = value;
        }
    }
}
