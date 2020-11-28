using System;
using UnityEngine;
using JoyLib.Code.Rollers;
using JoyLib.Code.Graphics;
using System.Collections.Generic;
using System.Linq;
using DevionGames.InventorySystem;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Unity;

namespace JoyLib.Code.Entities.Items
{
    public class ItemFactory
    {
        protected GameObject GameManager { get; set; }

        protected LiveItemHandler ItemHandler { get; set; }

        protected ObjectIconHandler ObjectIcons { get; set; }

        public ItemFactory()
        {
            if(GameManager is null)
            {
                GameManager = GlobalConstants.GameManager;
                ObjectIcons = GameManager.GetComponent<ObjectIconHandler>();
                ItemHandler = GameManager.GetComponent<LiveItemHandler>();
            }
        }

        public ItemInstance CreateRandomItemOfType(string[] tags, bool identified = false)
        {
            BaseItemType[] matchingTypes = ItemHandler.FindItemsOfType(tags);
            if (matchingTypes.Length > 0)
            {
                int result = RNG.instance.Roll(0, matchingTypes.Length);
                BaseItemType itemType = matchingTypes[result];

                ItemInstance itemInstance = ScriptableObject.CreateInstance<ItemInstance>(); 
                    
                    itemInstance.Initialise(
                        itemType, 
                        EntityDerivedValue.GetDefaultForItem(
                            itemType.Material.Bonus,
                            itemType.Weight),
                        new Vector2Int(-1, -1), 
                        identified, 
                        ObjectIcons.GetSprites(
                            itemType.SpriteSheet,
                            itemType.UnidentifiedName));
                return itemInstance;
            }

            return null;
        }

        public ItemInstance CreateSpecificType(string name, string[] tags, bool identified = false)
        {
            BaseItemType[] matchingTypes = ItemHandler.FindItemsOfType(tags);
            List<BaseItemType> secondRound = new List<BaseItemType>();
            foreach (BaseItemType itemType in matchingTypes)
            {
                if (identified == false)
                {
                    if (itemType.UnidentifiedName.Equals(name, System.StringComparison.OrdinalIgnoreCase))
                    {
                        secondRound.Add(itemType);
                    }
                }
                else
                {
                    if (itemType.IdentifiedName.Equals(name, System.StringComparison.OrdinalIgnoreCase))
                    {
                        secondRound.Add(itemType);
                    }
                }
            }
            if (secondRound.Count > 0)
            {
                int result = RNG.instance.Roll(0, secondRound.Count);
                BaseItemType type = secondRound[result];
                ItemInstance itemInstance = ScriptableObject.CreateInstance<ItemInstance>(); 
                    
                    itemInstance.Initialise(
                        type, 
                        EntityDerivedValue.GetDefaultForItem(
                            type.Material.Bonus,
                            type.Weight),
                        new Vector2Int(-1, -1), 
                        identified, 
                        ObjectIcons.GetSprites(
                            type.SpriteSheet,
                            type.UnidentifiedName));
                    
                return itemInstance;
            }

            throw new ItemTypeNotFoundException(name, "Could not find an item type by the name of " + name);
        }

        public ItemInstance CreateCompletelyRandomItem(bool identified = false, bool withAbility = false)
        {
            List<BaseItemType> itemDatabase = ItemHandler.ItemDatabase;

            int result = RNG.instance.Roll(0, itemDatabase.Count);
            BaseItemType itemType = itemDatabase[result];
            ItemInstance itemInstance = ScriptableObject.CreateInstance<ItemInstance>(); 
                itemInstance.Initialise(
                    itemType, 
                    EntityDerivedValue.GetDefaultForItem(
                        itemType.Material.Bonus,
                        itemType.Weight),
                    new Vector2Int(-1, -1), 
                    identified, 
                    ObjectIcons.GetSprites(
                        itemType.SpriteSheet,
                        itemType.UnidentifiedName));
                
            return itemInstance;
        }
    }
}