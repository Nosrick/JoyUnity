using JoyLib.Code.Entities.Abilities;
using JoyLib.Code.Entities.Jobs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace JoyLib.Code.Loaders
{
    public static class JobTypeLoader
    {
        public static List<JobType> LoadTypes()
        {
            List<JobType> returnTypes = new List<JobType>();

            string directory = Directory.GetCurrentDirectory() + GlobalConstants.DATA_FOLDER + "Jobs";
            string[] files = Directory.GetFiles(directory, "*.xml", SearchOption.AllDirectories);

            for (int i = 0; i < files.Length; i++)
            {
                try
                {
                    string name = "UNNAMED CLASS";
                    string description = "DESCRIPTION MISSING";

                    List<Tuple<int, Ability>> abilities = new List<Tuple<int, Ability>>();

                    XmlReader reader = XmlReader.Create(files[i]);

                    List<char> chars = new List<char>();
                    int j = files[i].Length - 1;
                    while (true)
                    {
                        //Search backwards through the string to find out if it's a letter or a dot
                        if (char.IsLetterOrDigit(files[i][j]) || files[i][j] == '.' || files[i][j] == ' ')
                            chars.Add(files[i][j]);
                        else
                            break;

                        j -= 1;
                    }

                    StringBuilder builder = new StringBuilder();
                    for (int k = chars.Count - 1; k > 3; k--)
                    {
                        builder.Append(chars[k]);
                    }
                    name = builder.ToString();

                    while (reader.Read())
                    {
                        if (reader.Depth == 0 && reader.NodeType == XmlNodeType.Element && !reader.Name.Equals("Job"))
                            break;

                        if (reader.Name.Equals(""))
                            continue;
                        
                        else if (reader.Name.Equals("Ability"))
                        {
                            string abilityName = "DEFAULT ABILITY";
                            int level = 1;
                            abilityName = reader.GetAttribute("Name");
                            level = int.Parse(reader.GetAttribute("Level"));

                            Ability ability = AbilityHandler.GetAbility(abilityName);
                            if(ability != null)
                            {
                                abilities.Add(new Tuple<int, Ability>(level, ability));
                            }
                        }
                        else if (reader.Name.Equals("Description"))
                        {
                            description = reader.ReadElementContentAsString();
                        }
                    }

                    reader.Close();

                    returnTypes.Add(new JobType(name, description, abilities));
                }
                catch (Exception e)
                {
                    Console.Out.WriteLine(e.Message);
                    Console.Out.WriteLine(e.StackTrace);
                }
            }

            return returnTypes;
        }
    }
}
