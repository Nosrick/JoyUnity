using System;
using JoyLib.Code.Entities.Abilities;
using JoyLib.Code.Graphics;
using JoyLib.Code.Helpers;
using JoyLib.Code.Rollers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using DevionGames.InventorySystem;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace JoyLib.Code.Entities.Items
{
    public class LiveItemHandler : MonoBehaviour
    {
        protected Dictionary<long, ItemInstance> m_LiveItems;

        protected List<BaseItemType> m_ItemDatabase;

        protected GameObject m_GameManager;

        protected ObjectIconHandler m_ObjectIcons;

        protected MaterialHandler m_MaterialHandler;

        protected static GameObject s_ItemPrefab; 

        public void Awake()
        {
            if (m_ItemDatabase is null)
            {
                Initialise();
            }
        }

        protected void Initialise()
        {
            m_LiveItems = new Dictionary<long, ItemInstance>();

            m_GameManager = GameObject.Find("GameManager");
            m_ObjectIcons = m_GameManager.GetComponent<ObjectIconHandler>();
            m_MaterialHandler = m_GameManager.GetComponent<MaterialHandler>();

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
                                                            abilities = item.Elements("Ability").Select(ability => AbilityHandler.instance.GetAbility(ability.GetAs<string>())).ToArray(),
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
                                                   filename = fileName
                                               }).ToList();

                    m_ObjectIcons.AddIcons(tileSet, iconData.ToArray());
                }


                string actionWord = doc.Element("ActionWord").DefaultIfEmpty("strikes");


                for (int j = 0; j < identifiedItems.Count; j++)
                {
                    UnidentifiedItem chosenDescription = new UnidentifiedItem(identifiedItems[j].name, identifiedItems[j].description);

                    if (unidentifiedItems.Count != 0)
                    {
                        int index = RNG.instance.Roll(0, unidentifiedItems.Count - 1);
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
                            identifiedItems[j].lightLevel);
                        items.Add(baseItemType);
                        
                        if(baseItemType.Slots.Any(slot => slot.Equals("none", StringComparison.OrdinalIgnoreCase)))
                        {
                            Item itemSO = ScriptableObject.CreateInstance<Item>();
                            itemSO.Name = baseItemType.IdentifiedName;
                            itemSO.Icon = m_ObjectIcons.GetSprite(baseItemType.SpriteSheet, baseItemType.IdentifiedName);
                            itemSO.Prefab = s_ItemPrefab;

                            InventoryManager.Database.items.Add(itemSO);
                        }
                        else
                        {
                            EquipmentItem itemSO = ScriptableObject.CreateInstance<EquipmentItem>();
                            itemSO.Name = baseItemType.IdentifiedName;
                            itemSO.Icon =
                                m_ObjectIcons.GetSprite(baseItemType.SpriteSheet, baseItemType.IdentifiedName);
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
                    }
                }
            }

            return items;
        }

        public BaseItemType[] FindItemsOfType(string[] tags)
        {
            List<BaseItemType> matchingTypes = new List<BaseItemType>();
            foreach (BaseItemType itemType in ItemDatabase)
            {
                int matches = 0;
                for (int i = 0; i < tags.Length; i++)
                {
                    if (itemType.Tags.Contains(tags[i]))
                    {
                        matches++;
                    }
                }
                if (matches == tags.Length || (tags.Length < itemType.Tags.Length && matches > 0))
                {
                    matchingTypes.Add(itemType);
                }
            }
            return matchingTypes.ToArray();
        }



        public ItemInstance GetInstance(long GUID)
        {
            if (LiveItems.ContainsKey(GUID))
            {
                return LiveItems[GUID];
            }

            return null;
        }

        public bool AddItem(ItemInstance item)
        {
            if (LiveItems.ContainsKey(item.GUID))
            {
                return false;
            }

            LiveItems.Add(item.GUID, item);
            return true;
        }

        public bool RemoveItem(long GUID)
        {
            if (LiveItems.ContainsKey(GUID))
            {
                LiveItems.Remove(GUID);
                return true;
            }

            return false;
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

        protected Dictionary<long, ItemInstance> LiveItems
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
