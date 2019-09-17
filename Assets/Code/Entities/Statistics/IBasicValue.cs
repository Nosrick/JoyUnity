namespace JoyLib.Code.Entities.Statistics
{
    public interface IBasicValue
    {
        string Name
        {
            get;
        }

        int Value
        {
            get;
        }

        int ModifyValue(int value);
        int SetValue(int value);
    }
}
