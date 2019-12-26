using JoyLib.Code.Cultures;
using JoyLib.Code.Scripting;
using System;
using System.Collections.Generic;

namespace JoyLib.Code.Entities.Sexes
{
    public class EntityBioSexHandler
    {
        private static readonly Lazy<EntityBioSexHandler> lazy = new Lazy<EntityBioSexHandler>(() => new EntityBioSexHandler());

        public static EntityBioSexHandler instance => lazy.Value;

        private Dictionary<string, Type> m_Sexes;

        public bool Load(CultureType[] cultures)
        {
            if(m_Sexes != null)
            {
                return true;
            }

            m_Sexes = new Dictionary<string, Type>();

            foreach(CultureType culture in cultures)
            {
                foreach (string sex in culture.Sexes)
                {
                    if(m_Sexes.ContainsKey(sex) == false)
                    {
                        Type type = ScriptingEngine.FetchType(sex);
                        if(type != null)
                        {
                            m_Sexes.Add(sex, type);
                        }
                    }
                }
            }

            return true;
        }

        public IBioSex Get(string name)
        {
            if(m_Sexes == null)
            {
                return null;
            }

            if(m_Sexes.ContainsKey(name))
            {
                IBioSex sex = (IBioSex)Activator.CreateInstance(m_Sexes[name]);
                return sex;
            }
            return null;
        }
    }
}
