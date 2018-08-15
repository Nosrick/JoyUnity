using JoyLib.Code.Loaders;
using System.Collections.Generic;

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

        public static CultureType Get(string name)
        {
            if(s_Cultures.ContainsKey(name))
            {
                return s_Cultures[name];
            }

            return null;
        }
    }
}
