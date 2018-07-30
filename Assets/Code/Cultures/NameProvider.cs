using JoyLib.Code.Entities;
using JoyLib.Code.Loaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JoyLib.Code.Cultures
{
    public static class NameProvider
    {
        private static Dictionary<string, CultureType> s_Cultures;

        public static void Initialise()
        {
            if (s_Cultures == null)
            {
                s_Cultures = CultureLoader.LoadCultures();
            }
        }

        public static string GetRandomName(string creatureType, Sex sex)
        {
            if (s_Cultures.ContainsKey(creatureType))
            {
                return s_Cultures[creatureType].GetRandomName(sex);
            }

            return "Alex";
        }
    }
}
