using JoyLib.Code.Collections;
using JoyLib.Code.Entities.Needs;

namespace JoyLib.Code.Entities.Statistics
{
    public interface IGrowingValue : IRollableValue
    {
        float Experience
        {
            get;
        }

        int Threshold
        {
            get;
        }

        NonUniqueDictionary<INeed, float> GoverningNeeds
        {
            get;
        }

        int AddExperience(float value);
    }
}
