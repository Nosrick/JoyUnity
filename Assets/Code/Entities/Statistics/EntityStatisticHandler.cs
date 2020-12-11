using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using JoyLib.Code.Helpers;
using UnityEngine;

namespace JoyLib.Code.Entities.Statistics
{
    public class EntityStatisticHandler : IEntityStatisticHandler
    {
        public IEnumerable<string> StatisticNames { get; protected set; }

        public EntityStatisticHandler()
        {
            this.StatisticNames = this.LoadStatisticNames();
        }

        protected IEnumerable<string> LoadStatisticNames()
        {
            string file = Directory.GetCurrentDirectory() + GlobalConstants.DATA_FOLDER + "Statistics.xml";

            List<string> names = new List<string>();

            try
            {
                XElement doc = XElement.Load(file);

                names.AddRange(from stat in doc.Elements("Statistic")
                    select stat.GetAs<string>());
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            
            return names;
        }
    }
}