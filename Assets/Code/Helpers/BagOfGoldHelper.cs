using System;
using System.Collections.Generic;
using JoyLib.Code.Entities.Items;
using UnityEngine;

namespace JoyLib.Code.Helpers
{
    public static class BagOfGoldHelper
    {
        private static ItemFactory s_ItemFactory = new ItemFactory();
        
        private static ILiveItemHandler ItemHandler { get; set; }

        public static ItemInstance GetBagOfGold(int count)
        {
            if (ItemHandler is null)
            {
                ItemHandler = GlobalConstants.GameManager.ItemHandler;
            }
            
            ItemInstance bag = s_ItemFactory.CreateRandomItemOfType(new string[] { "container", "leather" }, true);
            ItemHandler.AddItem(bag);
            List<ItemInstance> coins = new List<ItemInstance>();
            int gold = count / 100;
            int silver = (count - (gold * 100))  / 10;
            int copper = (count - (gold * 100) - (silver * 10));

            if (gold > 0)
            {
                ItemInstance goldCoin = s_ItemFactory.CreateSpecificType("gold coin", new string[] {"currency"}, true);
                for (int i = 0; i < gold; i++)
                {
                    coins.Add(goldCoin.Copy(goldCoin));
                }
            }
            
            if (silver > 0)
            {
                ItemInstance silverCoin = s_ItemFactory.CreateSpecificType("silver coin", new string[] {"currency"}, true);
                for (int i = 0; i < silver; i++)
                {
                    coins.Add(silverCoin.Copy(silverCoin));
                }
            }

            if (copper > 0)
            {
                ItemInstance copperCoin = s_ItemFactory.CreateSpecificType("copper coin", new string[] {"currency"}, true);
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
