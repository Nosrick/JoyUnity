using JoyLib.Code.Entities.Abilities;
using JoyLib.Code.Graphics;
using JoyLib.Code.Helpers;
using JoyLib.Code.Rollers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using UnityEngine;

namespace JoyLib.Code.Entities.Items
{
    public class LiveItemHandler
    {
        private Dictionary<long, ItemInstance> m_LiveItems;

        private static List<BaseItemType> s_ItemDatabase;

        public LiveItemHandler()
        {
            m_LiveItems = new Dictionary<long, ItemInstance>();
        }

        public static bool Initialise()
        {
            if (s_ItemDatabase != null)
            {
                return true;
            }

            s_ItemDatabase = LoadItems();
            return true;
        }

        public static List<BaseItemType> LoadItems()
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
                                                            abilities = item.Elements("Ability").Select(ability => AbilityHandler.GetAbility(ability.GetAs<string>().ToLower())).ToArray(),
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

                    ObjectIconHandler.instance.AddIcons(fileName, tileSet, iconData.ToArray());
                }


                string actionWord = doc.Element("ActionWord").DefaultIfEmpty("strikes").ToLower();


                for (int j = 0; j < identifiedItems.Count; j++)
                {
                    UnidentifiedItem chosenDescription = new UnidentifiedItem(identifiedItems[j].name, identifiedItems[j].description);

                    if (unidentifiedItems.Count != 0)
                    {
                        int index = RNG.Roll(0, unidentifiedItems.Count - 1);
                        chosenDescription = unidentifiedItems[index];
                        unidentifiedItems.RemoveAt(index);
                    }

                    for (int k = 0; k < identifiedItems[j].materials.Length; k++)
                    {

                        items.Add(new BaseItemType(identifiedItems[j].tags, identifiedItems[j].description, chosenDescription.description, chosenDescription.name,
                            identifiedItems[j].name, identifiedItems[j].slots, identifiedItems[j].size,
                            MaterialHandler.GetMaterial(identifiedItems[j].materials[k]), identifiedItems[j].skill, actionWord,
                            identifiedItems[j].value, identifiedItems[j].weighting, identifiedItems[j].spriteSheet, identifiedItems[j].lightLevel));
                    }
                }
            }

            return items;
        }

        protected BaseItemType[] FindItemsOfType(string[] tags)
        {
            List<BaseItemType> matchingTypes = new List<BaseItemType>();
            foreach (BaseItemType itemType in s_ItemDatabase)
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

        public ItemInstance CreateRandomItemOfType(string[] tags, bool identified = false)
        {
            BaseItemType[] matchingTypes = FindItemsOfType(tags);
            if (matchingTypes.Length > 0)
            {
                int result = RNG.Roll(0, matchingTypes.Length - 1);
                BaseItemType itemType = matchingTypes[result];

                ItemInstance itemInstance = new ItemInstance(itemType, new Vector2Int(-1, -1), identified);
                m_LiveItems.Add(itemInstance.GUID, itemInstance);

                return itemInstance;
            }

            return null;
        }

        public ItemInstance CreateSpecificType(string name, string[] tags, bool identified = false)
        {
            string lowerName = name.ToLowerInvariant();
            BaseItemType[] matchingTypes = FindItemsOfType(tags);
            List<BaseItemType> secondRound = new List<BaseItemType>();
            foreach (BaseItemType itemType in matchingTypes)
            {
                if (identified == false)
                {
                    if (itemType.UnidentifiedName.ToLowerInvariant() == lowerName)
                    {
                        secondRound.Add(itemType);
                    }
                }
                else
                {
                    if (itemType.IdentifiedName == lowerName)
                    {
                        secondRound.Add(itemType);
                    }
                }
            }
            if (secondRound.Count > 0)
            {
                int result = RNG.Roll(0, secondRound.Count - 1);
                BaseItemType type = secondRound[result];
                ItemInstance itemInstance = new ItemInstance(type, new Vector2Int(-1, -1), identified);
                m_LiveItems.Add(itemInstance.GUID, itemInstance);
                return itemInstance;
            }

            return null;
        }

        public ItemInstance GetInstance(long GUID)
        {
            if (m_LiveItems.ContainsKey(GUID))
            {
                return m_LiveItems[GUID];
            }

            return null;
        }

        public ItemInstance CreateCompletelyRandomItem(bool identified = false, bool withAbility = false)
        {
            int result = RNG.Roll(0, s_ItemDatabase.Count - 1);
            BaseItemType itemType = s_ItemDatabase[result];
            ItemInstance itemInstance = new ItemInstance(itemType, new Vector2Int(-1, -1), identified);
            m_LiveItems.Add(itemInstance.GUID, itemInstance);
            return itemInstance;
        }

        public class ItemCreationException : Exception
        {
            public BaseItemType ItemType;

            public ItemCreationException(BaseItemType itemType, string message) :
                base(message)
            {
                ItemType = itemType;
            }
        }
    }
}
