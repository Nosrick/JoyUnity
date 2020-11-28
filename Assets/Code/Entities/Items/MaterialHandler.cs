﻿using System.Collections.Generic;
using System.IO;
using System.Xml;
using System;
using System.Linq;
using System.Xml.Linq;
using JoyLib.Code.Helpers;
using UnityEngine;


namespace JoyLib.Code.Entities.Items
{
    public class MaterialHandler : IMaterialHandler
    {
        private Dictionary<string, IItemMaterial> m_Materials;

        public MaterialHandler()
        {
            Initialise();
        }

        public void Initialise()
        {
            List<IItemMaterial> flatList = LoadMaterials();
            m_Materials = new Dictionary<string, IItemMaterial>(flatList.Count);

            foreach(IItemMaterial material in flatList)
            {
                m_Materials.Add(material.Name, material);
            }

            m_Materials.Add("DEFAULT MATERIAL", new ItemMaterial("DEFAULT MATERIAL", 0.1f, 0, 1.0f, 0.0f));
        }

        public List<IItemMaterial> LoadMaterials()
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

        public IItemMaterial GetMaterial(string nameRef)
        {
            if(m_Materials is null)
            {
                Initialise();
            }

            if (m_Materials.Any(pair => pair.Value.Name.Equals(nameRef, StringComparison.OrdinalIgnoreCase)))
            {
                return m_Materials.First(pair => pair.Value.Name.Equals(nameRef, StringComparison.OrdinalIgnoreCase)).Value;
            }

            return m_Materials["DEFAULT MATERIAL"];
        }
    }
}
