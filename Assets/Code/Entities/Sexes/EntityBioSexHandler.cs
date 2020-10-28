using JoyLib.Code.Cultures;
using JoyLib.Code.Scripting;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace JoyLib.Code.Entities.Sexes
{
    public class EntityBioSexHandler : MonoBehaviour
    {
        protected Dictionary<string, IBioSex> m_Sexes;

        protected static CultureHandler CultureHandler
        {
            get;
            set;
        }

        public void Awake()
        {
            if(m_Sexes is null)
            {
                Initialise();
            }
        }

        private void Initialise()
        {
            
            CultureHandler = GameObject.Find("GameManager")
                                            .GetComponent<CultureHandler>();

            Load(CultureHandler.Cultures);
        }

        public bool Load(CultureType[] cultures)
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
