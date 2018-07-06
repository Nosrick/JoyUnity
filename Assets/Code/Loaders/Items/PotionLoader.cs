using JoyLib.Code.Entities.Items;
using JoyLib.Code.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace JoyLib.Code.Loaders.Items
{
    public static class PotionLoader
    {
        public static List<BaseItemType> LoadPotions()
        {
            List<BaseItemType> potions = new List<BaseItemType>();

            string filePath = Directory.GetCurrentDirectory() + "//Data//Items//Potions//Potions.xml";
            XmlReader reader = XmlReader.Create(filePath);

            List<IdentifiedItem> identifiedItems = new List<IdentifiedItem>();
            List<UnidentifiedItem> unidentifiedItems = new List<UnidentifiedItem>();
            string actionWord = "strikes";
            string governingSkill = "None";
            string category = "None";

            while (reader.Read())
            {
                if (reader.Depth == 0 && reader.NodeType == XmlNodeType.Element && !reader.Name.Equals("Items"))
                    break;

                if(reader.Name.Equals("Category") && reader.NodeType == XmlNodeType.Element)
                {
                    category = reader.ReadElementContentAsString();
                }

                if (reader.Name.Equals("IdentifiedItem") && reader.NodeType == XmlNodeType.Element)
                {
                    IdentifiedItem item = new IdentifiedItem();
                    while (reader.NodeType != XmlNodeType.EndElement)
                    {
                        reader.Read();
                        if (reader.Name.Equals("Name"))
                        {
                            item.name = reader.ReadElementContentAsString();
                        }
                        else if (reader.Name.Equals("Description"))
                        {
                            item.description = reader.ReadElementContentAsString();
                        }
                        else if (reader.Name.Equals("Value"))
                        {
                            item.value = reader.ReadElementContentAsInt();
                        }
                        else if (reader.Name.Equals("Effect"))
                        {
                            item.interactionFile = Directory.GetCurrentDirectory() + "//Data//Scripts//Items//Potions//" + reader.ReadElementContentAsString();
                        }
                        else if(reader.Name.Equals("SpawnWeight"))
                        {
                            item.weighting = reader.ReadElementContentAsInt();
                        }
                    }
                    item.category = category;
                    identifiedItems.Add(item);
                }
                else if (reader.Name.Equals("UnidentifiedItem") && reader.NodeType == XmlNodeType.Element)
                {
                    UnidentifiedItem item = new UnidentifiedItem();
                    while (reader.NodeType != XmlNodeType.EndElement)
                    {
                        reader.Read();
                        if (reader.Name.Equals("Name"))
                        {
                            item.name = reader.ReadElementContentAsString();
                        }
                        else if (reader.Name.Equals("Description"))
                        {
                            item.description = reader.ReadElementContentAsString();
                        }
                    }
                    unidentifiedItems.Add(item);
                }
                else if(reader.Name.Equals("ActionWord"))
                {
                    actionWord = reader.ReadElementContentAsString();
                }
            }

            reader.Close();

            ItemMaterial glass = MaterialHandler.GetMaterial("Glass");

            for (int i = 0; i < identifiedItems.Count; i++)
            {
                int index = RNG.Roll(0, unidentifiedItems.Count - 1);
                UnidentifiedItem chosenDescription = unidentifiedItems[index];
                unidentifiedItems.RemoveAt(index);

                potions.Add(new BaseItemType(identifiedItems[i].category, identifiedItems[i].description, chosenDescription.description, chosenDescription.name, 
                    identifiedItems[i].name, "None", 5.0f, glass, "Potions", governingSkill, actionWord, 
                    identifiedItems[i].interactionFile, identifiedItems[i].value, identifiedItems[i].weighting));
            }

            return potions;
        }
    }
}