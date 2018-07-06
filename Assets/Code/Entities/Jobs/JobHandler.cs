using JoyLib.Code.Helpers;
using JoyLib.Code.Loaders;
using System.Collections.Generic;
using System.Linq;

namespace JoyLib.Code.Entities.Jobs
{
    public static class JobHandler
    {
        private static List<JobType> s_Jobs;

        public static void Initialise()
        {
            s_Jobs = new List<JobType>();

            s_Jobs = JobTypeLoader.LoadTypes();
        }

        public static JobType Get(string jobName)
        {
            if (s_Jobs.Any(x => x.name == jobName))
            {
                return s_Jobs.First(x => x.name == jobName);
            }

            return null;
        }

        public static JobType GetRandom()
        {
            return s_Jobs[RNG.Roll(0, s_Jobs.Count - 1)];
        }
    }
}
