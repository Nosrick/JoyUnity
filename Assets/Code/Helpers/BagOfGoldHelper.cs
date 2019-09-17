using JoyLib.Code.Entities.Items;
using JoyLib.Code.States;

namespace JoyLib.Code.Helpers
{
    public static class BagOfGoldHelper
    {
        public static ItemInstance GetBagOfGold(int count)
        {
            ItemInstance bag = WorldState.ItemHandler.CreateSpecificType("leather bag", new string[] { "container", "leather" }, true);
            for (int i = 0; i < count; i++)
            {
                ItemInstance coin = WorldState.ItemHandler.CreateSpecificType("copper coin", new string[] { "currency" }, true);
                bag.PutItem(coin.GUID);
            }

            return bag;
        }
    }
}
