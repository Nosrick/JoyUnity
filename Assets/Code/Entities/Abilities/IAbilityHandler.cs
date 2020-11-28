using System.Collections.Generic;

namespace JoyLib.Code.Entities.Abilities
{
    public interface IAbilityHandler
    {
        IAbility GetAbility(string nameRef);
    }
}