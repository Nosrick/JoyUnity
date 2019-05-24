using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace JoyLib.Code.Entities
{
    public class EntityStatistic
    {
        private static List<string> s_Names;

        public EntityStatistic(int value, int successThreshold)
        {
            Value = value;
            SuccessThreshold = successThreshold;
        }

        public void Modify(int value)
        {
            Value = Math.Max(1, Value + value);
        }

        public int Value
        {
            get;
            protected set;
        }

        public int SuccessThreshold
        {
            get;
            protected set;
        }

        public static bool LoadStatistics()
        {
            if(s_Names != null)
            {
                return true;
            }

            s_Names = new List<string>();

            XmlReader reader = XmlReader.Create(Directory.GetCurrentDirectory() + GlobalConstants.DATA_FOLDER + "Statistics.xml");
            while(reader.Read())
            {
                if(reader.Name.Equals(""))
                {
                    continue;
                }

                if(reader.NodeType == XmlNodeType.EndElement && reader.Name.Equals("Statistics"))
                {
                    break;
                }

                if(reader.Name.Equals("Statistic"))
                {
                    s_Names.Add(reader.ReadElementContentAsString());
                }
            }

            return true;
        }

        public static string[] Names
        {
            get
            {
                return s_Names.ToArray();
            }
        }
    }
}
