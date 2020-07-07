using JoyLib.Code.Cultures;
using JoyLib.Code.Scripting;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace JoyLib.Code.Entities.Sexuality
{
    public class EntitySexualityHandler : MonoBehaviour
    {
        protected Dictionary<string, ISexuality> m_Sexualities;

        protected CultureHandler m_CultureHandler;

        public void Awake()
        {
            Initialise();
        }

        public void Initialise()
        {
            m_CultureHandler = GameObject.Find("GameManager").GetComponent<CultureHandler>();
            Load(m_CultureHandler.Cultures);
        }

        protected bool Load(CultureType[] cultures)
        {
            if (m_Sexualities != null)
            {
                return true;
            }

            m_Sexualities = new Dictionary<string, ISexuality>();

            foreach (CultureType culture in cultures)
            {
                foreach (string sexuality in culture.Sexualities)
                {
                    if (m_Sexualities.ContainsKey(sexuality) == false)
                    {
                        Type type = ScriptingEngine.instance.FetchType(sexuality);
                        if (!(type is null))
                        {
                            ISexuality sexualityInstance = (ISexuality)Activator.CreateInstance(type);
                            m_Sexualities.Add(sexuality, sexualityInstance);
                        }
                    }
                }
            }

            return true;
        }

        public ISexuality Get(string sexuality)
        {
            if (m_Sexualities is null)
            {
            }

            if (m_Sexualities.ContainsKey(sexuality))
            {
                return m_Sexualities[sexuality];
            }
            return null;
        }

        public ISexuality[] Sexualities
        {
            get
            {
                if (m_Sexualities is null)
                {
                    Initialise();
                }

                return m_Sexualities.Values.ToArray();
            }
        }
    }
}
