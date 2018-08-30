using JoyLib.Code.Entities.Items;
using JoyLib.Code.States;

namespace JoyLib.Code.Helpers
{
    public static class BagOfGoldHelper
    {
        public static ItemInstance GetBagOfGold(int count)
        {
            ItemInstance bag = WorldState.ItemHandler.CreateSpecificType("Leather bag", "Leather bag");
            for (int i = 0; i < count; i++)
            {
                ItemInstance coin = WorldState.ItemHandler.CreateRandomItemOfType("Currency", true);
                bag.PutItem(coin.GUID);
            }

            return bag;
        }
    }
}
