using JoyLib.Code.Graphics;
using System.Collections.Generic;

namespace JoyLib.Code.World.Generators.Overworld
{
    public class OverworldGenerator : IWorldSpaceGenerator
    {
        public WorldTile[,] GenerateWorldSpace(int sizeRef)
        {
            WorldTile[,] tiles = new WorldTile[sizeRef, sizeRef];

            for (int i = 0; i < tiles.GetLength(0); i++)
            {
                for(int j = 0; j < tiles.GetLength(1); j++)
                {
                    tiles[i, j] = WorldTile.Plains;
                }
            }

            return tiles;
        }

        public void GenerateTileObjects(WorldTile[,] tiles)
        {
        }

        public List<JoyObject> GenerateWalls(WorldTile[,] worldTiles)
        {
            return new List<JoyObject>();
        }
    }
}
