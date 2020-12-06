using System.Collections.Generic;
using JoyLib.Code.Collections;
using JoyLib.Code.Entities.Statistics;

namespace JoyLib.Code.Entities.Abilities
{
    public interface IAbilityHandler
    {
        IAbility GetAbility(string nameRef);
        IEnumerable<IAbility> GetAvailableAbilities(IEntity actor);

        IEnumerable<IAbility> GetAvailableAbilities(
            IEntityTemplate template,
            BasicValueContainer<IRollableValue> stats,
            BasicValueContainer<IGrowingValue> skills);
    }
}