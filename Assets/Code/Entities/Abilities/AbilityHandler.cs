using System;
using System.Collections.Generic;
using System.Linq;

namespace JoyLib.Code.Entities.Abilities
{
    public static class AbilityHandler
    {
        private static List<Ability> s_Abilities;

        private static void Load()
        {
            s_Abilities = new List<Ability>();
            List<Type> types = typeof(Ability).Assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(Ability))).ToList();
            foreach (Type type in types)
            {
                s_Abilities.Add((Ability)type.Assembly.CreateInstance(type.Assembly.GetName().Name));
            }
        }

        public static void Initialise()
        {
            if (s_Abilities != null)
                return;

            Load();
        }

        public static Ability GetAbility(string nameRef)
        {
            if(s_Abilities.Any(x => x.m_InternalName.Equals(nameRef)))
            {
                return s_Abilities.First(x => x.m_InternalName.Equals(nameRef));
            }
            return null;
        }
    }
}
