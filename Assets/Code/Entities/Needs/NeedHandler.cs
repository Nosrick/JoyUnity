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
            m_Needs = Initialise();
        }

        public static Dictionary<string, INeed> Initialise()
        {
            try
            {
                Dictionary<string, INeed> needs = new Dictionary<string, INeed>();

                Type[] needTypes = Scripting.ScriptingEngine.instance.FetchTypeAndChildren(typeof(INeed));

                foreach (Type type in needTypes)
                {
                    if ((typeof(INeed)).IsAssignableFrom(type) == true && type.IsAbstract == false)
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

        public string[] NeedNames
        {
            get
            {
                return m_Needs.Keys.ToArray();
            }
        }
    }
}
