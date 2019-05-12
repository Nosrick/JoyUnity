using System;
using System.Collections.Generic;
using UnityEngine;

namespace JoyLib.Code.Entities.Needs
{
    public static class NeedHandler
    {
        private static Dictionary<string, AbstractNeed> s_Needs;

        public static bool Initialise()
        {
            try
            {
                if (s_Needs != null)
                {
                    return true;
                }

                s_Needs = new Dictionary<string, AbstractNeed>();

                List<Type> needTypes = Scripting.ScriptingEngine.FetchTypeAndChildren("AbstractNeed");

                foreach (Type type in needTypes)
                {
                    if (type == typeof(AbstractNeed) && type.IsAbstract == false)
                    {
                        AbstractNeed newNeed = (AbstractNeed)Activator.CreateInstance(type);
                        s_Needs.Add(newNeed.name, newNeed);
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

        public static AbstractNeed Get(string name)
        {
            if(s_Needs.ContainsKey(name))
            {
                return s_Needs[name].Copy();
            }
            return null;
        }

        public static AbstractNeed GetRandomised(string name)
        {
            if(s_Needs.ContainsKey(name))
            {
                return s_Needs[name].Randomise();
            }
            return null;
        }
    }
}
