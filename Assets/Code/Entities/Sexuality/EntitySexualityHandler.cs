using JoyLib.Code.Cultures;
using JoyLib.Code.Scripting;
using System;
using System.Collections.Generic;

namespace JoyLib.Code.Entities.Sexuality
{
    public static class EntitySexualityHandler
    {
        private static Dictionary<string, Type> s_Sexualities;

        public static bool Load(CultureType[] cultures)
        {
            if(s_Sexualities != null)
            {
                return true;
            }

            s_Sexualities = new Dictionary<string, Type>();

            foreach(CultureType culture in cultures)
            {
                foreach(string sexuality in culture.Sexualities)
                {
                    if(s_Sexualities.ContainsKey(sexuality) == false)
                    {
                        Type type = ScriptingEngine.FetchType(sexuality);
                        if(type != null)
                        {
                            s_Sexualities.Add(sexuality, type);
                        }
                    }
                }
            }
            
            return true;
        }

        public static ISexuality Get(string sexuality)
        {
            if(s_Sexualities == null)
            {
                return null;
            }

            if(s_Sexualities.ContainsKey(sexuality))
            {
                ISexuality returnSexuality = (ISexuality)Activator.CreateInstance(s_Sexualities[sexuality]);
                return returnSexuality;
            }
            return null;
        }
    }
}
