﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using JoyLib.Code.Graphics;
using JoyLib.Code.Helpers;

namespace JoyLib.Code.Cultures
{
    public class CultureHandler : ICultureHandler
    {        
        protected Dictionary<string, ICulture> m_Cultures;

        public CultureHandler()
        {
            this.m_Cultures = new Dictionary<string, ICulture>();

            this.m_Cultures = this.LoadCultures();
        }

        private Dictionary<string, ICulture> LoadCultures()
        {
            string folderPath = Directory.GetCurrentDirectory() + GlobalConstants.DATA_FOLDER + "Cultures";
            string[] files = Directory.GetFiles(folderPath, "*.xml");

            IObjectIconHandler objectIcons = GlobalConstants.GameManager.ObjectIconHandler;

            Dictionary<string, ICulture> cultures = new Dictionary<string, ICulture>();

            foreach (string file in files)
            {
                XElement element = XElement.Load(file);

                foreach (XElement culture in element.Elements("Culture"))
                {
                    List<string> rulers = (from ruler in culture.Element("Rulers")
                                            .Elements("Ruler")
                                           select ruler.GetAs<string>()).ToList();

                    List<string> crimes = (from crime in culture.Element("Crimes")
                                                    .Elements("Crime")
                                           select crime.GetAs<string>()).ToList();

                    List<string> inhabitants = (from inhabitant in culture.Element("Inhabitants")
                                                    .Elements("Inhabitant")
                                                select inhabitant.GetAs<string>()).ToList();

                    List<string> relationships = (from relationship in culture.Element("Relationships")
                                                    .Elements("RelationshipType")
                                                  select relationship.GetAs<string>()).ToList();

                    List<NameData> nameDataList = (from nameData in culture.Element("Names")
                                                    .Elements("NameData")
                                                   select new NameData(
                                                      nameData.Element("Name").GetAs<string>(),
                                                      nameData.Elements("Chain").Select(x => x.GetAs<int>()).ToArray(),
                                                      nameData.Elements("Sex").Select(x => x.GetAs<string>()).ToArray(),
                                                      nameData.Elements("Group").Select(x => x.GetAs<int>()).ToArray()
                                                      )).ToList();

                    Dictionary<string, int> sexualitiesDictionary = (from sexualities in culture.Element("Sexualities")
                                                                        .Elements("Sexuality")
                                                                     select new KeyValuePair<string, int>(
                                                                      sexualities.Element("Name").GetAs<string>(),
                                                                      sexualities.Element("Chance").GetAs<int>()
                                                                      )).ToDictionary(x => x.Key, x => x.Value);

                    Dictionary<string, int> romanceDictionary = (from romances in culture.Element("Romances")
                                .Elements("Romance")
                            select new KeyValuePair<string, int>(
                                romances.Element("Name").GetAs<string>(),
                                romances.Element("Chance").GetAs<int>()))
                        .ToDictionary(x => x.Key, x => x.Value);

                    Dictionary<string, int> sexesDictionary = (from sexes in culture.Element("Sexes")
                                                                    .Elements("Sex")
                                                               select new KeyValuePair<string, int>(
                                                              sexes.Element("Name").GetAs<string>(),
                                                              sexes.Element("Chance").GetAs<int>()))
                                                                   .ToDictionary(x => x.Key, x => x.Value);

                    Dictionary<string, int> genderDictionary = (from genders in culture.Element("Genders")
                                .Elements("Gender")
                            select new KeyValuePair<string, int>(
                                genders.Element("Name").GetAs<string>(),
                                genders.Element("Chance").GetAs<int>()))
                        .ToDictionary(x => x.Key, x => x.Value);
                    
                    Dictionary<string, Tuple<int, int>> statVarianceDictionary = (from statVariances in culture.Element("Statistics")
                                                                                    .Elements("StatVariance")
                                                                                  select new KeyValuePair<string, Tuple<int, int>>(
                                                                                    statVariances.Element("Name").GetAs<string>(),
                                                                                    new Tuple<int, int>(
                                                                                        statVariances.Element("Chance").GetAs<int>(),
                                                                                        statVariances.Element("Magnitude").GetAs<int>())))
                                                                                          .ToDictionary(x => x.Key, x => x.Value);

                    Dictionary<string, int> jobPrevelenceDictionary = (from jobPrevelence in culture.Element("Jobs")
                                                                            .Elements("Job")
                                                                       select new KeyValuePair<string, int>(
                                                                      jobPrevelence.Element("Name").GetAs<string>(),
                                                                      jobPrevelence.Element("Chance").GetAs<int>()))
                                                                           .ToDictionary(x => x.Key, x => x.Value);

                    string cultureName = culture.Element("CultureName").GetAs<string>();

                    int nonConformingGenderChance = culture.Element("NonConformingGenderChance").DefaultIfEmpty(10);

                    string tileSet = culture.Element("TileSet").Element("Name").DefaultIfEmpty("");
                    //string filename = culture.Element("TileSet").Element("Filename").DefaultIfEmpty("");

                    if (tileSet.Length > 0)
                    {
                        IconData[] icons = (from cultureIcons in culture.Element("TileSet").Elements("Icon")
                                            select new IconData()
                                            {
                                                name = cultureIcons.Element("Name").GetAs<string>(),
                                                data = cultureIcons.Element("Data").DefaultIfEmpty(""),
                                                frames = cultureIcons.Element("Frames").GetAs<int>(),
                                                filename = cultureIcons.Element("Filename").GetAs<string>(),
                                                position = cultureIcons.Element("Position").DefaultIfEmpty(0)
                                            }).ToArray();

                        objectIcons.AddIcons(tileSet, icons);
                    }

                    cultures.Add(cultureName,
                        new CultureType(
                            cultureName,
                            tileSet,
                            rulers,
                            crimes,
                            nameDataList,
                            jobPrevelenceDictionary,
                            inhabitants,
                            sexualitiesDictionary,
                            sexesDictionary,
                            statVarianceDictionary,
                            relationships,
                            romanceDictionary,
                            genderDictionary,
                            nonConformingGenderChance));
                }
            }

            return cultures;
        }

        public ICulture GetByCultureName(string name)
        {
            if(this.m_Cultures is null)
            {
                this.LoadCultures();
            }
            
            if (this.m_Cultures.ContainsKey(name))
            {
                return this.m_Cultures[name];
            }

            return null;
        }

        public List<ICulture> GetByCreatureType(string type)
        {
            if(this.m_Cultures is null)
            {
                this.LoadCultures();
            }

            try
            {
                Dictionary<string, ICulture> cultures = this.m_Cultures.Where(
                    culture => culture.Value.Inhabitants.Contains(
                        type, GlobalConstants.STRING_COMPARER))
                    .ToDictionary(pair => pair.Key, pair => pair.Value);
                return cultures.Values.ToList();
            }
            catch (Exception e)
            {
                GlobalConstants.ActionLog.AddText("Could not find a culture for creature type " + type);
                throw new InvalidOperationException("Could not find a culture for creature type " + type, e);
            }
        }

        public ICulture[] Cultures
        {
            get
            {
                if(this.m_Cultures is null)
                {
                    this.m_Cultures = this.LoadCultures();
                }
                return this.m_Cultures.Values.ToArray();
            }
        }
    }
}
