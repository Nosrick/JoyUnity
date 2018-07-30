using JoyLib.Code.Entities.Items;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace JoyLib.Code.Loaders
{
    public static class MaterialLoader
    {
        public static List<ItemMaterial> LoadMaterials()
        {
            List<ItemMaterial> materials = new List<ItemMaterial>();

            string folderPath = Directory.GetCurrentDirectory() + GlobalConstants.DATA_FOLDER + "Materials";
            string[] files = Directory.GetFiles(folderPath, "*.xml", SearchOption.AllDirectories);

            for (int i = 0; i < files.Length; i++)
            {
                XmlReader reader = XmlReader.Create(files[i]);

                while(reader.Read())
                {
                    if (reader.Depth == 0 && reader.NodeType == XmlNodeType.Element && !reader.Name.Equals("Materials"))
                        break;

                    if(reader.Name.Equals("Material"))
                    {
                        string name = "DEFAULT MATERIAL";
                        float hardness = 1;
                        int bonus = 0;
                        float weight = 1.0f;
                        while(reader.NodeType != XmlNodeType.EndElement)
                        {
                            reader.Read();
                            if(reader.Name.Equals("Name"))
                            {
                                name = reader.ReadElementContentAsString();
                            }
                            else if(reader.Name.Equals("Bonus"))
                            {
                                bonus = reader.ReadElementContentAsInt();
                            }
                            else if(reader.Name.Equals("Hardness"))
                            {
                                hardness = reader.ReadElementContentAsFloat();
                            }
                            else if(reader.Name.Equals("Weight"))
                            {
                                weight = reader.ReadElementContentAsFloat();
                            }
                        }

                        materials.Add(new ItemMaterial(name, hardness, bonus, weight));
                    }
                }

                reader.Close();
            }

            return materials;
        }
    }
}
