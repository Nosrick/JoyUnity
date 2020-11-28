using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JoyLib.Code.Entities.Needs
{
    public class NeedHandler : MonoBehaviour
    {
        private Dictionary<string, INeed> m_Needs;

        public void Awake()
        {
            if (m_Needs is null)
            {
                m_Needs = Initialise();                
            }
        }

        protected static Dictionary<string, INeed> Initialise()
        {
            try
            {
                Dictionary<string, INeed> needs = new Dictionary<string, INeed>();

                Type[] needTypes = Scripting.ScriptingEngine.instance.FetchTypeAndChildren(typeof(INeed));

                foreach (Type type in needTypes)
                {
                    if (typeof(INeed).IsAssignableFrom(type) == true && type.IsAbstract == false)
                    {
                        INeed newNeed = (INeed)Activator.CreateInstance(type);
                        needs.Add(newNeed.Name, newNeed);
                    }
                    else
                    {
                        continue;
                    }
                }
                return needs;
            }
            catch(Exception ex)
            {
                Debug.LogError(ex.Message);
                Debug.LogError(ex.StackTrace);
                Debug.LogError(ex.InnerException.StackTrace);
                return new Dictionary<string, INeed>();
            }
        }

        public INeed Get(string name)
        {
            if(m_Needs is null)
            {
                m_Needs = Initialise();
            }

            if(m_Needs.ContainsKey(name))
            {
                return m_Needs[name].Copy();
            }
            throw new InvalidOperationException("Need not found, looking for " + name);
        }

        public ICollection<INeed> GetMany(string[] names)
        {
            if (m_Needs is null)
            {
                m_Needs = Initialise();
            }

            INeed[] needs = m_Needs
                .Where(pair => names.Any(
                    name => name.Equals(pair.Key, StringComparison.OrdinalIgnoreCase)))
                .Select(pair => pair.Value)
                .ToArray();

            return needs;
        }

        public ICollection<INeed> GetManyRandomised(string[] names)
        {
            INeed[] tempNeeds = GetMany(names).ToArray();

            List<INeed> needs = new List<INeed>();

            foreach (INeed need in tempNeeds)
            {
                needs.Add(need.Randomise());
            }

            return needs;
        }

        public INeed GetRandomised(string name)
        {
            if(m_Needs is null)
            {
                m_Needs = Initialise();
            }

            if(m_Needs.ContainsKey(name))
            {
                return m_Needs[name].Randomise();
            }
            throw new InvalidOperationException("Need not found, looking for " + name);
        }

        public Dictionary<string, INeed> Needs
        {
            get
            {
                if (m_Needs is null)
                {
                    m_Needs = Initialise();
                }
                return new Dictionary<string, INeed>(m_Needs);
            }
        }
    }
}
