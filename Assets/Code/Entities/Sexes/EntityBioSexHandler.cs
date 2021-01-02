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
            this.Initialise();
        }

        protected void Initialise()
        {
            this.Load();
        }

        protected bool Load()
        {
            if(!(this.m_Sexes is null))
            {
                return true;
            }

            this.m_Sexes = new Dictionary<string, IBioSex>();
            
            IEnumerable<IBioSex> sexes = ScriptingEngine.instance.FetchAndInitialiseChildren<IBioSex>();
            foreach(IBioSex sex in sexes)
            {
                this.m_Sexes.Add(sex.Name, sex);
            }

            return true;
        }

        public IBioSex Get(string name)
        {
            if(this.m_Sexes.Any(sex => sex.Key.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                return this.m_Sexes.First(sex => sex.Key.Equals(name, StringComparison.OrdinalIgnoreCase)).Value;
            }
            return null;
        }

        public IBioSex[] Sexes
        {
            get
            {
                return this.m_Sexes.Values.ToArray();
            }
        }
    }
}
