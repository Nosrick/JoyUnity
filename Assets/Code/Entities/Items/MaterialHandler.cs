using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using JoyLib.Code.Helpers;

namespace JoyLib.Code.Entities.Items
{
    public class MaterialHandler : IMaterialHandler
    {
        protected Dictionary<string, IItemMaterial> m_Materials;

        public IEnumerable<IItemMaterial> Values => this.m_Materials.Values;

        public MaterialHandler()
        {
            this.Initialise();
        }

        public void Initialise()
        {
            this.m_Materials = this.Load().ToDictionary(material => material.Name, material => material);

            this.m_Materials.Add("DEFAULT MATERIAL", new ItemMaterial("DEFAULT MATERIAL", 0.1f, 0, 1.0f, 0.0f));
        }
        
        public IItemMaterial Get(string name)
        {
            if(this.m_Materials is null)
            {
                this.Initialise();
            }

            if (this.m_Materials.Any(pair => pair.Value.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                return this.m_Materials.First(pair => pair.Value.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).Value;
            }

            return this.m_Materials["DEFAULT MATERIAL"];
        }

        public IEnumerable<IItemMaterial> Load()
        {
            List<IItemMaterial> materials = new List<IItemMaterial>();
            
            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory() + GlobalConstants.DATA_FOLDER + "Materials", "*.xml", SearchOption.AllDirectories);

            for (int i = 0; i < files.Length; i++)
            {
                XElement reader = XElement.Load(files[i]);
                
                materials.AddRange(
                    (from material in reader.Elements("Material")
                        select new ItemMaterial(
                            material.Element("Name").GetAs<string>(),
                            material.Element("Hardness").DefaultIfEmpty(1.0f),
                            material.Element("Bonus").DefaultIfEmpty(0),
                            material.Element("Weight").GetAs<float>(),
                            material.Element("Value").DefaultIfEmpty(1.0f))));
            }

            return materials;
        }

        public void Dispose()
        {
            string[] keys = this.m_Materials.Keys.ToArray();
            foreach (string key in keys)
            {
                this.m_Materials[key] = null;
            }

            this.m_Materials = null;
        }
    }
}
