using JoyLib.Code.Entities.Items;
using JoyLib.Code.Helpers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JoyLib.Code.World.Generators.Interiors
{
    public static class DungeonItemPlacer
    {
        public static List<ItemInstance> PlaceItems(WorldInstance worldRef)
        {
            List<ItemInstance> placedItems = new List<ItemInstance>();

            int dungeonArea = worldRef.Tiles.GetLength(0) * worldRef.Tiles.GetLength(1);
            int itemsToPlace = dungeonArea / 50;

            List<Vector2Int> unavailablePoints = new List<Vector2Int>();
            foreach(JoyObject wall in worldRef.Objects.Where(x => x.IsWall))
            {
                unavailablePoints.Add(wall.WorldPosition);
            }

            for(int i = 0; i < itemsToPlace; i++)
            {
                Vector2Int point = new Vector2Int(RNG.Roll(1, worldRef.Tiles.GetLength(0) - 1), RNG.Roll(1, worldRef.Tiles.GetLength(1) - 1));

                while(unavailablePoints.Contains(point))
                {
                    point = new Vector2Int(RNG.Roll(1, worldRef.Tiles.GetLength(0) - 1), RNG.Roll(1, worldRef.Tiles.GetLength(1) - 1));
                }

                ItemInstance item = ItemProvider.RandomItem(false);
                item.Move(point);
                placedItems.Add(item);
            }

            return placedItems;
        }


    }
}
