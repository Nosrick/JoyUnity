using UnityEngine;
using JoyLib.Code.Rollers;
using JoyLib.Code.Graphics;
using System.Collections.Generic;

namespace JoyLib.Code.Entities.Items
{
    public class ItemFactory
    {
        protected static GameObject s_GameManager;

        protected static LiveItemHandler s_ItemHandler;

        protected static ObjectIconHandler s_ObjectIcons;

        public ItemFactory()
        {
            if(s_GameManager is null)
            {
                s_GameManager = GameObject.Find("GameManager");
                s_ObjectIcons = GameObject.Find("GameManager").GetComponent<ObjectIconHandler>();
                s_ItemHandler = GameObject.Find("GameManager").GetComponent<LiveItemHandler>();
            }
        }

        public ItemInstance CreateRandomItemOfType(string[] tags, bool identified = false)
        {
            BaseItemType[] matchingTypes = s_ItemHandler.FindItemsOfType(tags);
            if (matchingTypes.Length > 0)
            {
                int result = RNG.instance.Roll(0, matchingTypes.Length - 1);
                BaseItemType itemType = matchingTypes[result];

                ItemInstance itemInstance = new ItemInstance(itemType, 
                                                            new Vector2Int(-1, -1), 
                                                            identified, 
                                                            s_ObjectIcons.GetSprites(
                                                                itemType.SpriteSheet,
                                                                itemType.UnidentifiedName));
                return itemInstance;
            }

            return null;
        }

        public ItemInstance CreateSpecificType(string name, string[] tags, bool identified = false)
        {
            BaseItemType[] matchingTypes = s_ItemHandler.FindItemsOfType(tags);
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
                int result = RNG.instance.Roll(0, secondRound.Count - 1);
                BaseItemType type = secondRound[result];
                ItemInstance itemInstance = new ItemInstance(type, 
                                                            new Vector2Int(-1, -1), 
                                                            identified,
                                                            s_ObjectIcons.GetSprites(
                                                                type.SpriteSheet,
                                                                type.UnidentifiedName));
                return itemInstance;
            }

            throw new ItemTypeNotFoundException(name, "Could not find an item type by the name of " + name);
        }

        public ItemInstance CreateCompletelyRandomItem(bool identified = false, bool withAbility = false)
        {
            List<BaseItemType> itemDatabase = s_ItemHandler.ItemDatabase;

            int result = RNG.instance.Roll(0, itemDatabase.Count - 1);
            BaseItemType itemType = itemDatabase[result];
            ItemInstance itemInstance = new ItemInstance(itemType, 
                                                        new Vector2Int(-1, -1), 
                                                        identified,
                                                        s_ObjectIcons.GetSprites(
                                                            itemType.SpriteSheet,
                                                            itemType.UnidentifiedName));
            return itemInstance;
        }
    }
}