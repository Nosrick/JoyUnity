using JoyLib.Code.Cultures;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Abilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

namespace JoyLib.Code.Loaders
{
    public static class EntityLoader
    {
        public static List<EntityTemplate> LoadTypes()
        {
            List<EntityTemplate> entities = new List<EntityTemplate>();

            string directory = Directory.GetCurrentDirectory() + GlobalConstants.DATA_FOLDER + "Entities";
            string[] files = Directory.GetFiles(directory, "*.xml", SearchOption.AllDirectories);

            for (int i = 0; i < files.Length; i++)
            {
                try
                {
                    XmlReader reader = XmlReader.Create(files[i]);

                    string type;
                    string creatureType;
                    string tileset;
                    bool sentient;
                    int size;
                    Dictionary<string, EntityStatistic> statistics;
                    Dictionary<string, EntitySkill> skills;
                    List<Ability> abilities;
                    List<string> slots;
                    VisionType visionType;

                    while (reader.Read())
                    {
                        if (reader.Depth == 0 && reader.NodeType == XmlNodeType.Element && !reader.Name.Equals("Entities"))
                        {
                            break;
                        }

                        else if (reader.Name.Equals(""))
                        {
                            continue;
                        }
                            
                        else if (reader.Name.Equals("Entity") && reader.NodeType == XmlNodeType.Element)
                        {

                            type = "DEFAULT TYPE";
                            creatureType = "DEFAULT CREATURE TYPE";
                            tileset = "DEFAULT";
                            sentient = false;
                            size = 0;
                            statistics = new Dictionary<string, EntityStatistic>();
                            skills = new Dictionary<string, EntitySkill>();
                            slots = new List<string>();

                            abilities = new List<Ability>();
                            visionType = VisionType.Diurnal;

                            while(reader.Read())
                            {
                                if(reader.NodeType == XmlNodeType.EndElement && reader.Name.Equals("Entity"))
                                {
                                    break;
                                }

                                if (reader.Name.Equals("CreatureType"))
                                {
                                    creatureType = reader.ReadElementContentAsString();
                                }
                                else if (reader.Name.Equals("Sentient"))
                                {
                                    string sentience = reader.ReadElementContentAsString();
                                    if (sentience.Equals("Yes"))
                                    {
                                        sentient = true;
                                    }
                                    else
                                    {
                                        sentient = false;
                                    }
                                }
                                else if (reader.Name.Equals("Type"))
                                {
                                    type = reader.ReadElementContentAsString();
                                }
                                else if (reader.Name.Equals("Tileset"))
                                {
                                    tileset = reader.ReadElementContentAsString();
                                }
                                else if (reader.Name.Equals("Statistic"))
                                {
                                    string statIndex = "DEFAULT";
                                    int statValue = 4;
                                    int successThreshold = 7;
                                    while (reader.Read())
                                    {
                                        if (reader.NodeType == XmlNodeType.EndElement && reader.Name.Equals("Statistic"))
                                        {
                                            break;
                                        }
                                        else if (reader.Name.Equals(""))
                                        {
                                            continue;
                                        }
                                        else if (reader.Name.Equals("Name"))
                                        {
                                            statIndex = reader.ReadElementContentAsString();
                                        }
                                        else if (reader.Name.Equals("Value"))
                                        {
                                            statValue = reader.ReadElementContentAsInt();
                                        }
                                        else if(reader.Name.Equals("Threshold"))
                                        {
                                            successThreshold = reader.ReadElementContentAsInt();
                                        }
                                    }

                                    statistics.Add(statIndex, new EntityStatistic(statValue, successThreshold));
                                }
                                else if(reader.Name.Equals("VisionType"))
                                {
                                    visionType = (VisionType)Enum.Parse(typeof(VisionType), reader.ReadElementContentAsString());
                                }
                                else if(reader.Name.Equals("Size"))
                                {
                                    size = reader.ReadElementContentAsInt();
                                }
                                else if(reader.Name.Equals("Slots"))
                                {
                                    slots = new List<string>();
                                    while(reader.Read())
                                    {
                                        if(reader.NodeType == XmlNodeType.EndElement && reader.Name.Equals("Slots"))
                                        {
                                            break;
                                        }

                                        else if (reader.Name.Equals("Slot"))
                                        {
                                            slots.Add(reader.ReadElementContentAsString());
                                        }
                                    }
                                }
                            }

                            entities.Add(new EntityTemplate(statistics, skills, abilities, slots, size, sentient, visionType, creatureType, type, tileset));
                        }
                    }

                    reader.Close();
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                    Debug.LogError(e.StackTrace);
                }
            }

            return entities;
        }
    }
}
