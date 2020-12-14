using System;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Scripting;

namespace JoyLib.Code.Entities.Sexes
{
    public class EntityBioSexHandler : IEntityBioSexHandler
    {
        protected Dictionary<string, IBioSex> m_Sexes;

        public EntityBioSexHandler()
        {
            Initialise();
        }

        protected void Initialise()
        {
            Load();
        }

        protected bool Load()
        {
            if(!(m_Sexes is null))
            {
                return true;
            }

            m_Sexes = new Dictionary<string, IBioSex>();
            
            IEnumerable<IBioSex> sexes = ScriptingEngine.instance.FetchAndInitialiseChildren<IBioSex>();
            foreach(IBioSex sex in sexes)
            {
                m_Sexes.Add(sex.Name, sex);
            }

            return true;
        }

        public IBioSex Get(string name)
        {
            if(m_Sexes.Any(sex => sex.Key.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                return m_Sexes.First(sex => sex.Key.Equals(name, StringComparison.OrdinalIgnoreCase)).Value;
            }
            return null;
        }

        public IBioSex[] Sexes
        {
            get
            {
                return m_Sexes.Values.ToArray();
            }
        }
    }
}
