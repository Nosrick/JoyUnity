using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using JoyLib.Code.Collections;
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
            this.AbilityHandler = abilityHandler;
            this.Roller = roller;
            this.m_ObjectIcons = objectIconHandler;
            this.m_MaterialHandler = materialHandler;
            
            if (this.m_ItemDatabase is null)
            {
                this.Initialise();
            }
        }

        protected void Initialise()
        {
            this.m_LiveItems = new Dictionary<long, IItemInstance>();

            this.QuestRewards = new NonUniqueDictionary<long, long>();

            s_ItemPrefab = Resources.Load<GameObject>("Prefabs/ItemInstance");

            this.m_ItemDatabase = this.LoadItems();
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

                this.m_ObjectIcons.AddSpriteDataFromXML(tileSet, tileSetElement);

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
                            identifiedItems[j].name, identifiedItems[j].slots, identifiedItems[j].size, this.m_MaterialHandler.GetMaterial(identifiedItems[j].materials[k]), identifiedItems[j].skill,
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

        public BaseItemType[] FindItemsOfType(string[] tags, int tolerance = 1)
        {
            List<BaseItemType> matchingTypes = new List<BaseItemType>();
            foreach (BaseItemType itemType in this.ItemDatabase)
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
            if (this.LiveItems.ContainsKey(item.GUID))
            {
                return false;
            }

            this.LiveItems.Add(item.GUID, item);
            if (addToWorld 
                && item.MyWorld is null == false 
                && item.MyWorld.Objects.Any(o => o.GUID == item.GUID) == false)
            {
                item.MyWorld.AddObject(item);
            }
            return true;
        }

        public bool AddItems(IEnumerable<IItemInstance> items, bool addToWorld = false)
        {
            return items.Aggregate(true, (current, item) => current & this.AddItem(item));
        }

        public bool RemoveItemFromWorld(long GUID)
        {
            if (!this.LiveItems.ContainsKey(GUID))
            {
                return false;
            }
            
            IItemInstance item = this.GetItem(GUID);
            item.MyWorld?.RemoveObject(item.WorldPosition, item);
            //LiveItems.Remove(GUID);
            return true;

        }

        public bool RemoveItemFromWorld(IItemInstance item)
        {
            if (!this.LiveItems.ContainsKey(item.GUID))
            {
                return false;
            }

            return item.MyWorld.RemoveObject(item.WorldPosition, item);
        }

        public bool AddItemToWorld(WorldInstance world, long GUID)
        {
            if (!this.LiveItems.ContainsKey(GUID))
            {
                return false;
            }
            
            IItemInstance item = this.GetItem(GUID);
            item.MyWorld = world;
            world.AddObject(item);
            return true;
        }

        public IItemInstance GetItem(long GUID)
        {
            if (this.LiveItems.ContainsKey(GUID))
            {
                return this.LiveItems[GUID];
            }
            throw new InvalidOperationException("No item found with GUID " + GUID);
        }

        public IEnumerable<IItemInstance> GetQuestRewards(long questID)
        {
            return this.QuestRewards.ContainsKey(questID) 
                ? this.GetItems(this.QuestRewards.FetchValuesForKey(questID)) 
                : new IItemInstance[0];
        }

        public void CleanUpRewards(IEnumerable<long> GUIDs)
        {
            NonUniqueDictionary<long, long> cleanUp = new NonUniqueDictionary<long, long>();
            foreach (var tuple in this.QuestRewards)
            {
                if (GUIDs.Contains(tuple.Item2))
                {
                    cleanUp.Add(tuple.Item1, tuple.Item2);
                }
                else
                {
                    IItemInstance item = this.GetItem(tuple.Item2);
                    if (item.MonoBehaviourHandler is null)
                    {
                        GlobalConstants.ActionLog.AddText("No MBH found on " + item);
                    }
                    else
                    {
                        GlobalConstants.GameManager.ItemPool.Retire(
                            item.MonoBehaviourHandler.gameObject);
                    }
                }
            }

            this.QuestRewards = cleanUp;
        }

        public void AddQuestReward(long questID, long reward)
        {
            this.QuestRewards.Add(questID, reward);
        }

        public void AddQuestRewards(long questID, IEnumerable<long> rewards)
        {
            foreach (long reward in rewards)
            {
                this.QuestRewards.Add(questID, reward);
            }
        }

        public void AddQuestRewards(long questID, IEnumerable<IItemInstance> rewards)
        {
            this.AddQuestRewards(questID, rewards.Select(instance => instance.GUID));
        }

        public IEnumerable<IItemInstance> GetItems(IEnumerable<long> guids)
        {
            List<IItemInstance> items = new List<IItemInstance>();

            foreach (long guid in guids)
            {
                if (this.LiveItems.TryGetValue(guid, out IItemInstance item))
                {
                    items.Add(item);
                }
            }

            return items;
        }

        public List<BaseItemType> ItemDatabase
        {
            get
            {
                if (this.m_ItemDatabase is null)
                {
                    this.Initialise();
                }

                return new List<BaseItemType>(this.m_ItemDatabase);
            }
        }

        protected Dictionary<long, IItemInstance> LiveItems
        {
            get
            {
                if (this.m_LiveItems is null)
                {
                    this.Initialise();
                }

                return this.m_LiveItems;
            }
        }
        
        public NonUniqueDictionary<long, long> QuestRewards
        {
            get;
            protected set;
        }

        public IEnumerable<IItemInstance> AllItems => this.LiveItems.Values.ToList();
    }
}
