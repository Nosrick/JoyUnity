using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Castle.Core.Internal;
using JoyLib.Code.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JoyLib.Code.Entities.Statistics
{
    public class EntityStatisticHandler : IEntityStatisticHandler
    {
        public IEnumerable<string> StatisticNames => this.Statistics.Keys;

        public IEnumerable<IEntityStatistic> Values => this.Statistics.Values;
        
        protected IDictionary<string, IEntityStatistic> Statistics { get; set; }
        protected IDictionary<string, IEntityStatistic> DefaultStatistics { get; set; }

        public EntityStatisticHandler()
        {
            this.Statistics = this.Load().ToDictionary(statistic => statistic.Name, stat => stat);
        }

        public IEnumerable<IEntityStatistic> Load()
        {
            string file = Directory.GetCurrentDirectory() + GlobalConstants.DATA_FOLDER + "/Statistics/Statistics.json";

            List<IEntityStatistic> statistics = new List<IEntityStatistic>();

            using (StreamReader reader = new StreamReader(file))
            {
                using (JsonTextReader jsonReader = new JsonTextReader(reader))
                {
                    try
                    {
                        JObject jToken = JObject.Load(jsonReader);

                        if (jToken["Statistics"].IsNullOrEmpty() == false)
                        {
                            statistics.AddRange(jToken["Statistics"].Select(child =>
                                new EntityStatistic((string) child, 0, GlobalConstants.DEFAULT_SUCCESS_THRESHOLD)));
                        }
                    }
                    catch (Exception e)
                    {
                        GlobalConstants.ActionLog.AddText("Could not load skills from " + file);
                        GlobalConstants.ActionLog.StackTrace(e);
                    }
                }
            }

            this.DefaultStatistics = statistics.ToDictionary(stat => stat.Name, statistic => statistic);
            
            return statistics;
        }
        
        public IEntityStatistic Get(string name)
        {
            return this.Statistics.TryGetValue(name, out IEntityStatistic statistic) ? statistic.Copy() : null;
        }

        public IDictionary<string, IEntityStatistic> GetDefaultBlock()
        {
            return this.DefaultStatistics.Copy();
        }

        public void Dispose()
        {
            string[] keys = this.StatisticNames.ToArray();
            foreach (string key in keys)
            {
                this.Statistics[key] = null;
            }

            keys = this.DefaultStatistics.Keys.ToArray();
            foreach (string key in keys)
            {
                this.DefaultStatistics[key] = null;
            }

            this.Statistics = null;
            this.DefaultStatistics = null;
        }

        ~EntityStatisticHandler()
        {
            this.Dispose();
        }
    }
}