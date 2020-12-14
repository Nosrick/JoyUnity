using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JoyLib.Code.Entities.Needs
{
    public class NeedHandler : INeedHandler
    {
        protected Dictionary<string, INeed> m_Needs;

        public NeedHandler()
        {
            m_Needs = Initialise();
        }

        protected static Dictionary<string, INeed> Initialise()
        {
            try
            {
                Dictionary<string, INeed> needs = new Dictionary<string, INeed>();

                IEnumerable<INeed> needTypes = Scripting.ScriptingEngine.instance.FetchAndInitialiseChildren<INeed>();

                foreach (INeed type in needTypes)
                {
                    needs.Add(type.Name, type);
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

        public ICollection<INeed> GetMany(IEnumerable<string> names)
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

        public ICollection<INeed> GetManyRandomised(IEnumerable<string> names)
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

        public IEnumerable<INeed> Needs
        {
            get
            {
                if (m_Needs is null)
                {
                    m_Needs = Initialise();
                }
                return new List<INeed>(m_Needs.Values);
            }
        }

        public IEnumerable<string> NeedNames
        {
            get
            {
                if (m_Needs is null)
                {
                    m_Needs = Initialise();
                }

                return new List<string>(m_Needs.Keys);
            }
        }
    }
}
