using System;
using System.Collections.Generic;
using JoyLib.Code.Entities.Items;
using UnityEngine;

namespace JoyLib.Code.Helpers
{
    public static class BagOfGoldHelper
    {
        private static ItemFactory ItemFactory { get; set; }
        
        private static ILiveItemHandler ItemHandler { get; set; }

        public static ItemInstance GetBagOfGold(int count)
        {
            if (ItemHandler is null)
            {
                ItemHandler = GlobalConstants.GameManager.ItemHandler;
            }

            if (ItemFactory is null)
            {
                ItemFactory = GlobalConstants.GameManager.ItemFactory;
            }
            
            ItemInstance bag = ItemFactory.CreateRandomItemOfType(new string[] { "container", "leather" }, true);
            ItemHandler.AddItem(bag);
            List<ItemInstance> coins = new List<ItemInstance>();
            int gold = count / 100;
            int silver = (count - (gold * 100))  / 10;
            int copper = (count - (gold * 100) - (silver * 10));

            if (gold > 0)
            {
                ItemInstance goldCoin = ItemFactory.CreateSpecificType("gold coin", new string[] {"currency"}, true);
                for (int i = 0; i < gold; i++)
                {
                    coins.Add(goldCoin.Copy(goldCoin));
                }
            }
            
            if (silver > 0)
            {
                ItemInstance silverCoin = ItemFactory.CreateSpecificType("silver coin", new string[] {"currency"}, true);
                for (int i = 0; i < silver; i++)
                {
                    coins.Add(silverCoin.Copy(silverCoin));
                }
            }

            if (copper > 0)
            {
                ItemInstance copperCoin = ItemFactory.CreateSpecificType("copper coin", new string[] {"currency"}, true);
                for (int i = 0; i < copper; i++)
                {
                    coins.Add(copperCoin.Copy(copperCoin));
                }
            }
            
            foreach(ItemInstance coin in coins)
            {
                ItemHandler.AddItem(coin);
            }
            bag.AddContents(coins);

            return bag;
        }
    }
}
