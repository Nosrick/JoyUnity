using JoyLib.Code.Cultures;
using JoyLib.Code.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace JoyLib.Code.Loaders
{
    public static class CultureLoader
    {
        public static Dictionary<string, CultureType> LoadCultures()
        {
            string folderPath = Directory.GetCurrentDirectory() + "/Data/Cultures";
            string[] files = Directory.GetFiles(folderPath);

            Dictionary<string, CultureType> cultures = new Dictionary<string, CultureType>();

            for (int i = 0; i < files.Length; i++)
            {
                XmlReader reader = XmlReader.Create(files[i]);

                List<string> rulers = new List<string>();
                List<string> crimes = new List<string>();
                List<NameData> nameData = new List<NameData>();
                List<Gender> genders = new List<Gender>();
                string cultureName = "DEFAULT CULTURE";
                string inhabitants = "DEFAULT CREATURE";
                Dictionary<Sexuality, int> sexualities = new Dictionary<Sexuality, int>();
                int statVariance = 0;

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
                        string genderString = reader.GetAttribute("Gender");
                        Gender gender;

                        if (genderString.Equals("M"))
                            gender = Gender.Male;
                        else if (genderString.Equals("F"))
                            gender = Gender.Female;
                        else
                            gender = Gender.Neutral;

                        nameData.Add(new NameData(nameRef, false, gender));
                    }

                    if (reader.Name.Equals("Genders"))
                    {
                        string genderString = reader.ReadElementContentAsString();
                        string[] genderStrings = genderString.Split(',');
                        for (int j = 0; j < genderStrings.Length; j++)
                        {
                            if (genderStrings[j].Equals("F"))
                            {
                                genders.Add(Gender.Female);
                            }
                            else if (genderStrings[j].Equals("M"))
                            {
                                genders.Add(Gender.Male);
                            }
                            else
                            {
                                genders.Add(Gender.Neutral);
                            }
                        }
                    }

                    if (reader.Name.Equals("LastName"))
                    {
                        string nameRef = reader.GetAttribute("Name");
                        nameData.Add(new NameData(nameRef, true, Gender.Neutral));
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
                }

                reader.Close();
                cultures.Add(inhabitants, new CultureType(cultureName, rulers, crimes, nameData, inhabitants, genders, 
                    sexualities, statVariance));
            }

            return cultures;
        }
    }
}