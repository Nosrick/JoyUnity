using System;
using System.Collections.Generic;
using System.Linq;

namespace JoyLib.Code.Entities.Needs
{
    public static class NeedHandler
    {
        private static List<NeedAbstract> s_Needs;

        public static void Initialise()
        {
            if(s_Needs != null)
            {
                return;
            }

            s_Needs = new List<NeedAbstract>();

            List<Type> types = typeof(NeedAbstract).Assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(NeedAbstract))).ToList();
            foreach(Type type in types)
            {
                s_Needs.Add((NeedAbstract)type.Assembly.CreateInstance(type.Name));
            }
        }

        public static NeedAbstract Get(string name)
        {
            if(s_Needs.Any(x => x.Name == name))
            {
                return s_Needs.First(x => x.Name == name);
            }

            return null;
        }
    }
}
