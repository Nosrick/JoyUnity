using JoyLib.Code.Entities.Items;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace JoyLib.Code.Loaders.Items
{
    public static class FoodLoader
    {
        public static List<BaseItemType> LoadFood()
        {
            List<BaseItemType> food = new List<BaseItemType>();

            string filePath = Directory.GetCurrentDirectory() + "//Data//Items//Food//Food.xml";
            XmlReader reader = XmlReader.Create(filePath);

            List<IdentifiedItem> identifiedItems = new List<IdentifiedItem>();
            string actionWord = "strikes";
            string governingSkill = "None";
            string category = "None";

            while (reader.Read())
            {
                if (reader.Depth == 0 && reader.NodeType == XmlNodeType.Element && !reader.Name.Equals("Items"))
                    break;

                if (reader.Name.Equals("Category") && reader.NodeType == XmlNodeType.Element)
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
                            item.interactionFile = Directory.GetCurrentDirectory() + "//Data//Scripts//Items//Food//" + reader.ReadElementContentAsString();
                        }
                        else if (reader.Name.Equals("SpawnWeight"))
                        {
                            item.weighting = reader.ReadElementContentAsInt();
                        }
                        else if (reader.Name.Equals("Materials"))
                        {
                            string materials = reader.ReadElementContentAsString();
                            item.materials = materials.Split(',');
                        }
                        else if (reader.Name.Equals("Size"))
                        {
                            item.size = reader.ReadElementContentAsFloat();
                        }
                    }
                    item.category = category;
                    identifiedItems.Add(item);
                }
                else if (reader.Name.Equals("ActionWord"))
                {
                    actionWord = reader.ReadElementContentAsString();
                }
            }

            reader.Close();

            ItemMaterial foodMaterial = MaterialHandler.GetMaterial("Food");

            for (int i = 0; i < identifiedItems.Count; i++)
            {
                food.Add(new BaseItemType(identifiedItems[i].category, identifiedItems[i].description, identifiedItems[i].description, identifiedItems[i].name,
                    identifiedItems[i].name, "None", identifiedItems[i].size, foodMaterial, "Food", governingSkill, actionWord,
                    identifiedItems[i].interactionFile, identifiedItems[i].value, identifiedItems[i].weighting));
            }

            return food;
        }
    }
}
