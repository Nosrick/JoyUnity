﻿using JoyLib.Code.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Linq;

namespace JoyLib.Code.Cultures
{
    public static class CultureHandler
    {
        private static Dictionary<string, CultureType> s_Cultures;

        public static void Initialise()
        {
            s_Cultures = new Dictionary<string, CultureType>();

            s_Cultures = LoadCultures();
        }

        private static Dictionary<string, CultureType> LoadCultures()
        {
            string folderPath = Directory.GetCurrentDirectory() + GlobalConstants.DATA_FOLDER + "Cultures";
            string[] files = Directory.GetFiles(folderPath, "*.xml");

            Dictionary<string, CultureType> cultures = new Dictionary<string, CultureType>();

            foreach (string file in files)
            {
                XElement element = XElement.Load(file);

                foreach (XElement culture in element.Elements("Culture"))
                {
                    List<string> rulers = (from ruler in culture.Elements("Ruler")
                                           select ruler.GetAs<string>().ToLower()).ToList();

                    List<string> crimes = (from crime in culture.Elements("Crime")
                                           select crime.GetAs<string>().ToLower()).ToList();

                    List<string> inhabitants = (from inhabitant in culture.Elements("Inhabitant")
                                                select inhabitant.GetAs<string>().ToLower()).ToList();

                    List<string> relationships = (from relationship in culture.Elements("RelationshipType")
                                                  select relationship.GetAs<string>().ToLower()).ToList();

                    List<NameData> nameDataList = (from nameData in culture.Elements("NameData")
                                                   select new NameData(
                                                       nameData.Element("Name").GetAs<string>().ToLower(),
                                                       nameData.Elements("Chain").Select(x => x.GetAs<int>()).ToArray(),
                                                       nameData.Elements("Sex").Select(x => x.GetAs<string>().ToLower()).ToArray()
                                                       )).ToList();

                    Dictionary<string, int> sexualitiesDictionary = (from sexualities in culture.Elements("Sexuality")
                                                                     select new KeyValuePair<string, int>(
                                                                         sexualities.Element("Name").GetAs<string>().ToLower(),
                                                                         sexualities.Element("Chance").GetAs<int>()
                                                                         )).ToDictionary(x => x.Key, x => x.Value);

                    Dictionary<string, int> sexesDictionary = (from sexes in culture.Elements("Sex")
                                                               select new KeyValuePair<string, int>(
                                                                   sexes.Element("Name").GetAs<string>().ToLower(),
                                                                   sexes.Element("Chance").GetAs<int>()))
                                                                   .ToDictionary(x => x.Key, x => x.Value);

                    Dictionary<string, Tuple<int, int>> statVarianceDictionary = (from statVariances in culture.Elements("StatVariance")
                                                                                  select new KeyValuePair<string, Tuple<int, int>>(
                                                                                      statVariances.Element("Name").GetAs<string>().ToLower(),
                                                                                      new Tuple<int, int>(
                                                                                          statVariances.Element("Chance").GetAs<int>(),
                                                                                          statVariances.Element("Magnitude").GetAs<int>())))
                                                                                          .ToDictionary(x => x.Key, x => x.Value);

                    Dictionary<string, int> jobPrevelenceDictionary = (from jobPrevelence in culture.Elements("Job")
                                                                       select new KeyValuePair<string, int>(
                                                                           jobPrevelence.Element("Name").GetAs<string>().ToLower(),
                                                                           jobPrevelence.Element("Chance").GetAs<int>()))
                                                                           .ToDictionary(x => x.Key, x => x.Value);

                    string cultureName = culture.Element("CultureName").GetAs<string>();

                    cultures.Add(cultureName,
                        new CultureType(
                            cultureName,
                            rulers,
                            crimes,
                            nameDataList,
                            jobPrevelenceDictionary,
                            inhabitants,
                            sexualitiesDictionary,
                            sexesDictionary,
                            statVarianceDictionary,
                            relationships));
                }
            }

            return cultures;
        }

        public static CultureType GetByCultureName(string name)
        {
            if(s_Cultures.ContainsKey(name))
            {
                return s_Cultures[name];
            }

            return null;
        }

        public static List<CultureType> GetByCreatureType(string type)
        {
            try
            {
                Dictionary<string, CultureType> cultures = s_Cultures.Where(culture => culture.Value.Inhabitants.Contains(type.ToLowerInvariant())).ToDictionary(pair => pair.Key, pair => pair.Value);
                return cultures.Values.ToList();
            }
            catch(Exception e)
            {
                ActionLog.WriteToLog("Could not find a culture for creature type " + type);
                return new List<CultureType>();
            }
        }

        public static CultureType[] Cultures
        {
            get
            {
                return s_Cultures.Values.ToArray();
            }
        }
    }
}
