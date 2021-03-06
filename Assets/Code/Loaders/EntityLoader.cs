﻿using JoyLib.Code.Cultures;
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

            Dictionary<string, CultureType> cultures = CultureLoader.LoadCultures();

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
                    Dictionary<StatisticIndex, EntityStatistic> statistics;
                    Dictionary<string, EntitySkill> skills;
                    List<Ability> abilities;
                    List<string> slots;
                    VisionType visionType;

                    while (reader.Read())
                    {
                        if (reader.Depth == 0 && reader.NodeType == XmlNodeType.Element && !reader.Name.Equals("Entities"))
                            break;

                        if (reader.Depth == 1 && reader.NodeType == XmlNodeType.Element && !reader.Name.Equals("Entity"))
                            continue;

                        if (reader.Name.Equals(""))
                            continue;
                        if (reader.Name.Equals("Entity") && reader.NodeType == XmlNodeType.Element)
                        {

                            type = "DEFAULT TYPE";
                            creatureType = "DEFAULT CREATURE TYPE";
                            tileset = "DEFAULT";
                            sentient = false;
                            size = 1;
                            statistics = new Dictionary<StatisticIndex, EntityStatistic>(9);
                            skills = new Dictionary<string, EntitySkill>();
                            slots = new List<string>();

                            abilities = new List<Ability>();
                            visionType = VisionType.Diurnal;

                            do
                            {
                                reader.Read();
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
                                    StatisticIndex statIndex;
                                    statIndex = (StatisticIndex)Enum.Parse(typeof(StatisticIndex), reader.GetAttribute("Name"));

                                    int statValue;
                                    int.TryParse(reader.GetAttribute("Value"), out statValue);

                                    statistics.Add(statIndex, new EntityStatistic(statValue, GlobalConstants.DEFAULT_SUCCESS_THRESHOLD));
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
                                    do
                                    {
                                        reader.Read();
                                        if (reader.Name.Equals("Slot"))
                                        {
                                            slots.Add(reader.ReadElementContentAsString());
                                        }
                                    } while (!reader.Name.Equals("Slots") && reader.NodeType != XmlNodeType.EndElement);
                                }
                            } while (!reader.Name.Equals("Entity") && reader.NodeType != XmlNodeType.EndElement);

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
