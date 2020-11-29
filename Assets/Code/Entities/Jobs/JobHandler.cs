using JoyLib.Code.Collections;
using JoyLib.Code.Entities.Abilities;
using JoyLib.Code.Helpers;
using JoyLib.Code.Rollers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

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
            return m_Jobs[Roller.Roll(0, m_Jobs.Count - 1)];
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
                        Dictionary<string, float> statGrowths = (from growth in jobElement.Elements("Statistic")
                                                                 select new KeyValuePair<string, float>(
                                                                     growth.Element("Name").GetAs<string>(),
                                                                     growth.Element("Growth").GetAs<float>()))
                                                                     .ToDictionary(x => x.Key, x => x.Value);

                        Dictionary<string, int> skillGrowths = (from growth in jobElement.Elements("Skill")
                                                                select new KeyValuePair<string, int>(
                                                                    growth.Element("Name").GetAs<string>(),
                                                                    growth.Element("Growth").GetAs<int>()))
                                                                    .ToDictionary(x => x.Key, x => x.Value);

                        NonUniqueDictionary<int, IAbility> abilities = new NonUniqueDictionary<int, IAbility>();
                        //TODO: Remove this nastiness
                        try
                        {

                            List<Tuple<int, IAbility>> listAbilities = (from ability in jobElement.Elements("Ability")
                                                                        select new Tuple<int, IAbility>(
                                                                            ability.Element("Level").GetAs<int>(),
                                                                            GlobalConstants.GameManager.AbilityHandler.GetAbility(
                                                                                ability.Element("Name").GetAs<string>()))).ToList();

                            foreach (Tuple<int, IAbility> ability in listAbilities)
                            {
                                abilities.Add(ability.Item1, ability.Item2);
                            }
                        }
                        catch(Exception e)
                        {
                            ActionLog.instance.AddText("ERROR LOADING ABILITIES FOR JOB, FILE " + file);
                            ActionLog.instance.AddText(e.Message);
                            ActionLog.instance.AddText(e.StackTrace);
                        }
                                                                        

                        string name = jobElement.Element("Name").GetAs<string>();
                        string description = jobElement.Element("Description").DefaultIfEmpty("NO DESCRIPTION PROVIDED.");

                        jobTypes.Add(new JobType(name, description, statGrowths, skillGrowths, abilities));
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
