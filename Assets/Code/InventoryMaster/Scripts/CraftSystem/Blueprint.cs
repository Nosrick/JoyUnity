using System.Collections.Generic;

namespace InventoryMaster.Scripts.CraftSystem
{
    [System.Serializable]
    public class Blueprint
    {

        public List<int> ingredients = new List<int>();
        public List<int> amount = new List<int>();
        public Item.Item finalItem;
        public int amountOfFinalItem;
        public float timeToCraft;

        public Blueprint(List<int> ingredients, int amountOfFinalItem, List<int> amount, Item.Item item)
        {
            this.ingredients = ingredients;
            this.amount = amount;
            finalItem = item;
        }

        public Blueprint() { }

    }
}