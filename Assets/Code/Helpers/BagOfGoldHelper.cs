using JoyLib.Code.Entities.Items;

namespace JoyLib.Code.Helpers
{
    public static class BagOfGoldHelper
    {
        private static ItemFactory s_ItemFactory = new ItemFactory();

        public static ItemInstance GetBagOfGold(int count)
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
