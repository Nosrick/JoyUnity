using JoyLib.Code.Entities.Items;
using JoyLib.Code.Helpers;
using JoyLib.Code.Rollers;
using JoyLib.Code.States;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JoyLib.Code.World.Generators.Interiors
{
    public class DungeonItemPlacer
    {
        protected static ItemFactory s_ItemFactory = new ItemFactory();

        protected static ILiveItemHandler s_ItemHandler = GlobalConstants.GameManager.ItemHandler;

        /// <summary>
        /// Places items in the dungeon
        /// </summary>
        /// <param name="worldRef">The world in which to place the items</param>
        /// <param name="prosperity">The prosperity of the world, the lower the better</param>
        /// <returns>The items placed</returns>
        public List<ItemInstance> PlaceItems(WorldInstance worldRef, int prosperity = 50)
        {
            List<ItemInstance> placedItems = new List<ItemInstance>();

            int dungeonArea = worldRef.Tiles.GetLength(0) * worldRef.Tiles.GetLength(1);
            int itemsToPlace = dungeonArea / prosperity;

            List<Vector2Int> unavailablePoints = new List<Vector2Int>();
            foreach(IJoyObject wall in worldRef.Walls.Values)
            {
                unavailablePoints.Add(wall.WorldPosition);
            }

            for(int i = 0; i < itemsToPlace; i++)
            {
                Vector2Int point = new Vector2Int(RNG.instance.Roll(1, worldRef.Tiles.GetLength(0) - 1), RNG.instance.Roll(1, worldRef.Tiles.GetLength(1) - 1));

                while(unavailablePoints.Contains(point))
                {
                    point = new Vector2Int(RNG.instance.Roll(1, worldRef.Tiles.GetLength(0) - 1), RNG.instance.Roll(1, worldRef.Tiles.GetLength(1) - 1));
                }

                ItemInstance item = s_ItemFactory.CreateCompletelyRandomItem();
                item.MyWorld = worldRef;
                s_ItemHandler.AddItem(item, true);
                item.Move(point);
                placedItems.Add(item);
            }

            return placedItems;
        }


    }
}
