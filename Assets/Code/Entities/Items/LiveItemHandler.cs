using JoyLib.Code.Helpers;
using JoyLib.Code.Loaders.Items;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JoyLib.Code.Entities.Items
{
    public class LiveItemHandler
    {
        private Dictionary<long, ItemInstance> m_LiveItems = new Dictionary<long, ItemInstance>();

        private static Dictionary<string, List<BaseItemType>> s_ItemDatabase = new Dictionary<string, List<BaseItemType>>();

        public static void LoadItems()
        {
            List<BaseItemType> items = ItemLoader.LoadItems();
            while (items.Count > 0)
            {
                List<BaseItemType> types = items.Where(x => x.Category == items[0].Category).ToList();
                string baseType = types[0].Category;
                s_ItemDatabase.Add(baseType, types);

                List<BaseItemType> itemsToRemove = new List<BaseItemType>();
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].Category == baseType)
                    {
                        itemsToRemove.Add(items[i]);
                    }
                }

                for (int i = 0; i < itemsToRemove.Count; i++)
                {
                    items.Remove(itemsToRemove[i]);
                }
            }
        }

        public ItemInstance CreateRandomItemOfType(string type, bool identified = false)
        {
            if(s_ItemDatabase.ContainsKey(type))
            {
                int result = RNG.Roll(0, s_ItemDatabase[type].Count - 1);
                List<BaseItemType> items = s_ItemDatabase[type];
                BaseItemType itemType = items[result];

                ItemInstance newItem = new ItemInstance(itemType, new Vector2Int(-1, -1), identified);

                m_LiveItems.Add(newItem.GUID, newItem);
                return newItem;
            }

            return null;
        }

        public ItemInstance CreateSpecificType(string baseType, string specificType, bool identified = false)
        {
            if(s_ItemDatabase.ContainsKey(baseType))
            {
                foreach(BaseItemType type in s_ItemDatabase[baseType])
                {
                    if(type.UnidentifiedName == specificType)
                    {
                        ItemInstance newItem = new ItemInstance(type, new Vector2Int(-1, -1), identified);

                        m_LiveItems.Add(newItem.GUID, newItem);
                        return newItem;
                    }
                }
            }
            return null;
        }

        public ItemInstance GetInstance(long GUID)
        {
            if(m_LiveItems.ContainsKey(GUID))
            {
                return m_LiveItems[GUID];
            }

            return null;
        }
    }
}
