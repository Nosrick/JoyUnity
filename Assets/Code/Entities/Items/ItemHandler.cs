using JoyLib.Code.Loaders.Items;
using System.Collections.Generic;
using System.Linq;

namespace JoyLib.Code.Entities.Items
{
    public static class ItemHandler
    {
        private static Dictionary<string, List<BaseItemType>> m_Items = new Dictionary<string, List<BaseItemType>>();

        public static void LoadItems()
        {
            List<BaseItemType> items = ItemLoader.LoadItems();
            while(items.Count > 0)
            {
                List<BaseItemType> types = items.Where(x => x.Category == items[0].Category).ToList();
                string baseType = types[0].Category;
                m_Items.Add(baseType, types);

                List<BaseItemType> itemsToRemove = new List<BaseItemType>();
                for(int i = 0; i < items.Count; i++)
                {
                    if (items[i].Category == baseType)
                    {
                        itemsToRemove.Add(items[i]);
                    }
                }

                for(int i = 0; i < itemsToRemove.Count; i++)
                {
                    items.Remove(itemsToRemove[i]);
                }
            }
        }

        public static List<BaseItemType> GetFlatItemList()
        {
            List<BaseItemType> items = new List<BaseItemType>();
            foreach(List<BaseItemType> list in m_Items.Values)
            {
                items.AddRange(list);
            }
            return items;
        }

        public static BaseItemType GetSpecificItem(string itemType)
        {
            List<BaseItemType> items = GetFlatItemList();
            try
            {
                if (items.Any(x => x.UnidentifiedName == itemType))
                {
                    return items.First(x => x.UnidentifiedName == itemType);
                }
            }
            catch
            {
            }
            return null;
        }

        public static List<BaseItemType> GetItemsOfType(string itemType)
        {
            if (m_Items.ContainsKey(itemType))
                return m_Items[itemType];

            return new List<BaseItemType>();
        }
    }
}
