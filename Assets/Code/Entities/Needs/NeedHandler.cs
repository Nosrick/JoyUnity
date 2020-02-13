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

        public NeedHandler()
        {
            try
            {
                m_Needs = new Dictionary<string, INeed>();

                Type[] needTypes = Scripting.ScriptingEngine.instance.FetchTypeAndChildren(typeof(INeed));

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
            }
            catch(Exception ex)
            {
                Debug.LogError(ex.Message);
                Debug.LogError(ex.StackTrace);
            }
        }

        public INeed Get(string name)
        {
            if(m_Needs.ContainsKey(name))
            {
                return m_Needs[name].Copy();
            }
            throw new InvalidOperationException("Need not found, looking for " + name);
        }

        public INeed GetRandomised(string name)
        {
            if(m_Needs.ContainsKey(name))
            {
                return m_Needs[name].Randomise();
            }
            throw new InvalidOperationException("Need not found, looking for " + name);
        }
    }
}
