using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using JoyLib.Code.Entities.Abilities;
using JoyLib.Code.Helpers;
using JoyLib.Code.Rollers;

namespace JoyLib.Code.Entities.Jobs
{
    public class JobHandler : IJobHandler
    {
        protected List<IJob> m_Jobs;
        
        protected RNG Roller { get; set; }

        public JobHandler(RNG roller)
        {
            Roller = roller;
            m_Jobs = LoadTypes();
        }

        public IJob Get(string jobName)
        {
            if (m_Jobs.Any(x => x.Name == jobName))
            {
                return m_Jobs.First(x => x.Name == jobName);
            }

            return null;
        }

        public IJob GetRandom()
        {
            return m_Jobs[Roller.Roll(0, m_Jobs.Count)];
        }

        protected List<IJob> LoadTypes()
        {
            List<IJob> jobTypes = new List<IJob>();

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
                        Dictionary<string, int> statDiscounts = (from discount in jobElement.Elements("Statistic")
                                                                 select new KeyValuePair<string, int>(
                                                                     discount.Element("Name").GetAs<string>(),
                                                                     discount.Element("Discount").GetAs<int>()))
                                                                     .ToDictionary(x => x.Key, x => x.Value);

                        Dictionary<string, int> skillDiscounts = (from discount in jobElement.Elements("Skill")
                                                                select new KeyValuePair<string, int>(
                                                                    discount.Element("Name").GetAs<string>(),
                                                                    discount.Element("Growth").GetAs<int>()))
                                                                    .ToDictionary(x => x.Key, x => x.Value);

                        Dictionary<IAbility, int> abilities = new Dictionary<IAbility, int>();
                        //TODO: Remove this nastiness
                        try
                        {
                            abilities = (from ability in jobElement.Elements("Ability")
                                select new KeyValuePair<IAbility, int>(
                                    GlobalConstants.GameManager.AbilityHandler.GetAbility(
                                        ability.Element("Name").GetAs<string>()),
                                    ability.Element("Cost").GetAs<int>()))
                                .ToDictionary(x => x.Key, x => x.Value);
                        }
                        catch(Exception e)
                        {
                            ActionLog.instance.AddText("ERROR LOADING ABILITIES FOR JOB, FILE " + file);
                            ActionLog.instance.AddText(e.Message);
                            ActionLog.instance.AddText(e.StackTrace);
                        }
                                                                        

                        string name = jobElement.Element("Name").GetAs<string>();
                        string description = jobElement.Element("Description").DefaultIfEmpty("NO DESCRIPTION PROVIDED.");

                        jobTypes.Add(new JobType(name, description, statDiscounts, skillDiscounts, abilities));
                    }
                }
                catch(Exception e)
                {
                    ActionLog.instance.AddText("ERROR LOADING JOBS, FILE " + file);
                    ActionLog.instance.AddText(e.Message);
                    ActionLog.instance.AddText(e.StackTrace);
                }
            }

            return jobTypes;
        }

        public IJob[] Jobs
        {
            get
            {
                if(m_Jobs is null)
                {
                    m_Jobs = LoadTypes();
                }

                return m_Jobs.ToArray();
            }
        }
    }
}
