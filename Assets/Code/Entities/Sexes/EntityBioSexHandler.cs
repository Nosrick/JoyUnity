using JoyLib.Code.Cultures;
using JoyLib.Code.Scripting;
using System;
using System.Collections.Generic;

namespace JoyLib.Code.Entities.Sexes
{
    public static class EntityBioSexHandler
    {
        private static Dictionary<string, Type> s_Sexes;

        public static bool Load(CultureType[] cultures)
        {
            if(s_Sexes != null)
            {
                return true;
            }

            s_Sexes = new Dictionary<string, Type>();

            foreach(CultureType culture in cultures)
            {
                foreach (string sex in culture.Sexes)
                {
                    if(s_Sexes.ContainsKey(sex) == false)
                    {
                        Type type = ScriptingEngine.FetchType(sex);
                        if(type != null)
                        {
                            s_Sexes.Add(sex, type);
                        }
                    }
                }
            }

            return true;
        }

        public static IBioSex Get(string name)
        {
            if(s_Sexes == null)
            {
                return null;
            }

            if(s_Sexes.ContainsKey(name))
            {
                IBioSex sex = (IBioSex)Activator.CreateInstance(s_Sexes[name]);
                return sex;
            }
            return null;
        }
    }
}
