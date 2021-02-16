using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using JoyLib.Code.Entities.Abilities;
using JoyLib.Code.Graphics;
using JoyLib.Code.Helpers;
using JoyLib.Code.Rollers;

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

            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory() + GlobalConstants.DATA_FOLDER + "Items", "*.xml", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                XElement doc = XElement.Load(file);

                List<IdentifiedItem> identifiedItems = (from item in doc.Elements("IdentifiedItem")
                                                        select new IdentifiedItem()
                                                        {
                                                            name = item.Element("Name").GetAs<string>(),
                                                            description = item.Element("Description").GetAs<string>(),
                                                            value = item.Element("Value").GetAs<int>(),
                                                            size = item.Element("Size").GetAs<int>(),
                                                            spriteSheet = item.Element("Tileset").GetAs<string>(),
                                                            skill = item.Element("Skill").DefaultIfEmpty("none"),
                                                            slots = item.Elements("Slot").Select(slot => slot.DefaultIfEmpty("none")).ToArray(),
                                                            materials = item.Elements("Material").Select(material => material.GetAs<string>()).ToArray(),
                                                            tags = item.Elements("Tag").Select(tag => tag.GetAs<string>()).ToArray(),
                                                            weighting = item.Element("SpawnWeighting").GetAs<int>(),
                                                            abilities = item.Elements("Effect").Select(ability => ability.GetAs<string>() != null ? this.AbilityHandler?.GetAbility(ability.GetAs<string>()) : null).ToArray(),
                                                            lightLevel = item.Element("LightLevel").GetAs<int>()

                                                        }).ToList();

                List<UnidentifiedItem> unidentifiedItems = (from item in doc.Elements("UnidentifiedItem")
                                                            select new UnidentifiedItem()
                                                            {
                                                                name = item.Element("Name").GetAs<string>(),
                                                                description = item.Element("Description").GetAs<string>()
                                                            }).ToList();

                XElement tileSetElement = doc.Element("TileSet");
                string tileSet = tileSetElement.Element("Name").GetAs<string>();

                this.ObjectIcons.AddSpriteDataFromXML(tileSet, tileSetElement);

                string actionWord = doc.Element("ActionWord").DefaultIfEmpty("strikes");

                for (int j = 0; j < identifiedItems.Count; j++)
                {
                    UnidentifiedItem chosenDescription = new UnidentifiedItem(identifiedItems[j].name, identifiedItems[j].description);

                    if (unidentifiedItems.Count != 0)
                    {
                        int index = this.Roller.Roll(0, unidentifiedItems.Count);
                        chosenDescription = unidentifiedItems[index];
                        unidentifiedItems.RemoveAt(index);
                    }

                    for (int k = 0; k < identifiedItems[j].materials.Length; k++)
                    {
                        BaseItemType baseItemType = new BaseItemType(identifiedItems[j].tags,
                            identifiedItems[j].description, chosenDescription.description, chosenDescription.name,
                            identifiedItems[j].name, identifiedItems[j].slots, identifiedItems[j].size, this.MaterialHandler.Get(identifiedItems[j].materials[k]), identifiedItems[j].skill,
                            actionWord,
                            identifiedItems[j].value, identifiedItems[j].weighting, identifiedItems[j].spriteSheet,
                            identifiedItems[j].lightLevel, 
                            identifiedItems[j].abilities);
                        items.Add(baseItemType);
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