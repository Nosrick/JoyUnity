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
        private Dictionary<string, IBioSex> m_Sexes;

        public void Awake()
        {
            if(m_Sexes is null)
            {
                Initialise();
            }
        }

        private void Initialise()
        {
            
            CultureHandler cultureHandler = GameObject.Find("GameManager")
                                            .GetComponent<CultureHandler>();

            Load(cultureHandler.Cultures);
        }

        public bool Load(CultureType[] cultures)
        {
            if(m_Sexes != null)
            {
                return true;
            }

            m_Sexes = new Dictionary<string, IBioSex>();

            foreach(CultureType culture in cultures)
            {
                foreach (string sex in culture.Sexes)
                {
                    if(m_Sexes.ContainsKey(sex) == false)
                    {
                        Type type = ScriptingEngine.instance.FetchType(sex);
                        if(!(type is null))
                        {
                            IBioSex sexInstance = (IBioSex)Activator.CreateInstance(type);
                            m_Sexes.Add(sex, sexInstance);
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
                return m_Sexes[name];
            }
            return null;
        }

        public IBioSex[] Sexes
        {
            get
            {
                if(m_Sexes is null)
                {
                    Initialise();
                }
                return m_Sexes.Values.ToArray();
            }
        }
    }
}
