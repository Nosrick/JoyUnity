namespace JoyLib.Code.Entities.Statistics
{
    public class ConcreteBasicValue : IBasicValue
    {
        public ConcreteBasicValue(string name, int value)
        {
            Name = name;
            Value = value;
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

        public int ModifyValue(int value)
        {
            Value = Value + value;
            return Value;
        }

        public int SetValue(int value)
        {
            Value = value;
            return Value;
        }
    }
}
