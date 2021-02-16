using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Castle.Core.Internal;
using JoyLib.Code.Entities.Abilities;
using JoyLib.Code.Graphics;
using JoyLib.Code.Helpers;
using JoyLib.Code.Rollers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JoyLib.Code.Entities.Items
{
    public class ItemDatabase : IItemDatabase
    {
        protected List<BaseItemType> m_ItemDatabase;

        protected IObjectIconHandler ObjectIcons { get; set; }

        protected IMaterialHandler MaterialHandler { get; set; }

        protected IAbilityHandler AbilityHandler { get; set; }

        public IEnumerable<BaseItemType> Values => this.m_ItemDatabase;
        
        protected IRollable Roller { get; set; }

        public ItemDatabase(
            IObjectIconHandler objectIconHandler,
            IMaterialHandler materialHandler,
            IAbilityHandler abilityHandler,
            IRollable roller = null)
        {
            this.ObjectIcons = objectIconHandler;
            this.MaterialHandler = materialHandler;
            this.AbilityHandler = abilityHandler;
            this.Roller = roller ?? new RNG();
            
            this.m_ItemDatabase = this.Load().ToList();
        }
        
        public IEnumerable<BaseItemType> Load()
        {
            List<BaseItemType> items = new List<BaseItemType>();

            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory() + GlobalConstants.DATA_FOLDER + "Items", "*.json", SearchOption.AllDirectories);

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

                            foreach (JToken child in jToken.Values())
                            {
                                List<IdentifiedItem> identifiedItems = new List<IdentifiedItem>();

                                foreach (JToken identifiedToken in child["IdentifiedItems"])
                                {
                                    string name = (string) identifiedToken["Name"];
                                    string description = ((string) identifiedToken["Description"]) ?? "";
                                    int value = (int) identifiedToken["Value"];
                                    IEnumerable<string> materials =
                                        identifiedToken["Materials"].Select(token => (string) token);

                                    int size = (int) identifiedToken["Size"];
                                    int lightLevel = (int) (identifiedToken["LightLevel"] ?? 0);
                                    int spawnWeight = (int) (identifiedToken["SpawnWeight"] ?? 1);
                                    IEnumerable<string> tags =
                                        identifiedToken["Tags"].Select(token => (string) token);
                                    string tileSet = (string) identifiedToken["TileSet"];
                                    IEnumerable<string> abilityNames = identifiedToken["Effects"].IsNullOrEmpty() 
                                                                        ? new List<string>() 
                                                                        : identifiedToken["Effects"].Select(token => (string) token);
                                    IEnumerable<IAbility> abilities = abilityNames.Select(abilityName => this.AbilityHandler.GetAbility(abilityName));
                                    IEnumerable<string> slots = identifiedToken["Slots"].IsNullOrEmpty()
                                        ? new List<string>() 
                                        : identifiedToken["Slots"].Select(token => (string) token);
                                    IEnumerable<string> skills = identifiedToken["Skill"].IsNullOrEmpty()
                                        ? new[] { "none" }
                                        : identifiedToken["Skill"].Select(token => (string) token);

                                    identifiedItems.Add(new IdentifiedItem(
                                        name,
                                        tags.ToArray(),
                                        description,
                                        value,
                                        abilities.ToArray(),
                                        spawnWeight,
                                        skills,
                                        materials.ToArray(),
                                        size,
                                        slots.ToArray(),
                                        tileSet,
                                        lightLevel));
                                }

                                List<UnidentifiedItem> unidentifiedItems = new List<UnidentifiedItem>();

                                if (child["UnidentifiedItems"].IsNullOrEmpty() == false)
                                {
                                    foreach (JToken unidentifiedToken in child["UnidentifiedItems"])
                                    {
                                        string name = (string) unidentifiedToken["Name"];
                                        string description = ((string) unidentifiedToken["Description"]) ?? "";
                                        string identifiedName = (string) unidentifiedToken["IdentifiedName"];

                                        unidentifiedItems.Add(new UnidentifiedItem(name, description, identifiedName));
                                    }
                                }

                                JToken tileSetToken = child["TileSet"];
                                string tileSetName = (string) tileSetToken["Name"];

                                string actionWord = ((string) child["ActionWord"]) ?? "strikes";

                                this.ObjectIcons.AddSpriteDataFromJson(tileSetName, tileSetToken["SpriteData"]);

                                foreach (IdentifiedItem identifiedItem in identifiedItems)
                                {
                                    string[] materials = identifiedItem.materials.ToArray();
                                    for (int i = 0; i < materials.Length; i++)
                                    {
                                        UnidentifiedItem[] possibilities = unidentifiedItems.Where(item =>
                                                item.identifiedName.Equals(identifiedItem.name,
                                                    StringComparison.OrdinalIgnoreCase))
                                            .ToArray();
                                        string materialName = materials[i];
                                        UnidentifiedItem selectedItem;
                                        if (possibilities.IsNullOrEmpty())
                                        {
                                            selectedItem = new UnidentifiedItem(
                                                identifiedItem.name,
                                                identifiedItem.description,
                                                identifiedItem.name);
                                        }
                                        else
                                        {
                                            selectedItem = this.Roller.SelectFromCollection(possibilities);
                                        }

                                        items.Add(new BaseItemType(
                                            identifiedItem.tags,
                                            identifiedItem.description,
                                            selectedItem.description,
                                            selectedItem.name,
                                            identifiedItem.name,
                                            identifiedItem.slots,
                                            identifiedItem.size,
                                            this.MaterialHandler.Get(materialName),
                                            identifiedItem.skills,
                                            actionWord,
                                            identifiedItem.value,
                                            identifiedItem.weighting,
                                            identifiedItem.spriteSheet,
                                            identifiedItem.lightLevel,
                                            identifiedItem.abilities));
                                    }

                                }
                            }
                        }
                        catch (Exception e)
                        {
                            GlobalConstants.ActionLog.AddText("Error when loading items from " + file, LogLevel.Warning);
                            GlobalConstants.ActionLog.StackTrace(e);
                        }
                    }
                }
            }

            return items;
        }
        
        public BaseItemType Get(string name)
        {
            return this.m_ItemDatabase.FirstOrDefault(type =>
                type.UnidentifiedName.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
        
        public IEnumerable<BaseItemType> FindItemsOfType(string[] tags, int tolerance = 1)
        {
            List<BaseItemType> matchingTypes = new List<BaseItemType>();
            foreach (BaseItemType itemType in this.m_ItemDatabase)
            {
                if (itemType.Tags.Intersect(tags).Count() >= tolerance)
                {
                    matchingTypes.Add(itemType);
                }
            }
            return matchingTypes.ToArray();
        }
        
        public void Dispose()
        {
            for (int i = 0; i < this.m_ItemDatabase.Count; i++)
            {
                this.m_ItemDatabase[i] = null;
            }
            
            this.m_ItemDatabase = null;
        }
    }
}