using JoyLib.Code.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JoyLib.Code.Entities.Abilities
{
    public static class AbilityHandler
    {
        private static List<IAbility> s_Abilities;

        private static void Load()
        {
            s_Abilities = new List<IAbility>();
            Type[] types = ScriptingEngine.FetchTypeAndChildren("AbstractAbility");
            foreach (Type type in types)
            {
                if (type.IsAbstract == false && type.IsInterface == false)
                {
                    s_Abilities.Add((IAbility)Activator.CreateInstance(type));
                }
            }
        }

        public static bool Initialise()
        {
            if (s_Abilities != null)
            {
                return true;
            }
                
            Load();
            return true;
        }

        public static IAbility GetAbility(string nameRef)
        {
            if(s_Abilities.Any(x => x.InternalName.Equals(nameRef)))
            {
                return s_Abilities.First(x => x.InternalName.Equals(nameRef));
            }
            throw new InvalidOperationException("Could not find IAbility with name " + nameRef);
        }
    }
}
