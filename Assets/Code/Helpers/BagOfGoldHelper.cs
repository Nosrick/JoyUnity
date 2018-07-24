using JoyLib.Code.Entities.Items;

namespace JoyLib.Code.Helpers
{
    public static class BagOfGoldHelper
    {
        public static ItemInstance GetBagOfGold(int count)
        {
            BaseItemType bagType = ItemHandler.GetSpecificItem("Leather bag");
            ItemInstance bag = new ItemInstance(bagType, new UnityEngine.Vector2Int(-1, -1), true);
            for (int i = 0; i < count; i++)
            {
                bag.PutItem(ItemProvider.RandomItemOfType("Currency", true));
            }

            return bag;
        }
    }
}
