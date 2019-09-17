using JoyLib.Code.Rollers;

namespace JoyLib.Code.Entities.Statistics
{
    public interface IRollableValue : IBasicValue
    {
        int SuccessThreshold
        {
            get;
        }

        IRollable Roller
        {
            get;
        }
    }
}
