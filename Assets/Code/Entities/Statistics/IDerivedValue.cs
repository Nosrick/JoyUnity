namespace JoyLib.Code.Entities.Statistics
{
    public interface IDerivedValue<T> : IBasicValue<T> where T : struct
    {
        T Maximum
        {
            get;
            set;
        }

        T SetValue(string data);
        T SetMaximum(string data);
    }
}
