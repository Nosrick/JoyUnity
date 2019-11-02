using JoyLib.Code.Helpers;
using JoyLib.Code.Loaders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JoyLib.Code.Cultures
{
    public static class CultureHandler
    {
        private static Dictionary<string, CultureType> s_Cultures;

        public static void Initialise()
        {
            s_Cultures = new Dictionary<string, CultureType>();

            s_Cultures = CultureLoader.LoadCultures();
        }

        public static CultureType GetByCultureName(string name)
        {
            if(s_Cultures.ContainsKey(name))
            {
                return s_Cultures[name];
            }

            return null;
        }

        public static List<CultureType> GetByCreatureType(string type)
        {
            try
            {
                Dictionary<string, CultureType> cultures = s_Cultures.Where(culture => culture.Value.Inhabitants.Contains(type.ToLowerInvariant())).ToDictionary(pair => pair.Key, pair => pair.Value);
                return cultures.Values.ToList();
            }
            catch(Exception e)
            {
                ActionLog.WriteToLog("Could not find a culture for creature type " + type);
                return new List<CultureType>();
            }
        }

        public static CultureType[] Cultures
        {
            get
            {
                return s_Cultures.Values.ToArray();
            }
        }
    }
}
