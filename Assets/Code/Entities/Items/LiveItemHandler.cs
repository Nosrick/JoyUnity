using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using DevionGames.InventorySystem;
using JoyLib.Code.Entities.Abilities;
using JoyLib.Code.Graphics;
using JoyLib.Code.Helpers;
using JoyLib.Code.Rollers;
using JoyLib.Code.World;
using UnityEngine;

namespace JoyLib.Code.Entities.Items
{
    public class LiveItemHandler : ILiveItemHandler
    {
        protected Dictionary<long, IItemInstance> m_LiveItems;

        protected List<BaseItemType> m_ItemDatabase;

        protected IObjectIconHandler m_ObjectIcons;

        protected IMaterialHandler m_MaterialHandler;
        
        protected IAbilityHandler AbilityHandler { get; set; }
        
        protected RNG Roller { get; set; }

        protected static GameObject s_ItemPrefab; 

        public LiveItemHandler(
            IObjectIconHandler objectIconHandler,
            IMaterialHandler materialHandler,
            IAbilityHandler abilityHandler,
            RNG roller)
        {
            AbilityHandler = abilityHandler;
            Roller = roller;
            m_ObjectIcons = objectIconHandler;
            m_MaterialHandler = materialHandler;
            
            if (m_ItemDatabase is null)
            {
                Initialise();
            }
        }

        protected void Initialise()
        {
            m_LiveItems = new Dictionary<long, IItemInstance>();

            s_ItemPrefab = Resources.Load<GameObject>("Prefabs/ItemInstance");

            m_ItemDatabase = LoadItems();
        }

        protected List<BaseItemType> LoadItems()
        {
            InventoryManager.Database.items.Clear();
            InventoryManager.Database.equipments.Clear();
            
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
                                                            abilities = item.Elements("Effect").Select(ability => ability.GetAs<string>() != null ? AbilityHandler?.GetAbility(ability.GetAs<string>()) : null).ToArray(),
                                                            lightLevel = item.Element("LightLevel").GetAs<int>()

                                                        }).ToList();

                List<UnidentifiedItem> unidentifiedItems = (from item in doc.Elements("UnidentifiedItem")
                                                            select new UnidentifiedItem()
                                                            {
                                                                name = item.Element("Name").GetAs<string>(),
                                                                description = item.Element("Description").GetAs<string>()
                                                            }).ToList();

                string tileSet = doc.Element("TileSet").Element("Name").GetAs<string>();

                string fileName = doc.Element("TileSet").Element("Filename").DefaultIfEmpty("");

                if (fileName.Length > 0)
                {
                    List<IconData> iconData = (from item in doc.Element("TileSet").Elements("Icon")
                                               select new IconData()
                                               {
                                                   name = item.Element("Name").GetAs<string>(),
                                                   data = item.Element("Data").DefaultIfEmpty(""),
                                                   frames = item.Element("Frames").DefaultIfEmpty(1),
                                                   filename = fileName,
                                                   position = item.Element("Position").GetAs<int>()
                                               }).ToList();

                    m_ObjectIcons.AddIcons(tileSet, iconData.ToArray());
                }


                string actionWord = doc.Element("ActionWord").DefaultIfEmpty("strikes");

                string effect = doc.Element("Effect").DefaultIfEmpty("");

