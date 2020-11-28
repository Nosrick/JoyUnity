using JoyLib.Code.Cultures;
using JoyLib.Code.Scripting;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace JoyLib.Code.Entities.Sexes
{
    public class EntityBioSexHandler : IEntityBioSexHandler
    {
        protected Dictionary<string, IBioSex> m_Sexes;

        protected ICultureHandler CultureHandler
        {
            get;
            set;
        }

        public EntityBioSexHandler(ICultureHandler cultureHandler)
        {
            this.CultureHandler = cultureHandler;
            Initialise();
        }

        protected void Initialise()
        {
            Load(CultureHandler.Cultures);
        }

        protected bool Load(ICulture[] cultures)
        {
            if (CultureHandler is null)
            {
                Initialise();
            }
            
            if(!(m_Sexes is null))
            {
                return true;
            }

            m_Sexes = new Dictionary<string, IBioSex>();
            
            IBioSex[] sexes = ScriptingEngine.instance.FetchAndInitialiseChildren<IBioSex>();
            foreach(IBioSex sex in sexes)
            {
                m_Sexes.Add(sex.Name, sex);
            }

            return true;
        }

        public IBioSex Get(string name)
        {
            if (CultureHandler is null)
            {
                Initialise();
            }

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
                if(CultureHandler is null)
                {
                    Initialise();
                }
                return m_Sexes.Values.ToArray();
            }
        }
    }
}
