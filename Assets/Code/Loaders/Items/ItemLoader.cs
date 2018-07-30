using JoyLib.Code.Entities.Items;
using JoyLib.Code.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace JoyLib.Code.Loaders.Items
{
    public static class ItemLoader
    {
        public static List<BaseItemType> LoadItems()
        {
            List<BaseItemType> weapons = new List<BaseItemType>();

            string folderPath = Directory.GetCurrentDirectory() + GlobalConstants.DATA_FOLDER + "Items";
            string[] files = Directory.GetFiles(folderPath, "*.xml", SearchOption.AllDirectories);


            for (int i = 0; i < files.Length; i++)
            {
                XmlReader reader = XmlReader.Create(files[i]);

                List<IdentifiedItem> identifiedItems = new List<IdentifiedItem>();
                List<UnidentifiedItem> unidentifiedItems = new List<UnidentifiedItem>();

                string actionWord = "strikes";
                string category = "Misc";
                int lightLevel = 0;

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
                        item.slot = "None";
                        item.skill = "None";
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
                                item.interactionFile = reader.ReadElementContentAsString();
                            }
                            else if(reader.Name.Equals("SpawnWeight"))
                            {
                                item.weighting = reader.ReadElementContentAsInt();
                            }
                            else if(reader.Name.Equals("Materials"))
                            {
                                string materials = reader.ReadElementContentAsString();
                                item.materials = materials.Split(',');
                            }
                            else if(reader.Name.Equals("Size"))
                            {
                                item.size = reader.ReadElementContentAsFloat();
                            }
                            else if(reader.Name.Equals("Slot"))
                            {
                                item.slot = reader.ReadElementContentAsString();
                            }
                            else if(reader.Name.Equals("Skill"))
                            {
                                item.skill = reader.ReadElementContentAsString();
                            }
                            else if(reader.Name.Equals("LightLevel"))
                            {
                                lightLevel = reader.ReadElementContentAsInt();
                            }
                        }
                        item.category = category;
                        identifiedItems.Add(item);
                    }
                    else if (reader.Name.Equals("UnidentifiedItem"))
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
                    else if (reader.Name.Equals("ActionWord"))
                    {
                        actionWord = reader.ReadElementContentAsString();
                    }
                }

                reader.Close();

                for (int j = 0; j < identifiedItems.Count; j++)
                {
                    UnidentifiedItem chosenDescription = new UnidentifiedItem(identifiedItems[j].name, identifiedItems[j].description);

                    if (unidentifiedItems.Count != 0)
                    {
                        int index = RNG.Roll(0, unidentifiedItems.Count - 1);
                        chosenDescription = unidentifiedItems[index];
                        unidentifiedItems.RemoveAt(index);
                    }

                    for (int k = 0; k < identifiedItems[j].materials.Length; k++)
                    {

                        weapons.Add(new BaseItemType(identifiedItems[j].category, identifiedItems[j].description, chosenDescription.description, chosenDescription.name,
                            identifiedItems[j].name, identifiedItems[j].slot, identifiedItems[j].size,
                            MaterialHandler.GetMaterial(identifiedItems[j].materials[k]),
                            identifiedItems[j].category, identifiedItems[j].skill, actionWord, identifiedItems[j].interactionFile,
                            identifiedItems[j].value, identifiedItems[j].weighting, lightLevel));
                    }
                }
            }

            return weapons;
        }
    }
}
