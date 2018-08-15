using JoyLib.Code.Loaders;
using System.Collections.Generic;
using System.Linq;

namespace JoyLib.Code.Entities
{
    public static class EntityTemplateHandler
    {
        private static List<EntityTemplate> s_Templates;

        public static void Initialise()
        {
            s_Templates = new List<EntityTemplate>();

            s_Templates = EntityLoader.LoadTypes();
        }

        public static EntityTemplate Get(string type)
        {
            if(s_Templates.Any(x => x.CreatureType == type))
            {
                return s_Templates.First(x => x.CreatureType == type);
            }

            return null;
        }

        public static List<EntityTemplate> Templates
        {
            get
            {
                return new List<EntityTemplate>(s_Templates);
            }
        }
    }
}
