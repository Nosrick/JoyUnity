using JoyLib.Code.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JoyLib.Code.Entities.Abilities
{
    public class AbilityHandler : IAbilityHandler
    {
        protected List<IAbility> Abilities { get; set; }

        public AbilityHandler()
        {
            Initialise();
        }

        private void Load()
        {
            Abilities = new List<IAbility>();
            Abilities.AddRange(ScriptingEngine.instance.FetchAndInitialiseChildren<IAbility>());
        }

        public bool Initialise()
        {
            if (Abilities is null)
            {
                Load();
            }
                
            return true;
        }

        public IAbility GetAbility(string nameRef)
        {
            Initialise();
            
            if(Abilities.Any(x => x.InternalName.Equals(nameRef, StringComparison.OrdinalIgnoreCase)))
            {
                return Abilities.First(x => x.InternalName.Equals(nameRef, StringComparison.OrdinalIgnoreCase));
            }
            throw new InvalidOperationException("Could not find IAbility with name " + nameRef);
        }
    }
}
