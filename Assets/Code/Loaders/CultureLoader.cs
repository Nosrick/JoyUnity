using JoyLib.Code.Cultures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

namespace JoyLib.Code.Loaders
{
    public static class CultureLoader
    {
        public static Dictionary<string, CultureType> LoadCultures()
        {
            string folderPath = Directory.GetCurrentDirectory() + GlobalConstants.DATA_FOLDER + "Cultures";
            string[] files = Directory.GetFiles(folderPath, "*.xml");

            Dictionary<string, CultureType> cultures = new Dictionary<string, CultureType>();

            for (int i = 0; i < files.Length; i++)
            {
                XmlReader reader = XmlReader.Create(files[i]);

                List<string> rulers = new List<string>();
                List<string> crimes = new List<string>();
                List<NameData> nameData = new List<NameData>();
                Dictionary<string, int> sexes = new Dictionary<string, int>();
                string cultureName = "DEFAULT CULTURE";
                List<string> inhabitants = new List<string>();
                Dictionary<string, int> sexualities = new Dictionary<string, int>();
                Dictionary<string, int> statVariances = new Dictionary<string, int>();
                List<string> relationships = new List<string>();
                Dictionary<string, int> jobPrevelence = new Dictionary<string, int>();

                while (reader.Read())
                {
                    if (reader.Depth == 0 && reader.NodeType == XmlNodeType.Element && !reader.Name.Equals("Culture"))
                        break;

                    if (reader.Name.Equals(""))
                        continue;

                    if (reader.Name.Equals("CultureName"))
                    {
                        cultureName = reader.ReadElementContentAsString();
                    }

                    if (reader.Name.Equals("CreatureName"))
                    {
                        inhabitants.Add(reader.ReadElementContentAsString());
                    }

                    if (reader.Name.Equals("Ruler"))
                    {
                        rulers.Add(reader.ReadElementContentAsString());
                    }

                    if (reader.Name.Equals("Crime"))
                    {
                        crimes.Add(reader.ReadElementContentAsString());
                    }

                    if(reader.Name.Equals("Job"))
                    {
                        string name = "DEFAULT";
                        int prevelence = 0;

                        while(reader.Read())
                        {
                            if(reader.Name.Equals("Job") && reader.NodeType == XmlNodeType.EndElement)
                            {
                                break;
                            }
                            else if(reader.Name.Equals("Name"))
                            {
                                name = reader.ReadElementContentAsString();
                            }
                            else if(reader.Name.Equals("Chance"))
                            {
                                prevelence = reader.ReadElementContentAsInt();
                            }
                        }

                        if(jobPrevelence.ContainsKey(name) == false && prevelence > 0)
                        {
                            jobPrevelence.Add(name, prevelence);
                        }
                    }

                    if (reader.Name.Equals("NameData"))
                    {
                        string nameRef = "DEFAULT";
                        List<string> sexStrings = new List<string>();
                        List<int> chain = new List<int>();

                        while (reader.Read())
                        {
                            if(reader.Name.Equals("NameData") && reader.NodeType == XmlNodeType.EndElement)
                            {
                                break;
                            }
                            else if(reader.Name.Equals("Name"))
                            {
                                nameRef = reader.ReadElementContentAsString();
                            }
                            else if(reader.Name.Equals("Sex"))
                            {
                                sexStrings.Add(reader.ReadElementContentAsString());
                            }
                            else if(reader.Name.Equals("Chain"))
                            {
                                chain.Add(reader.ReadElementContentAsInt());
                            }
                        }

                        nameData.Add(new NameData(nameRef, chain.ToArray(), sexStrings.ToArray()));
                    }

                    if (reader.Name.Equals("Sex"))
                    {
                        string sexName = "DEFAULT";
                        int sexChance = 0;
                        while(reader.Read())
                        {
                            if(reader.Name.Equals("Sex") && reader.NodeType == XmlNodeType.EndElement)
                            {
                                break;
                            }

                            if(reader.Name.Equals("Name"))
                            {
                                sexName = reader.ReadElementContentAsString();
                            }
                            else if(reader.Name.Equals("Chance"))
                            {
                                sexChance = reader.ReadElementContentAsInt();
                            }
                        }

                        if(sexes.ContainsKey(sexName) == false && sexChance > 0)
                        {
                            sexes.Add(sexName, sexChance);
                        }
                    }

                    if(reader.Name.Equals("Sexuality"))
                    {
                        string sexualityName = "DEFAULT";
                        int sexualityChance = 5;
                        while (reader.Read())
                        {
                            if(reader.Name.Equals("Sexuality") && reader.NodeType == XmlNodeType.EndElement)
                            {
                                break;
                            }

                            if(reader.Name.Equals("Name"))
                            {
                                sexualityName = reader.ReadElementContentAsString();
                            }
                            else if(reader.Name.Equals("Chance"))
                            {
                                sexualityChance = reader.ReadElementContentAsInt();
                            }
                        }

                        sexualities.Add(sexualityName, sexualityChance);
                    }

                    if(reader.Name.Equals("StatVariance"))
                    {
                        string statName = "DEFAULT";
                        int variance = 0;
                        while(reader.Read())
                        {
                            if(reader.NodeType == XmlNodeType.EndElement && reader.Name.Equals("StatVariance"))
                            {
                                break;
                            }

                            if(reader.Name.Equals("Statistic"))
                            {
                                statName = reader.ReadElementContentAsString();
                            }
                            else if(reader.Name.Equals("Variance"))
                            {
                                variance = reader.ReadElementContentAsInt();
                            }
                        }
                        if(statVariances.ContainsKey(statName) == false)
                        {
                            statVariances.Add(statName, variance);
                        }
                    }

                    if(reader.Name.Equals("RelationshipType"))
                    {
                        try
                        {
                            relationships.Add(reader.ReadElementContentAsString());
                        }
                        catch(Exception e)
                        {
                            Debug.LogError(e.Message);
                            Debug.LogError(e.StackTrace);
                        }
                    }
                }

                reader.Close();
                cultures.Add(cultureName, new CultureType(cultureName, rulers, crimes, nameData, jobPrevelence, inhabitants,
                    sexualities, sexes, statVariances, relationships));
            }

            return cultures;
        }
    }
}