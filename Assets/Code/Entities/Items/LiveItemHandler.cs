using JoyLib.Code.Entities.Abilities;
using JoyLib.Code.Graphics;
using JoyLib.Code.Helpers;
using JoyLib.Code.Rollers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

namespace JoyLib.Code.Entities.Items
{
    public class LiveItemHandler : MonoBehaviour
    {
        protected Dictionary<long, ItemInstance> m_LiveItems;

        protected List<BaseItemType> m_ItemDatabase;

        protected GameObject m_GameManager;

        protected ObjectIconHandler m_ObjectIcons;

        protected MaterialHandler m_MaterialHandler;

        public void Awake()
        {
            Initialise();
        }

        protected void Initialise()
        {
            m_LiveItems = new Dictionary<long, ItemInstance>();

            m_GameManager = GameObject.Find("GameManager");
            m_ObjectIcons = m_GameManager.GetComponent<ObjectIconHandler>();
            m_MaterialHandler = m_GameManager.GetComponent<MaterialHandler>();

            m_ItemDatabase = LoadItems();
        }

        protected List<BaseItemType> LoadItems()
        {
            List<BaseItemType> items = new List<BaseItemType>();

            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory() + GlobalConstants.DATA_FOLDER + "Items", "*.xml", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                XElement doc = XElement.Load(file);

                List<IdentifiedItem> identifiedItems = (from item in doc.Elements("IdentifiedItem")
                                                        select new IdentifiedItem()
                                                        {
                                                            name = item.Element("Name").GetAs<string>().ToLower(),
                                                            description = item.Element("Description").GetAs<string>().ToLower(),
                                                            value = item.Element("Value").GetAs<int>(),
                                                            size = item.Element("Size").GetAs<int>(),
                                                            spriteSheet = item.Element("Tileset").GetAs<string>().ToLower(),
                                                            skill = item.Element("Skill").DefaultIfEmpty("none").ToLower(),
                                                            slots = item.Elements("Slot").Select(slot => slot.DefaultIfEmpty("none").ToLower()).ToArray(),
                                                            materials = item.Elements("Material").Select(material => material.GetAs<string>().ToLower()).ToArray(),
                                                            tags = item.Elements("Tag").Select(tag => tag.GetAs<string>().ToLower()).ToArray(),
                                                            weighting = item.Element("SpawnWeighting").GetAs<int>(),
                                                            abilities = item.Elements("Ability").Select(ability => AbilityHandler.instance.GetAbility(ability.GetAs<string>().ToLower())).ToArray(),
                                                            lightLevel = item.Element("LightLevel").GetAs<int>()

                                                        }).ToList();

                List<UnidentifiedItem> unidentifiedItems = (from item in doc.Elements("UnidentifiedItem")
                                                            select new UnidentifiedItem()
                                                            {
                                                                name = item.Element("Name").GetAs<string>().ToLower(),
                                                                description = item.Element("Description").GetAs<string>().ToLower()
                                                            }).ToList();

                string tileSet = doc.Element("TileSet").Element("Name").GetAs<string>().ToLower();

                string fileName = doc.Element("TileSet").Element("Filename").DefaultIfEmpty("");

                if (fileName.Length > 0)
                {
                    List<IconData> iconData = (from item in doc.Element("TileSet").Elements("Icon")
                                               select new IconData()
                                               {
                                                   name = item.Element("Name").GetAs<string>().ToLower(),
                                                   data = item.Element("Data").DefaultIfEmpty("").ToLower(),
                                                   frames = item.Element("Frames").DefaultIfEmpty(1),
                                                   position = new Vector2Int(item.Element("X").GetAs<int>(), item.Element("Y").GetAs<int>())
                                               }).ToList();

                    m_ObjectIcons.AddIcons(fileName, tileSet, iconData.ToArray());
                }


                string actionWord = doc.Element("ActionWord").DefaultIfEmpty("strikes").ToLower();


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

                        items.Add(new BaseItemType(identifiedItems[j].tags, identifiedItems[j].description, chosenDescription.description, chosenDescription.name,
                            identifiedItems[j].name, identifiedItems[j].slots, identifiedItems[j].size,
                            m_MaterialHandler.GetMaterial(identifiedItems[j].materials[k]), identifiedItems[j].skill, actionWord,
                            identifiedItems[j].value, identifiedItems[j].weighting, identifiedItems[j].spriteSheet, identifiedItems[j].lightLevel));
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
            if(LiveItems.ContainsKey(item.GUID))
            {
                return false;
            }

            LiveItems.Add(item.GUID, item);
            return true;
        }

        public bool RemoveItem(long GUID)
        {
            if(LiveItems.ContainsKey(GUID))
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
                if(m_ItemDatabase is null)
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
                if(m_LiveItems is null)
                {
                    Initialise();
                }

                return m_LiveItems;
            }
        }
    }
}
