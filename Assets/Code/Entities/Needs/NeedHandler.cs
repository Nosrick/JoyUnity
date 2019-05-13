using System;
using System.Collections.Generic;
using UnityEngine;

namespace JoyLib.Code.Entities.Needs
{
    public static class NeedHandler
    {
        private static Dictionary<string, INeed> s_Needs;

        public static bool Initialise()
        {
            try
            {
                if (s_Needs != null)
                {
                    return true;
                }

                s_Needs = new Dictionary<string, INeed>();

                List<Type> needTypes = Scripting.ScriptingEngine.FetchTypeAndChildren("AbstractNeed");

                foreach (Type type in needTypes)
                {
                    if (typeof(INeed).IsAssignableFrom(type) == true && type.IsAbstract == false)
                    {
                        INeed newNeed = (INeed)Activator.CreateInstance(type);
                        s_Needs.Add(newNeed.GetName(), newNeed);
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

        public static INeed Get(string name)
        {
            if(s_Needs.ContainsKey(name))
            {
                return s_Needs[name].Copy();
            }
            return null;
        }

        public static INeed GetRandomised(string name)
        {
            if(s_Needs.ContainsKey(name))
            {
                return s_Needs[name].Randomise();
            }
            return null;
        }
    }
}
