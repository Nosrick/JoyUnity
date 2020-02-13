using System.Collections.Generic;
using System.IO;
using System.Xml;
using System;

namespace JoyLib.Code.Entities.Items
{
    public class MaterialHandler
    {
        private static readonly Lazy<MaterialHandler> lazy = new Lazy<MaterialHandler>(() => new MaterialHandler());

        public static MaterialHandler instance => lazy.Value;

        private static Dictionary<string, ItemMaterial> m_Materials;

        public MaterialHandler()
        {
            List<ItemMaterial> flatList = LoadMaterials();
            m_Materials = new Dictionary<string, ItemMaterial>(flatList.Count);

            foreach(ItemMaterial material in flatList)
            {
                m_Materials.Add(material.Name, material);
            }

            m_Materials.Add("DEFAULT MATERIAL", new ItemMaterial("DEFAULT MATERIAL", 0.1f, 0, 1.0f, 0.0f));
        }

        public List<ItemMaterial> LoadMaterials()
        {
            List<ItemMaterial> materials = new List<ItemMaterial>();
            
            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory() + GlobalConstants.DATA_FOLDER + "Materials", "*.xml", SearchOption.AllDirectories);

            for (int i = 0; i < files.Length; i++)
            {
                XmlReader reader = XmlReader.Create(files[i]);

                while (reader.Read())
                {
                    if (reader.Depth == 0 && reader.NodeType == XmlNodeType.Element && !reader.Name.Equals("Materials"))
                        break;

                    if (reader.Name.Equals("Material"))
                    {
                        string name = "DEFAULT MATERIAL";
                        float hardness = 1;
                        int bonus = 0;
                        float weight = 1.0f;
                        float value = 1.0f;

                        while (reader.NodeType != XmlNodeType.EndElement)
                        {
                            reader.Read();
                            if (reader.Name.Equals("Name"))
                            {
                                name = reader.ReadElementContentAsString();
                            }
                            else if (reader.Name.Equals("Bonus"))
                            {
                                bonus = reader.ReadElementContentAsInt();
                            }
                            else if (reader.Name.Equals("Hardness"))
                            {
                                hardness = reader.ReadElementContentAsFloat();
                            }
                            else if (reader.Name.Equals("Weight"))
                            {
                                weight = reader.ReadElementContentAsFloat();
                            }
                            else if (reader.Name.Equals("Value"))
                            {
                                value = reader.ReadElementContentAsFloat();
                            }
                        }

                        materials.Add(new ItemMaterial(name, hardness, bonus, weight, value));
                    }
                }

                reader.Close();
            }

            return materials;
        }

        public ItemMaterial GetMaterial(string nameRef)
        {
            if (m_Materials.ContainsKey(nameRef))
                return m_Materials[nameRef];

            return m_Materials["DEFAULT MATERIAL"];
        }
    }
}
