using JoyLib.Code.Cultures;
using JoyLib.Code.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
                List<Sex> sexs = new List<Sex>();
                string cultureName = "DEFAULT CULTURE";
                string inhabitants = "DEFAULT CREATURE";
                Dictionary<Sexuality, int> sexualities = new Dictionary<Sexuality, int>();
                int statVariance = 0;
                RelationshipType relationshipType = RelationshipType.Monoamorous;

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
                        inhabitants = reader.ReadElementContentAsString();
                    }

                    if (reader.Name.Equals("Ruler"))
                    {
                        rulers.Add(reader.ReadElementContentAsString());
                    }

                    if (reader.Name.Equals("Crime"))
                    {
                        crimes.Add(reader.ReadElementContentAsString());
                    }

                    if (reader.Name.Equals("FirstName"))
                    {
                        string nameRef = reader.GetAttribute("Name");
                        string sexString = reader.GetAttribute("Sex");
                        Sex sex;

                        if (sexString.Equals("M"))
                        {
                            sex = Sex.Male;
                        }
                        else if (sexString.Equals("F"))
                        {
                            sex = Sex.Female;
                        }
                        else
                        {
                            sex = Sex.Neutral;
                        }

                        nameData.Add(new NameData(nameRef, false, sex));
                    }

                    if (reader.Name.Equals("Sexes"))
                    {
                        string sexString = reader.ReadElementContentAsString();
                        string[] sexStrings = sexString.Split(',');
                        for (int j = 0; j < sexStrings.Length; j++)
                        {
                            if (sexStrings[j].Equals("F"))
                            {
                                sexs.Add(Sex.Female);
                            }
                            else if (sexStrings[j].Equals("M"))
                            {
                                sexs.Add(Sex.Male);
                            }
                            else
                            {
                                sexs.Add(Sex.Neutral);
                            }
                        }
                    }

                    if (reader.Name.Equals("LastName"))
                    {
                        string nameRef = reader.GetAttribute("Name");
                        nameData.Add(new NameData(nameRef, true, Sex.Neutral));
                    }

                    if(reader.Name.Equals("Sexuality"))
                    {
                        string sexualitiesRawString = reader.ReadElementContentAsString();
                        string[] sexualitiesStrings = sexualitiesRawString.Split(',');
                        sexualities.Add(Sexuality.Heterosexual, int.Parse(sexualitiesStrings[0]));
                        sexualities.Add(Sexuality.Homosexual, int.Parse(sexualitiesStrings[1]));
                        sexualities.Add(Sexuality.Bisexual, int.Parse(sexualitiesStrings[2]));
                        sexualities.Add(Sexuality.Asexual, int.Parse(sexualitiesStrings[3]));
                    }

                    if(reader.Name.Equals("StatVariance"))
                    {
                        statVariance = reader.ReadElementContentAsInt(); 
                    }

                    if(reader.Name.Equals("RelationshipType"))
                    {
                        try
                        {
                            relationshipType = (RelationshipType)Enum.Parse(typeof(RelationshipType), reader.ReadElementContentAsString(), true);
                        }
                        catch(Exception e)
                        {
                            Debug.LogError(e.Message);
                            Debug.LogError(e.StackTrace);
                        }
                    }
                }

                reader.Close();
                cultures.Add(inhabitants, new CultureType(cultureName, rulers, crimes, nameData, inhabitants, sexs, 
                    sexualities, statVariance, relationshipType));
            }

            return cultures;
        }
    }
}