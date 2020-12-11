using System.Collections.Generic;
using JoyLib.Code.Entities.Abilities;
using JoyLib.Code.Entities.Statistics;

namespace JoyLib.Code.Rollers
{
    public interface IRollable
    {
        int Roll(
            IEnumerable<IRollableValue<int>> statistics, 
            IEnumerable<IRollableValue<int>> skills,
            IEnumerable<IAbility> modifiers,  
            params string[] tags);
    }
}
