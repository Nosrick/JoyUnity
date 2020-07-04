using JoyLib.Code.Entities.Items;
using JoyLib.Code.States;
using UnityEngine;

namespace JoyLib.Code.Helpers
{
    public class BagOfGoldHelper
    {
        private static ItemFactory s_ItemFactory = new ItemFactory();

        public ItemInstance GetBagOfGold(int count)
        {
            ItemInstance bag = s_ItemFactory.CreateSpecificType("leather bag", new string[] { "container", "leather" }, true);
            for (int i = 0; i < count; i++)
            {
                ItemInstance coin = s_ItemFactory.CreateSpecificType("copper coin", new string[] { "currency" }, true);
                bag.PutItem(coin.GUID);
            }

            return bag;
        }
    }
}
