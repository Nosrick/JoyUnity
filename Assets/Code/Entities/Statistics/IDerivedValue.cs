namespace JoyLib.Code.Entities.Statistics
{
    public interface IDerivedValue : IBasicValue
    {
        int Maximum
        {
            get;
        }

        int ModifyMaximum(int value);

        int SetMaximum(int value);
    }
}
