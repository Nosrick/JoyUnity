using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using JoyLib.Code.Entities.Abilities;
using JoyLib.Code.Helpers;
using JoyLib.Code.Rollers;
using UnityEngine;

namespace JoyLib.Code.Entities.Jobs
{
    public class JobHandler : IJobHandler
    {
        protected List<IJob> m_Jobs;
        
        protected RNG Roller { get; set; }

        public JobHandler(RNG roller)
        {
            this.Roller = roller;
            this.m_Jobs = this.LoadTypes();
        }

        public IJob Get(string jobName)
        {
            if (this.m_Jobs.Any(x => x.Name.Equals(jobName, StringComparison.OrdinalIgnoreCase)))
            {
                IJob job = this.m_Jobs.First(x => x.Name.Equals(jobName, StringComparison.OrdinalIgnoreCase));
                return job.Copy(job);
            }

            return null;
        }

        public IJob GetRandom()
        {
            int result = this.Roller.Roll(0, this.m_Jobs.Count);
            return this.m_Jobs[result].Copy(this.m_Jobs[result]);
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
                                                                    discount.Element("Discount").GetAs<int>()))
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
                            GlobalConstants.ActionLog.AddText("ERROR LOADING ABILITIES FOR JOB, FILE " + file);
                            GlobalConstants.ActionLog.AddText(e.Message);
                            GlobalConstants.ActionLog.AddText(e.StackTrace);
                            Debug.LogWarning("ERROR LOADING ABILITIES FOR JOB, FILE " + file);
                            Debug.LogWarning(e.Message);
                            Debug.LogWarning(e.StackTrace);
                        }
                                                                        

                        string name = jobElement.Element("Name").GetAs<string>();
                        string description = jobElement.Element("Description").DefaultIfEmpty("NO DESCRIPTION PROVIDED.");

                        jobTypes.Add(new JobType(name, description, statDiscounts, skillDiscounts, abilities));
                    }
                }
                catch(Exception e)
                {
                    GlobalConstants.ActionLog.AddText("ERROR LOADING JOBS, FILE " + file);
                    GlobalConstants.ActionLog.AddText(e.Message);
                    GlobalConstants.ActionLog.AddText(e.StackTrace);
                    Debug.LogWarning("ERROR LOADING JOB FROM FILE " + file);
                    Debug.LogWarning(e.Message);
                    Debug.LogWarning(e.StackTrace);
                }
            }

            return jobTypes;
        }

        public IEnumerable<IJob> Jobs
        {
            get
            {
                if(this.m_Jobs is null)
                {
                    this.m_Jobs = this.LoadTypes();
                }

                return this.m_Jobs;
            }
        }
    }
}
