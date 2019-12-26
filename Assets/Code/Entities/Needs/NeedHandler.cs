using System;
using System.Collections.Generic;
using UnityEngine;

namespace JoyLib.Code.Entities.Needs
{
    public class NeedHandler
    {
        private static readonly Lazy<NeedHandler> lazy = new Lazy<NeedHandler>(() => new NeedHandler());

        public static NeedHandler instance => lazy.Value;

        private Dictionary<string, INeed> m_Needs;

        public bool Initialise()
        {
            try
            {
                if (m_Needs != null)
                {
                    return true;
                }

                m_Needs = new Dictionary<string, INeed>();

                Type[] needTypes = Scripting.ScriptingEngine.FetchTypeAndChildren("AbstractNeed");

                foreach (Type type in needTypes)
                {
                    if (typeof(INeed).IsAssignableFrom(type) == true && type.IsAbstract == false)
                    {
                        INeed newNeed = (INeed)Activator.CreateInstance(type);
                        m_Needs.Add(newNeed.Name, newNeed);
                    }
                    else
                    {
                        continue;
                    }
                }

                return true;
            }
            catch(Exception ex)
            {
                Debug.LogError(ex.Message);
                Debug.LogError(ex.StackTrace);
                return false;
            }
        }

        public INeed Get(string name)
        {
            if(m_Needs.ContainsKey(name))
            {
                return m_Needs[name].Copy();
            }
            return null;
        }

        public INeed GetRandomised(string name)
        {
            if(m_Needs.ContainsKey(name))
            {
                return m_Needs[name].Randomise();
            }
            return null;
        }
    }
}
