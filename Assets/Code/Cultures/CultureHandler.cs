using JoyLib.Code.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Linq;
using JoyLib.Code.Graphics;
using UnityEngine;

namespace JoyLib.Code.Cultures
{
    public class CultureHandler : MonoBehaviour
    {        
        protected Dictionary<string, CultureType> m_Cultures;

        public void Awake()
        {
            m_Cultures = new Dictionary<string, CultureType>();

            m_Cultures = LoadCultures();
        }

        private Dictionary<string, CultureType> LoadCultures()
        {
            string folderPath = Directory.GetCurrentDirectory() + GlobalConstants.DATA_FOLDER + "Cultures";
            string[] files = Directory.GetFiles(folderPath, "*.xml");

            ObjectIconHandler objectIcons = GameObject.Find("GameManager")
                                                .GetComponent<ObjectIconHandler>();

            Dictionary<string, CultureType> cultures = new Dictionary<string, CultureType>();

            foreach (string file in files)
            {
                XElement element = XElement.Load(file);

                foreach (XElement culture in element.Elements("Culture"))
                {
                    List<string> rulers = (from ruler in culture.Element("Rulers")
                                            .Elements("Ruler")
                                           select ruler.GetAs<string>().ToLower()).ToList();

                    List<string> crimes = (from crime in culture.Element("Crimes")
                                                    .Elements("Crime")
                                           select crime.GetAs<string>().ToLower()).ToList();

                    List<string> inhabitants = (from inhabitant in culture.Element("Inhabitants")
                                                    .Elements("Inhabitant")
                                                select inhabitant.GetAs<string>().ToLower()).ToList();

                    List<string> relationships = (from relationship in culture.Element("Relationships")
                                                    .Elements("RelationshipType")
                                                  select relationship.GetAs<string>().ToLower()).ToList();

                    List<NameData> nameDataList = (from nameData in culture.Element("Names")
                                                    .Elements("NameData")
                                                   select new NameData(
                                                      nameData.Element("Name").GetAs<string>().ToLower(),
                                                      nameData.Elements("Chain").Select(x => x.GetAs<int>()).ToArray(),
                                                      nameData.Elements("Sex").Select(x => x.GetAs<string>().ToLower()).ToArray()
                                                      )).ToList();

                    Dictionary<string, int> sexualitiesDictionary = (from sexualities in culture.Element("Sexualities")
                                                                        .Elements("Sexuality")
                                                                     select new KeyValuePair<string, int>(
                                                                      sexualities.Element("Name").GetAs<string>().ToLower(),
                                                                      sexualities.Element("Chance").GetAs<int>()
                                                                      )).ToDictionary(x => x.Key, x => x.Value);

                    Dictionary<string, int> sexesDictionary = (from sexes in culture.Element("Sexes")
                                                                    .Elements("Sex")
                                                               select new KeyValuePair<string, int>(
                                                              sexes.Element("Name").GetAs<string>().ToLower(),
                                                              sexes.Element("Chance").GetAs<int>()))
                                                                   .ToDictionary(x => x.Key, x => x.Value);

                    Dictionary<string, Tuple<int, int>> statVarianceDictionary = (from statVariances in culture.Element("Statistics")
                                                                                    .Elements("StatVariance")
                                                                                  select new KeyValuePair<string, Tuple<int, int>>(
                                                                                    statVariances.Element("Name").GetAs<string>().ToLower(),
                                                                                    new Tuple<int, int>(
                                                                                        statVariances.Element("Chance").GetAs<int>(),
                                                                                        statVariances.Element("Magnitude").GetAs<int>())))
                                                                                          .ToDictionary(x => x.Key, x => x.Value);

                    Dictionary<string, int> jobPrevelenceDictionary = (from jobPrevelence in culture.Element("Jobs")
                                                                            .Elements("Job")
                                                                       select new KeyValuePair<string, int>(
                                                                      jobPrevelence.Element("Name").GetAs<string>().ToLower(),
                                                                      jobPrevelence.Element("Chance").GetAs<int>()))
                                                                           .ToDictionary(x => x.Key, x => x.Value);

                    string cultureName = culture.Element("CultureName").GetAs<string>();

                    string tileSet = culture.Element("TileSet").Element("Name").DefaultIfEmpty("");
                    string filename = culture.Element("TileSet").Element("Filename").DefaultIfEmpty("");

                    if (tileSet.Length > 0)
                    {
                        IconData[] icons = (from cultureIcons in culture.Element("TileSet").Elements("Icon")
                                            select new IconData()
                                            {
                                                name = cultureIcons.Element("Name").GetAs<string>(),
                                                data = cultureIcons.Element("Data").DefaultIfEmpty(""),
                                                frames = cultureIcons.Element("Frames").GetAs<int>(),
                                                position = new UnityEngine.Vector2Int(
                                                    cultureIcons.Element("X").GetAs<int>(),
                                                    cultureIcons.Element("Y").GetAs<int>())
                                            }).ToArray();

                        objectIcons.AddIcons(
                            filename, tileSet, icons);
                    }

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

        public CultureType GetByCultureName(string name)
        {
            if (m_Cultures.ContainsKey(name))
            {
                return m_Cultures[name];
            }

            return null;
        }

        public List<CultureType> GetByCreatureType(string type)
        {
            try
            {
                Dictionary<string, CultureType> cultures = m_Cultures.Where(culture => culture.Value.Inhabitants.Contains(type.ToLowerInvariant())).ToDictionary(pair => pair.Key, pair => pair.Value);
                return cultures.Values.ToList();
            }
            catch (Exception e)
            {
                ActionLog.instance.AddText("Could not find a culture for creature type " + type);
                throw new InvalidOperationException("Could not find a culture for creature type " + type, e);
            }
        }

        public CultureType[] Cultures
        {
            get
            {
                return m_Cultures.Values.ToArray();
            }
        }
    }
}
