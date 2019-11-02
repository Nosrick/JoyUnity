using JoyLib.Code.Collections;
using JoyLib.Code.Entities.Abilities;
using JoyLib.Code.Helpers;
using JoyLib.Code.Rollers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace JoyLib.Code.Entities.Jobs
{
    public static class JobHandler
    {
        private static List<JobType> s_Jobs;

        public static void Initialise()
        {
            s_Jobs = new List<JobType>();

            s_Jobs = LoadTypes();
        }

        public static JobType Get(string jobName)
        {
            if (s_Jobs.Any(x => x.Name == jobName))
            {
                return s_Jobs.First(x => x.Name == jobName);
            }

            return null;
        }

        public static JobType GetRandom()
        {
            return s_Jobs[RNG.Roll(0, s_Jobs.Count - 1)];
        }

        private static List<JobType> LoadTypes()
        {
            List<JobType> jobTypes = new List<JobType>();

            string[] files = Directory.GetFiles(
                Directory.GetCurrentDirectory() + GlobalConstants.DATA_FOLDER + "Jobs", 
                "*.xml", 
                SearchOption.AllDirectories);

            foreach(string file in files)
            {
                try
                {
                    XElement doc = XElement.Load(file);

                    foreach(XElement jobElement in doc.Elements("Job"))
                    {
                        Dictionary<string, float> statGrowths = (from growth in jobElement.Elements("Statistic")
                                                                 select new KeyValuePair<string, float>(
                                                                     growth.Element("Name").GetAs<string>().ToLower(),
                                                                     growth.Element("Growth").GetAs<float>()))
                                                                     .ToDictionary(x => x.Key, x => x.Value);

                        Dictionary<string, int> skillGrowths = (from growth in jobElement.Elements("Skill")
                                                                select new KeyValuePair<string, int>(
                                                                    growth.Element("Name").GetAs<string>().ToLower(),
                                                                    growth.Element("Growth").GetAs<int>()))
                                                                    .ToDictionary(x => x.Key, x => x.Value);

                        NonUniqueDictionary<int, IAbility> abilities = new NonUniqueDictionary<int, IAbility>();
                        //TODO: Remove this nastiness
                        try
                        {

                            List<Tuple<int, IAbility>> listAbilities = (from ability in jobElement.Elements("Ability")
                                                                        select new Tuple<int, IAbility>(
                                                                            ability.Element("Level").GetAs<int>(),
                                                                            AbilityHandler.GetAbility(
                                                                                ability.Element("Name").GetAs<string>().ToLower()))).ToList();

                            foreach (Tuple<int, IAbility> ability in listAbilities)
                            {
                                abilities.Add(ability.Item1, ability.Item2);
                            }
                        }
                        catch(InvalidOperationException e)
                        {
                            ActionLog.WriteToLog("ERROR LOADING ABILITIES FOR JOB, FILE " + file);
                            ActionLog.WriteToLog(e.Message);
                            ActionLog.WriteToLog(e.StackTrace);
                        }
                                                                        

                        string name = jobElement.Element("Name").GetAs<string>().ToLower();
                        string description = jobElement.Element("Description").DefaultIfEmpty("NO DESCRIPTION PROVIDED.").ToLower();

                        jobTypes.Add(new JobType(name, description, statGrowths, skillGrowths, abilities));
                    }
                }
                catch(Exception e)
                {
                    ActionLog.WriteToLog("ERROR LOADING JOBS, FILE " + file);
                    ActionLog.WriteToLog(e.Message);
                    ActionLog.WriteToLog(e.StackTrace);
                }
            }

            return jobTypes;
        }
    }
}
