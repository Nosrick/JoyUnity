using JoyLib.Code.Entities.Abilities;
using System.Collections.Generic;

namespace JoyLib.Code.Entities.Jobs
{
    public class JobType
    {
        public JobType(string name, string description, List<Tuple<int, Ability>> abilities)
        {
            this.name = name;
            this.description = description;
            this.abilities = abilities;
        }

        public string name
        {
            get;
            private set;
        }

        public string description
        {
            get;
            private set;
        }

        public List<Tuple<int, Ability>> abilities
        {
            get;
            protected set;
        }
    }
}
