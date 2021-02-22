using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Castle.Core.Internal;
using JoyLib.Code.Entities.Abilities;
using JoyLib.Code.Helpers;
using JoyLib.Code.Rollers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JoyLib.Code.Entities.Jobs
{
    public class JobHandler : IJobHandler
    {
        protected List<IJob> m_Jobs;
        
        protected RNG Roller { get; set; }
        
        protected IAbilityHandler AbilityHandler { get; set; }

        public JobHandler(
            IAbilityHandler abilityHandler,
            RNG roller)
        {
            this.AbilityHandler = abilityHandler;
            this.Roller = roller;
            this.m_Jobs = this.Load().ToList();
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

        public IEnumerable<IJob> Load()
        {
            List<IJob> jobTypes = new List<IJob>();

            string[] files = Directory.GetFiles(
                Directory.GetCurrentDirectory() + GlobalConstants.DATA_FOLDER + "Jobs", 
                "*.json", 
                SearchOption.AllDirectories);

            foreach (string file in files)
            {
                using (StreamReader reader = new StreamReader(file))
                {
                    using (JsonTextReader jsonReader = new JsonTextReader(reader))
                    {
                        try
                        {
                            JObject jToken = JObject.Load(jsonReader);

                            if (jToken.IsNullOrEmpty())
                            {
                                continue;
                            }

                            foreach (JToken child in jToken["Jobs"])
                            {
                                string name = (string) child["Name"];
                                string description = (string) child["Description"] ?? "NO DESCRIPTION PROVIDED.";

                                IDictionary<string, int> statisticDiscounts = new Dictionary<string, int>();
                                if (child["Statistics"].IsNullOrEmpty() == false)
                                {
                                    foreach (JToken statistic in child["Statistics"])
                                    {
                                        statisticDiscounts.Add(
                                            (string) statistic["Name"],
                                            (int) statistic["Discount"]);
                                    }
                                }

                                IDictionary<string, int> skillDiscounts = new Dictionary<string, int>();
                                if (child["Skills"].IsNullOrEmpty() == false)
                                {
                                    foreach (JToken skill in child["Skills"])
                                    {
                                        skillDiscounts.Add(
                                            (string) skill["Name"],
                                            (int) skill["Discount"]);
                                    }
                                }

                                IDictionary<IAbility, int> abilityCosts = new Dictionary<IAbility, int>();
                                if (child["Abilities"].IsNullOrEmpty() == false)
                                {
                                    foreach (JToken ability in child["Abilities"])
                                    {
                                        abilityCosts.Add(
                                            this.AbilityHandler.Get((string) ability["Name"]),
                                            (int) ability["Cost"]);
                                    }
                                }
                                
                                jobTypes.Add(
                                    new JobType(
                                        name,
                                        description,
                                        statisticDiscounts,
                                        skillDiscounts,
                                        abilityCosts));
                            }
                        }
                        catch (Exception e)
                        {
                            GlobalConstants.ActionLog.AddText("ERROR LOADING JOB, FILE " + file, LogLevel.Error);
                            GlobalConstants.ActionLog.StackTrace(e);
                        }
                        finally
                        {
                            jsonReader.Close();
                            reader.Close();
                        }
                    }
                }
            }
            
            return jobTypes;
        }

        public IEnumerable<IJob> Values => this.m_Jobs ?? (this.m_Jobs = this.Load().ToList());

        public void Dispose()
        {
            this.m_Jobs = null;
        }

        ~JobHandler()
        {
            this.Dispose();
        }
    }
}
