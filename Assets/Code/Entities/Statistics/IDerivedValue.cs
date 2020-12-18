namespace JoyLib.Code.Entities.Statistics
{
    public interface IDerivedValue : IBasicValue<int>
    {
        int Maximum
        {
            get;
            set;
        }

        int SetValue(string data);
        int SetMaximum(string data);
    }
}
