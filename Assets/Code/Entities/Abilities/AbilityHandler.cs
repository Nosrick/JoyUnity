using JoyLib.Code.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JoyLib.Code.Entities.Abilities
{
    public class AbilityHandler
    {
        private static readonly Lazy<AbilityHandler> lazy = new Lazy<AbilityHandler>(() => new AbilityHandler());

        public static AbilityHandler instance => lazy.Value;

        private List<IAbility> m_Abilities;

        private void Load()
        {
            m_Abilities = new List<IAbility>();
            m_Abilities.AddRange(ScriptingEngine.instance.FetchAndInitialiseChildren<IAbility>());
        }

        public bool Initialise()
        {
            if (m_Abilities is null)
            {
                Load();
            }
                
            return true;
        }

        public IAbility GetAbility(string nameRef)
        {
            Initialise();
            
            if(m_Abilities.Any(x => x.InternalName.Equals(nameRef, StringComparison.OrdinalIgnoreCase)))
            {
                return m_Abilities.First(x => x.InternalName.Equals(nameRef, StringComparison.OrdinalIgnoreCase));
            }
            throw new InvalidOperationException("Could not find IAbility with name " + nameRef);
        }
    }
}
