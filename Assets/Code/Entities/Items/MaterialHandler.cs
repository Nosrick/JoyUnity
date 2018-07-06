using JoyLib.Code.Loaders;
using System.Collections.Generic;

namespace JoyLib.Code.Entities.Items
{
    public static class MaterialHandler
    {
        private static Dictionary<string, ItemMaterial> s_Materials;

        public static void Initialise()
        {
            List<ItemMaterial> flatList = MaterialLoader.LoadMaterials();
            s_Materials = new Dictionary<string, ItemMaterial>(flatList.Count);

            foreach(ItemMaterial material in flatList)
            {
                s_Materials.Add(material.name, material);
            }

            s_Materials.Add("DEFAULT MATERIAL", new ItemMaterial("DEFAULT MATERIAL", 0.1f, 0, 1.0f));
        }

        public static ItemMaterial GetMaterial(string nameRef)
        {
            if (s_Materials.ContainsKey(nameRef))
                return s_Materials[nameRef];

            return s_Materials["DEFAULT MATERIAL"];
        }
    }
}
