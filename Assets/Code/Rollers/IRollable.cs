using JoyLib.Code.Entities.Abilities;
using JoyLib.Code.Entities.Statistics;

namespace JoyLib.Code.Rollers
{
    public interface IRollable
    {
        int Roll(IRollableValue[] statistics, IGrowingValue[] skills, IAbility[] modifiers, params string[] tags);
    }
}
