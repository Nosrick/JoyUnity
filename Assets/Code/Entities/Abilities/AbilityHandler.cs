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
            Type[] types = ScriptingEngine.FetchTypeAndChildren("AbstractAbility");
            foreach (Type type in types)
            {
                if (type.IsAbstract == false && type.IsInterface == false)
                {
                    m_Abilities.Add((IAbility)Activator.CreateInstance(type));
                }
            }
        }

        public bool Initialise()
        {
            if (m_Abilities != null)
            {
                return true;
            }
                
            Load();
            return true;
        }

        public IAbility GetAbility(string nameRef)
        {
            if(m_Abilities.Any(x => x.InternalName.Equals(nameRef)))
            {
                return m_Abilities.First(x => x.InternalName.Equals(nameRef));
            }
            throw new InvalidOperationException("Could not find IAbility with name " + nameRef);
        }
    }
}
