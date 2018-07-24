using JoyLib.Code.Helpers;
using System.Collections.Generic;
using UnityEngine;

namespace JoyLib.Code.Entities.Items
{
    public static class ItemProvider
    {
        public static ItemInstance RandomItem(bool identified = false)
        {
            List<BaseItemType> itemChances = new List<BaseItemType>();
            List<BaseItemType> items = ItemHandler.GetFlatItemList();

            for(int i = 0; i < items.Count; i++)
            {
                for(int j = 0; j < items[i].SpawnWeighting; j++)
                {
                    itemChances.Add(items[i]);
                }
            }

            return new ItemInstance(itemChances[RNG.Roll(0, itemChances.Count - 1)], new Vector2Int(-1, -1), identified);
        }

        public static ItemInstance RandomItemOfType(string type, bool identified = false)
        {
            List<BaseItemType> itemChances = new List<BaseItemType>();
            List<BaseItemType> items = ItemHandler.GetItemsOfType(type);

            for (int i = 0; i < items.Count; i++)
            {
                for (int j = 0; j < items[i].SpawnWeighting; j++)
                {
                    itemChances.Add(items[i]);
                }
            }

            return new ItemInstance(itemChances[RNG.Roll(0, itemChances.Count - 1)], new Vector2Int(-1, -1), identified);
        }
    }
}