                for (int j = 0; j < identifiedItems.Count; j++)
                {
                    UnidentifiedItem chosenDescription = new UnidentifiedItem(identifiedItems[j].name, identifiedItems[j].description);

                    if (unidentifiedItems.Count != 0)
                    {
                        int index = Roller.Roll(0, unidentifiedItems.Count);
                        chosenDescription = unidentifiedItems[index];
                        unidentifiedItems.RemoveAt(index);
                    }

                    for (int k = 0; k < identifiedItems[j].materials.Length; k++)
                    {
                        BaseItemType baseItemType = new BaseItemType(identifiedItems[j].tags,
                            identifiedItems[j].description, chosenDescription.description, chosenDescription.name,
                            identifiedItems[j].name, identifiedItems[j].slots, identifiedItems[j].size,
                            m_MaterialHandler.GetMaterial(identifiedItems[j].materials[k]), identifiedItems[j].skill,
                            actionWord,
                            identifiedItems[j].value, identifiedItems[j].weighting, identifiedItems[j].spriteSheet,
                            identifiedItems[j].lightLevel, 
                            identifiedItems[j].abilities);
                        items.Add(baseItemType);
                        
                        if(baseItemType.Slots.Any(slot => slot.Equals("none", StringComparison.OrdinalIgnoreCase)))
                        {
                            ItemInstance itemSO = ScriptableObject.CreateInstance<ItemInstance>();
                            itemSO.Name = baseItemType.IdentifiedName;
                            itemSO.Icon = m_ObjectIcons.GetSprite(baseItemType.SpriteSheet, baseItemType.IdentifiedName);
                            itemSO.Prefab = s_ItemPrefab;

                            InventoryManager.Database.items.Add(itemSO);
                        }
                        else
                        {
                            ItemInstance itemSO = ScriptableObject.CreateInstance<ItemInstance>();
                            itemSO.Name = baseItemType.IdentifiedName;
                            itemSO.Icon = m_ObjectIcons.GetSprite(baseItemType.SpriteSheet, baseItemType.IdentifiedName);
                            itemSO.Prefab = s_ItemPrefab;
                            List<EquipmentRegion> regions = new List<EquipmentRegion>();
                            foreach (string slot in baseItemType.Slots)
                            {
                                EquipmentRegion region = ScriptableObject.CreateInstance<EquipmentRegion>();
                                region.Name = slot;
                                regions.Add(region);
                            }

                            itemSO.Region = regions;
                            InventoryManager.Database.items.Add(itemSO);
                        }

                        foreach (string slot in identifiedItems.SelectMany(item => item.slots))
                        {
                            if (InventoryManager.Database.equipments.Any(region => region.Name.Equals(slot)) == false)
                            {
                                EquipmentRegion equipmentRegion = ScriptableObject.CreateInstance<EquipmentRegion>();
                                equipmentRegion.Name = slot;
                                InventoryManager.Database.equipments.Add(equipmentRegion);
                            }
                        }
                    }
                }
            }

            return items;
        }

        public BaseItemType[] FindItemsOfType(string[] tags, int tolerance = 1)
        {
            List<BaseItemType> matchingTypes = new List<BaseItemType>();
            foreach (BaseItemType itemType in ItemDatabase)
            {
                if (itemType.Tags.Intersect(tags).Count() >= tolerance)
                {
                    matchingTypes.Add(itemType);
                }
            }
            return matchingTypes.ToArray();
        }

        public bool AddItem(IItemInstance item, bool addToWorld = false)
        {
            if (LiveItems.ContainsKey(item.GUID))
            {
                return false;
            }

            LiveItems.Add(item.GUID, item);
            if (addToWorld 
                && item.MyWorld is null == false 
                && item.MyWorld.Objects.Any(o => o.GUID == item.GUID) == false)
            {
                item.MyWorld.AddObject(item);
            }
            return true;
        }

        public bool RemoveItemFromWorld(long GUID)
        {
            if (!LiveItems.ContainsKey(GUID))
            {
                return false;
            }
            
            IItemInstance item = GetItem(GUID);
            item.MyWorld?.RemoveObject(item.WorldPosition, item);
            //LiveItems.Remove(GUID);
            return true;

        }

        public bool RemoveItemFromWorld(IItemInstance item)
        {
            if (!LiveItems.ContainsKey(item.GUID))
            {
                return false;
            }

            return item.MyWorld.RemoveObject(item.WorldPosition, item);
        }

        public bool AddItemToWorld(WorldInstance world, long GUID)
        {
            if (!LiveItems.ContainsKey(GUID))
            {
                return false;
            }
            
            IItemInstance item = GetItem(GUID);
            item.MyWorld = world;
            world.AddObject(item);
            return true;
        }

        public IItemInstance GetItem(long GUID)
        {
            if (LiveItems.ContainsKey(GUID))
            {
                return LiveItems[GUID];
            }
            throw new InvalidOperationException("No item found with GUID " + GUID);
        }

        public List<BaseItemType> ItemDatabase
        {
            get
            {
                if (m_ItemDatabase is null)
                {
                    Initialise();
                }

                return new List<BaseItemType>(m_ItemDatabase);
            }
        }

        protected Dictionary<long, IItemInstance> LiveItems
        {
            get
            {
                if (m_LiveItems is null)
                {
                    Initialise();
                }

                return m_LiveItems;
            }
        }
    }
}
