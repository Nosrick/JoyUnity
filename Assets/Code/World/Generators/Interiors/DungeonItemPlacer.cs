using JoyLib.Code.Entities.Items;
using JoyLib.Code.Helpers;
using JoyLib.Code.Rollers;
using JoyLib.Code.States;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Graphics;
using JoyLib.Code.Unity;
using UnityEngine;

namespace JoyLib.Code.World.Generators.Interiors
{
    public class DungeonItemPlacer
    {
        protected IItemFactory ItemFactory { get; set; }

        protected ILiveItemHandler ItemHandler { get; set; }
        
        protected RNG Roller { get; set; }

        public DungeonItemPlacer(
            ILiveItemHandler itemHandler,
            RNG roller,
            IItemFactory itemFactory)
        {
            Roller = roller;
            ItemHandler = itemHandler;
            ItemFactory = itemFactory;
        }

        /// <summary>
        /// Places items in the dungeon
        /// </summary>
        /// <param name="worldRef">The world in which to place the items</param>
        /// <param name="prosperity">The prosperity of the world, the lower the better</param>
        /// <returns>The items placed</returns>
        public List<IItemInstance> PlaceItems(IWorldInstance worldRef, int prosperity = 50)
        {
            List<IItemInstance> placedItems = new List<IItemInstance>();

            int dungeonArea = worldRef.Tiles.GetLength(0) * worldRef.Tiles.GetLength(1);
            int itemsToPlace = dungeonArea / prosperity;

            List<Vector2Int> unavailablePoints = new List<Vector2Int>();
            foreach(IJoyObject wall in worldRef.Walls.Values)
            {
                unavailablePoints.Add(wall.WorldPosition);
            }

            for(int i = 0; i < itemsToPlace; i++)
            {
                Vector2Int point = new Vector2Int(Roller.Roll(1, worldRef.Tiles.GetLength(0) - 1), Roller.Roll(1, worldRef.Tiles.GetLength(1) - 1));

                while(unavailablePoints.Contains(point))
                {
                    point = new Vector2Int(Roller.Roll(1, worldRef.Tiles.GetLength(0) - 1), Roller.Roll(1, worldRef.Tiles.GetLength(1) - 1));
                }

                IItemInstance item = ItemFactory.CreateCompletelyRandomItem();
                item.MyWorld = worldRef;
                ItemHandler.AddItem(item, true);
                item.Move(point);
                placedItems.Add(item);
            }

            return placedItems;
        }


    }
}
