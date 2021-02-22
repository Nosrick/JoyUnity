﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Castle.Core.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            
            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory() + GlobalConstants.DATA_FOLDER + "Materials", "*.json", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                using (StreamReader reader = new StreamReader(file))
                {
                    using (JsonTextReader jsonReader = new JsonTextReader(reader))
                    {
                        try
                        {
                            JObject jToken = JObject.Load(jsonReader);

                            if (jToken.IsNullOrEmpty())
                            {
                                continue;
                            }

                            foreach (JToken child in jToken["Materials"])
                            {
                                string name = (string) child["Name"];
                                float hardness = (float) (child["Hardness"] ?? 1.0f);
                                int bonus = (int) (child["Bonus"] ?? 1);
                                float weight = (float) (child["Weight"] ?? 1.0f);
                                float value = (float) (child["Value"] ?? 1.0f);

                                materials.Add(
                                    new ItemMaterial(
                                        name,
                                        hardness,
                                        bonus,
                                        weight,
                                        value));
                            }
                        }
                        catch (Exception e)
                        {
                            GlobalConstants.ActionLog.AddText("Error loading materials from " + file);
                            GlobalConstants.ActionLog.StackTrace(e);
                        }
                        finally
                        {
                            jsonReader.Close();
                            reader.Close();
                        }
                    }
                }
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
