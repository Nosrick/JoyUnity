using JoyLib.Code.Entities.Items;
using UnityEngine;

namespace JoyLib.Code.Helpers
{
    public static class BagOfGoldHelper
    {
        private static ItemFactory s_ItemFactory = new ItemFactory();
        
        private static LiveItemHandler ItemHandler { get; set; }

        public static ItemInstance GetBagOfGold(int count)
        {
            if (ItemHandler is null)
            {
                ItemHandler = GameObject.Find("GameManager").GetComponent<LiveItemHandler>();
            }
            
            ItemInstance bag = s_ItemFactory.CreateSpecificType("leather bag", new string[] { "container", "leather" }, true);
            ItemHandler.AddItem(bag);
            for (int i = 0; i < count; i++)
            {
                ItemInstance coin = s_ItemFactory.CreateSpecificType("copper coin", new string[] { "currency" }, true);
                bag.PutItem(coin.GUID);
                ItemHandler.AddItem(coin);
            }

            return bag;
        }
    }
}
