namespace JoyLib.Code.Entities.Statistics
{
    public interface IBasicValue<T> where T : struct 
    {
        string Name
        {
            get;
            set;
        }

        T Value
        {
            get;
            set;
        }
    }
}
